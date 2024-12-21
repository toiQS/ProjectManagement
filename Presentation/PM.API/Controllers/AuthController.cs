using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.appUser;

namespace PM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //private string _authPath = "http://localhost:5298/api/auth/";
        //private readonly ILogger _logger;
        //private readonly HttpClient _httpClient;
        //public AuthController(ILogger<AuthController> logger)
        //{
        //    _httpClient = new HttpClient();
        //    _logger = logger;
        //    _httpClient.BaseAddress = new Uri(_authPath);
        //}
        //public async Task Login(string email, string password)
        //{
        //    var model = new LoginModel()
        //    {
        //        Email = email,
        //        Password = password
        //    };
        //    await _httpClient.PostAsJsonAsync<LoginModel>("login", model);
        //}
    }
}
