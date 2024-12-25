using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.appUser;
using System.Net.Http.Json;

namespace PM.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public AuthController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:5000");
        }
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) return BadRequest();
            var model = new LoginModel()
            {
                Email = email,
                Password = password
            };
            var response = await _httpClient.PostAsJsonAsync("/auth/login", model);
            if (response.IsSuccessStatusCode)
            {
                return Ok(response.Content.ReadAsStringAsync().Result);
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string username, string email, string password)
        {
            var model = new RegisterModel()
            {
                Email = email,
                Password = password,
                UserName = username
            };
            var response = await _httpClient.PostAsJsonAsync("/auth/register", model);
            if (response.IsSuccessStatusCode)
            {
                return Ok(response.Content.ReadAsStringAsync().Result);
            }
            return BadRequest();
        }
    }
}
