using Microsoft.AspNetCore.Mvc;
using Shared.appUser;
using System.Net.Http.Json;

namespace PM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private string _gatewayUrl = "http://localhost:5000/auth/"; // URL của Ocelot Gateway

        public AuthController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_gatewayUrl);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var model = new LoginModel
            {
                Email = email,
                Password = password
            };

            // Gửi yêu cầu đến Ocelot Gateway
            var response = await _httpClient.PostAsJsonAsync("login", model);

            // Kiểm tra trạng thái phản hồi
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, new { Error = error });
            }

            var result = await response.Content.ReadAsStringAsync();
            return Ok(new { Message = "Login successful", Data = result });
        }
    }
}
