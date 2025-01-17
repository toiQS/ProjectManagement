using PM.Domain;
using PM.DomainServices.Models;

namespace PM.DomainServices.ILogic
{
    public interface IPositionLogic
    {
        Task<ServicesResult<string>> GetPositionWorkByMemberId(string memberId);
        Task<ServicesResult<PositionWorkOfMember>> GetPositionWorkOfMember(string memberId);

    }
}
