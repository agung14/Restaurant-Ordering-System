using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrdering.Infrastructure.Repository.Interface;
using RestaurantOrdering.Model.Request;
using RestaurantOrdering.Model.Response;
using RestaurantOrdering.WebAPI.JwtService;

namespace RestaurantOrdering.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private IConfiguration _config;
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(IConfiguration config, ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
            _config = config;
        }
        

        /// <summary>
        /// STEP: 1 - Generate JWT ketika reserve, Token valid selama 6 jam
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("reservetable")]
        public async Task<TokenResponse> GetRandomToken([FromBody] CustomerRequest request)
        {
            TokenResponse response = new TokenResponse();

            var jwt = new JwtServices(_config);

            var token = jwt.GenerateJwt(request.TableCode!, request.Name!, request.Email!);

            var IsSuccess = await _customerRepository.Create(request, token);

            response.token = token;

            return response;
        }

        /// <summary>
        /// Get All Dishes
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("dishes")]
        public async Task<IActionResult> GetAllDishes()
        {
            var Dishes = await _customerRepository.GetAllDish();

            return Ok(Dishes);
        }


        /// <summary>
        /// Get All Vouchers
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("vouchers")]
        public async Task<IActionResult> GetAllVouchers()
        {
            var vouchers = await _customerRepository.GetAllVoucher();

            return Ok(vouchers);
        }

    }
}
