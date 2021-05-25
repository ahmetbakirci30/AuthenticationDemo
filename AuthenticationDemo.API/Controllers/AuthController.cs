using AuthenticationDemo.API.Services;
using AuthenticationDemo.API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthenticationDemo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _service;

        public AuthController(IUserService service)
        {
            _service = service;
        }

        // POST api/Auth/Register
        [HttpPost("Register")]
        public async Task<ActionResult<string>> Register([FromBody] RegisterViewModel model)
        {
            var result = await _service.Register(model);

            return result.Succeeded ? Ok(result.Message) : BadRequest(result.Message);
        }

        // POST api/Auth/Login
        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginViewModel model)
        {
            var result = await _service.Login(model);

            return result.Succeeded ? Ok(result.Message) : BadRequest(result.Message);
        }

        // POST api/Auth/RegisterAsync
        [HttpPost("RegisterAsync")]
        public async Task<ActionResult<string>> RegisterAsync([FromBody] RegisterViewModel model)
        {
            var (succeeded, message) = await _service.RegisterAsync(model);

            return succeeded ? Ok(message) : BadRequest(message);
        }

        // POST api/Auth/LoginAsync
        [HttpPost("LoginAsync")]
        public async Task<ActionResult<string>> LoginAsync([FromBody] LoginViewModel model)
        {
            var (succeeded, message) = await _service.LoginAsync(model);

            return succeeded ? Ok(message) : BadRequest(message);
        }
    }
}