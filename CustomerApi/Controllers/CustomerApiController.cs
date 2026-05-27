using Microsoft.AspNetCore.Mvc;
using CustomerApi.Models;
using CustomerApi.Services;

namespace CustomerApi.Controllers
{
    [ApiController]
    [Route("api/customer")]
    public class CustomerApiController : ControllerBase
    {
        private readonly IRegisterService _registerService;
        private readonly ILoginService _loginService;

        public CustomerApiController(IRegisterService registerService, ILoginService loginService)
        {
            _registerService = registerService;
            _loginService = loginService;
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
    }
}
