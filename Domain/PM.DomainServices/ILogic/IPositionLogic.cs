using PM.DomainServices.Models;

namespace PM.DomainServices.ILogic
{
    public interface IPositionLogic
    {
        Task<ServicesResult<string>> GetPositionWorkByMemberId(string memberId);

    }
}
