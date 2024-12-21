using Microsoft.AspNetCore.Mvc;
using Shared.appUser;

namespace PM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private string _authPath = "http://localhost:5008/gateway/auth/";
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        public AuthController(ILogger<AuthController> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;
            _httpClient.BaseAddress = new Uri(_authPath);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) return BadRequest();

            var model = new LoginModel()
            {
                Email = email,
                Password = password
            };
            var result = await _httpClient.PostAsJsonAsync<LoginModel>("login", model);
            if (result == null) return BadRequest();
            var data = await result.Content.ReadAsStringAsync();
            if (data == null) return BadRequest();
            return Ok(data);
        }
        //public Task<IActionResult> RegisterUser(string userName, string email, string password)
        //{
        //    if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) return BadRequest();
        //}
    }
}