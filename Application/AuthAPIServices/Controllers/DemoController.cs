using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthAPIServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        public DemoController() { }
        [HttpGet]
        public async Task<IActionResult> GetDemo()
        {
            return Ok("Success");
        }
    }
}
