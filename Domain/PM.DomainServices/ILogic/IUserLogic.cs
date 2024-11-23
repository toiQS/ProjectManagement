using PM.Domain;
using PM.DomainServices.Shared;

namespace PM.DomainServices.ILogic;
public interface IUserLogic
{
    public Task<ServicesResult<ApplicationUser>> Login(string email, string password );
    public Task<ServicesResult<bool>> RegisterUser(string userName,string email, string password);
    public Task<ServicesResult<string>> GetRoleNameOfUser(string userId);

}