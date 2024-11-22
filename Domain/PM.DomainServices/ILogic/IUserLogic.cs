using PM.DomainServices.Shared;

namespace PM.DomainServices.ILogic;
public interface IUserLogic
{
    public Task<ServicesResult<bool>> Login(string email, string password );
    public Task<ServicesResult<bool>> RegisterUser(string email, string password);
}