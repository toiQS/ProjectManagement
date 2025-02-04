using PM.DomainServices.Models;
using PM.DomainServices.Models.users;

namespace PM.DomainServices.ILogics
{
    public interface IUserLogic
    {
        Task<ServicesResult<DetailAppUser>> DetailUserCurrent(string userId);
        Task<ServicesResult<DetailAppUser>> UpdateInfo(string userId, UpdateAppUser newInfo);
    }
}
