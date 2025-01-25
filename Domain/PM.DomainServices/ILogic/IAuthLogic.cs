using PM.DomainServices.Models.auths;
using PM.DomainServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.DomainServices.ILogic
{
    public interface IAuthLogic
    {
        Task<ServicesResult<AppUserClaim>> Login(LoginModel loginModel);
        Task<ServicesResult<bool>> Register(RegisterModel registerModel);
    }
}
