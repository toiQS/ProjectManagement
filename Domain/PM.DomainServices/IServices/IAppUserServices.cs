using PM.Domain;
using PM.DomainServices.Models;
using PM.DomainServices.Models.users;

namespace PM.Persistence.IServices
{
    /// <summary>
    /// Service interface for managing application user operations.
    /// </summary>
    public interface IApplicationUserServices
    {
        public Task<ServicesResult<DetailAppUser>> GetAppUserByIdOrEmail(string text);
        public Task<ServicesResult<bool>> UpdateInfoUser(string userId, UpdateAppUser updateAppUser);
    }
}
