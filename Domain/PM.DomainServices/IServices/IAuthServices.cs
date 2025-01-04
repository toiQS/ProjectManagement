using PM.Domain;
using PM.DomainServices.Models;

namespace PM.DomainServices.IServices
{
    public interface IAuthServices
    {
        public Task<ServicesResult<ApplicationUser>> Login(string email, string password);
        public Task<ServicesResult<bool>> Logout(string userId);
        public Task<ServicesResult<bool>> Register(string email, string username, string password);
    }
}
