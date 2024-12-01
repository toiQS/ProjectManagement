using Microsoft.AspNetCore.Mvc;

namespace RootAPIServices.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        public async Task<IActionResult> method1()
        {
            return Ok(1);
        }
        public async Task<IActionResult> method2(string demo)
        {
            return Ok(demo);  
        }

    }
}
