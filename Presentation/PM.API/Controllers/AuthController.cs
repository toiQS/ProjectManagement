using Microsoft.AspNetCore.Mvc;
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
        }
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var model = new LoginModel()
            {
                Email = email,
                Password = password
            };
            _httpClient.BaseAddress = new Uri("http://localhost/5000/auth");
            var response = await _httpClient.PostAsJsonAsync<LoginModel>("login",model);
            return Ok(response.Content);
        }
    }
}
