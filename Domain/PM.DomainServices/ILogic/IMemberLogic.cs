using PM.DomainServices.Models;
using PM.DomainServices.Models.members;
using PM.DomainServices.Models.users;
using Shared.member;

namespace PM.DomainServices.ILogic
{
    /// <summary>
    /// Defines the contract for member-related logic in a project management system.
    /// </summary>
    public interface IMemberLogic
    {
        public Task<ServicesResult<DetailAppUser>> CheckAndGetUser(string userId);
        public Task<ServicesResult<bool>> CheckProjectIsExisted(string projectId);
        public Task<ServicesResult<IEnumerable<IndexMember>>> GetMembersInProject();
        public Task<ServicesResult<DetailMember>> GetMemberByMemberId(string memberId);
        public Task<ServicesResult<bool>> Add(string appUserId, AddMember addMember);
        public Task<ServicesResult<bool>> UpdateInfo(string memberId, UpdateMember updateMember);
    }
}
