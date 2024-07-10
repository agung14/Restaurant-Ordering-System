using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrdering.Infrastructure.Repository.Interface;
using RestaurantOrdering.Model.Request;
using RestaurantOrdering.Model.Response;
using RestaurantOrdering.WebAPI.JwtService;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace RestaurantOrdering.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private IConfiguration _config;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICartRepository _cartRepository;

        public CartController(IConfiguration config, ICustomerRepository customerRepository, ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
            _customerRepository = customerRepository;
            _config = config;
        }

        /// <summary>
        /// Get Active Cart
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("activecart")]
        public async Task<IActionResult> GetActiveCart()
        {
            ActiveCartResponse response = new ActiveCartResponse();

            var token = Request.Headers["Authorization"].ToString();

            token = token.Replace("Bearer ", "");

            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(jwtSecurityToken.Payload.Exp!.Value).DateTime.ToLocalTime();

            if (DateTime.Now > expirationTime)
            {
                return Unauthorized();
            }

            var CartId = await _cartRepository.GetCartIdByToken(token);

            response.CartId = CartId;

            return Ok(response);
        }

        /// <summary>
        /// Get All Dish From Cart
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("dish")]
        public async Task<IActionResult> GetAllDishFromCart()
        {
            CartItemResponse response = new CartItemResponse();

            var token = Request.Headers["Authorization"].ToString();

            token = token.Replace("Bearer ", "");

            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(jwtSecurityToken.Payload.Exp!.Value).DateTime.ToLocalTime();

            if (DateTime.Now > expirationTime)
            {
                return Unauthorized();
            }

            var CartId = await _cartRepository.GetCartIdByToken(token);

            var DishFromCart = await _cartRepository.GetAllDishByCartId(CartId);

            response.CartItem = DishFromCart;

            return Ok(response);
        }

        /// <summary>
        /// STEP 2 - Add item/dish to cart
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("dish/add")]
        public async Task<IActionResult> AddDishToCart([FromBody] AddDishToCartRequest request)
        {
            var token = Request.Headers["Authorization"].ToString();

            token = token.Replace("Bearer ", "");

            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(jwtSecurityToken.Payload.Exp!.Value).DateTime.ToLocalTime();

            if (DateTime.Now > expirationTime)
            {
                return Unauthorized();
            }

            var CartId = await _cartRepository.GetCartIdByToken(token);

            var SuccessAddItemToCart = await _cartRepository.Create(request, CartId);


            return Ok();
        }

        /// <summary>
        /// Update chart QTY item
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [Route("dish/update")]
        public async Task<IActionResult> UpdateCart([FromBody] AddDishToCartRequest request)
        {
            var token = Request.Headers["Authorization"].ToString();

            token = token.Replace("Bearer ", "");

            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(jwtSecurityToken.Payload.Exp!.Value).DateTime.ToLocalTime();

            if (DateTime.Now > expirationTime)
            {
                return Unauthorized();
            }

            var CartId = await _cartRepository.GetCartIdByToken(token);
            var UpdateCart = await _cartRepository.UpdateCartItem(CartId, request.DishId, request.Qty);


            return Ok();
        }

        /// <summary>
        /// Delete chart item
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete]
        [Route("dish/delete/{dishid}")]
        public async Task<IActionResult> DeleteCartItem(int dishid)
        {
            var token = Request.Headers["Authorization"].ToString();

            token = token.Replace("Bearer ", "");

            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(jwtSecurityToken.Payload.Exp!.Value).DateTime.ToLocalTime();

            if (DateTime.Now > expirationTime)
            {
                return Unauthorized();
            }

            var CartId = await _cartRepository.GetCartIdByToken(token);
            var DeleteCart = await _cartRepository.DeleteCartItem(CartId, dishid);


            return Ok();
        }
    }
}
