using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CustomerApi.Models;
using CustomerApi.Services;
using CustomerApi.Helpers;

namespace CustomerApi.Controllers
{
    [ApiController]
    [Route("api/customer")]
    public class CustomerApiController : ControllerBase
    {
        private readonly IRegisterService _registerService;
        private readonly ILoginService _loginService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ICustomerService _customerService;

        public CustomerApiController(IRegisterService registerService, ILoginService loginService, IJwtTokenService jwtTokenService, ICustomerService customerService)
        {
            _registerService = registerService;
            _loginService = loginService;
            _jwtTokenService = jwtTokenService;
            _customerService = customerService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Username and password are required.");

            var ok = await _registerService.RegisterAsync(request);
            if (!ok)
                return Conflict("Username already exists.");

            return CreatedAtAction(nameof(Register), new { username = request.Username }, null);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Username and password are required.");

            var response = await _loginService.LoginAsync(request);
            if (!response.Success)
                return Unauthorized(response);

            return Ok(response);
        }

        [HttpGet("profile")]
        [Authorize]
        public IActionResult GetProfile()
        {
            var customerId = AuthHelper.GetCustomerIdFromRequest(Request, _jwtTokenService);
            if (customerId == null)
                return Unauthorized("Invalid token");

            return Ok(new { message = $"This is a protected endpoint. Your customer ID is: {customerId}" });
        }

        [HttpGet("info")]
        [Authorize]
        public async Task<IActionResult> GetCustomerInfo()
        {
            var customerId = AuthHelper.GetCustomerIdFromRequest(Request, _jwtTokenService);
            if (customerId == null)
                return Unauthorized("Invalid token");

            var customer = await _customerService.GetCustomerInfoAsync(customerId.Value);
            if (customer == null)
                return NotFound("Customer not found");

            return Ok(customer);
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            var customerId = AuthHelper.GetCustomerIdFromRequest(Request, _jwtTokenService);
            if (customerId == null)
                return Unauthorized("Invalid token");

            return Ok(new { message = "Logout successful. Please remove the token from your client." });
        }
    }
}
