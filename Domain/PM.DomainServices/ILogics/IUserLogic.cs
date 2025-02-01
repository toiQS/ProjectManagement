using PM.DomainServices.Models.users;
using PM.DomainServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.DomainServices.ILogics
{
    public interface IUserLogic : IDisposable
    {
        Task<ServicesResult<DetailAppUser>> DetailUserCurrent(string userId);
        Task<ServicesResult<DetailAppUser>> UpdateInfo(string userId, DetailAppUser newInfo);
    }
}
