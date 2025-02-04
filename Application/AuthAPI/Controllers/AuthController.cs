using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM.DomainServices.ILogics;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthLogic _authLogic;
        private readonly ILogger _logger;
        private IHttpContextAccessor _contextAccessor;
        public AuthController(IAuthLogic authLogic, ILogger logger, IHttpContextAccessor contextAccessor)
        {
            _authLogic = authLogic;
            _logger = logger;
            _contextAccessor = contextAccessor;
        }
        
    }
}
