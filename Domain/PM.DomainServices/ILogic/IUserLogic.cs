using PM.DomainServices.Models;
using PM.DomainServices.Models.projects;
using PM.DomainServices.Models.users;

namespace PM.DomainServices.ILogic
{
    public interface IUserLogic
    {
        Task<ServicesResult<DetailAppUser>> GetInfoOtherUserByUserId(string userId);
        Task<ServicesResult<DetailAppUser>> GetInfoUserCurrent(string userCurrentId);
        Task<ServicesResult<IEnumerable<IndexProject>>> GetListProjectsUserHasJoined();
        Task<ServicesResult<IEnumerable<IndexProject>>> GetListProjectsUserHasOwn();
    }
}
