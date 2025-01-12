using PM.DomainServices.Models.users;
using PM.DomainServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.DomainServices.ILogic
{
    public interface IUserLogic
    {
        Task<ServicesResult<DetailAppUser>> GetInfoOtherUserByUserId(string userId);
    }
}
