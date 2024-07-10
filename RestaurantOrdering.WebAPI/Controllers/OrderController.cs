using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using MimeKit;
using RestaurantOrdering.Infrastructure.Repository.Interface;
using RestaurantOrdering.Model.Request;
using RestaurantOrdering.Model.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RestaurantOrdering.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private IConfiguration _config;
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public OrderController(IConfiguration config, IWebHostEnvironment webHostEnvironment, IOrderRepository orderRepository, ICartRepository cartRepository, ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _config = config;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// STEP 3 - Order Semua item yang ada di keranjang belanja, dan buat keranjang belanja baru, kemudian kirim bukti order lewat email
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("checkout")]
        public async Task<IActionResult> PostCheckout(OrderRequest request)
        {
            #region Get JWT Claim Identity
            var token = Request.Headers["Authorization"].ToString();

            token = token.Replace("Bearer ", "");

            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(jwtSecurityToken.Payload.Exp!.Value).DateTime.ToLocalTime();
            var email = jwtSecurityToken.Claims.First(c => c.Type == "Email").Value;
            var name = jwtSecurityToken.Claims.First(c => c.Type == "Name").Value;


            if (DateTime.Now > expirationTime)
            {
                return Unauthorized();
            }

            #endregion

            #region Get CustomerID dan CartID
            var CustomerId = await _customerRepository.GetCustIdByToken(token);
            var CartId = await _cartRepository.GetCartIdByToken(token);
            #endregion

            #region Hitung total harga
            var DishFromCart = await _cartRepository.GetAllDishByCartId(CartId);

            UpdateCartRequest requestupdate = new UpdateCartRequest();
            int totalprice = 0;
            int totalqty = 0;
            string htmlitem = "<tr class=\"item\">\r\n\t\t\t\t\t<td style=\"padding: 5px;vertical-align: top;border-bottom: 1px solid #eee;\">[item]</td>\r\n\r\n\t\t\t\t\t<td style=\"padding: 5px;vertical-align: top;text-align: right;border-bottom: 1px solid #eee;\">Rp [price]</td>\r\n\t\t\t\t</tr>\t";
            string htmldiscount = "<tr class=\"item last\">\r\n\t\t\t\t\t<td style=\"padding: 5px;vertical-align: top;border-bottom: none;\">Voucher Discount: [discountname]</td>\r\n\r\n\t\t\t\t\t<td style=\"padding: 5px;vertical-align: top;text-align: right;border-bottom: none;\">Rp [discountamount]</td>\r\n\t\t\t\t</tr>";
            string appendhtmlitem = "";
            string appendhtmldiscount = "";

            foreach (var item in DishFromCart)
            {
                var htmlreplace = htmlitem.Replace("[item]", item.DishName + " x " + item.Qty);
                htmlreplace = htmlreplace.Replace("[price]", item.Price.ToString());
                appendhtmlitem += htmlreplace + "\n";

                htmlreplace = "";

                totalprice += (item.Price * item.Qty);
                totalqty += item.Qty;
            }

            var discountamount = 0;
            if (!string.IsNullOrEmpty(request.VoucherCode))
            {
                discountamount = await _orderRepository.GetDiscountVoucherByCode(request.VoucherCode);
                
                if (discountamount <= 0)
                {
                    request.VoucherCode = "";
                }
                else
                {
                    appendhtmldiscount = htmldiscount.Replace("[discountamount]", discountamount.ToString());
                    appendhtmldiscount = appendhtmldiscount.Replace("[discountname]", request.VoucherCode);
                }

                
            }

            requestupdate.cartid = CartId;
            requestupdate.totalitem = totalqty;
            requestupdate.totalprice = discountamount > 0 ? (totalprice - discountamount) : totalprice;
            requestupdate.ischeckout = true;

            if (totalprice <= 0)
            {
                return Ok();
            }
            #endregion


            #region Order semua yang ada di cart dan kirim email
            var SuccessUpdateCart = await _orderRepository.Update(requestupdate);
            var OrderData = await _orderRepository.CreateOrderLog(CartId, request.VoucherCode);

            var SuccessCreateNewCart = await _cartRepository.CreateNewCart(CustomerId);

            if (SuccessUpdateCart)
            {
                String MailBody = "";

                string path = Path.Combine(_webHostEnvironment.ContentRootPath, "html\\invoice.html");
                path = path.Replace("\\", "/");
                MailBody = System.IO.File.ReadAllText(path);
                MailBody = MailBody.Replace("[orderid]", OrderData.orderid.ToString());
                MailBody = MailBody.Replace("[ordertime]", OrderData.ordertime.ToString("dd-MM-yyyy HH:mm:ss"));
                MailBody = MailBody.Replace("[email]", email);
                MailBody = MailBody.Replace("[name]", name);
                MailBody = MailBody.Replace("[htmlitem]", appendhtmlitem);
                MailBody = MailBody.Replace("[htmldiscount]", appendhtmldiscount);
                MailBody = MailBody.Replace("[totalprice]", requestupdate.totalprice.ToString());

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Muhammad Agung Laksono", "agungcakery@gmail.com"));
                message.To.Add(new MailboxAddress(name, email));
                message.Subject = "Your Order Receipt";

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = MailBody;

                message.Body = bodyBuilder.ToMessageBody();


                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    client.Authenticate("agungcakery@gmail.com", "judt yxxh ivcv izrn");
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            #endregion


            return Ok();
        }

    }
}
