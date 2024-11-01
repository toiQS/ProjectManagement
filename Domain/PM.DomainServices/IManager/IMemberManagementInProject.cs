using PM.Domain.DTOs;

namespace PM.DomainServices.IManager
{
    public interface IMemberManagerInProject
    {

        //userid is id of user current using system
        //application user is id of orther user or self user current on system
        public Task<IEnumerable<Dictionary<string, object>>> GetAllMember(string userId, string roleInSystemId);
        public Task<IEnumerable<Dictionary<string,object>>> GetListMemberInProjectByProjectId(string userId,string projectId);
        public Task<Dictionary<string,string>> GetMemberInProjectByProjectIdAndApplicationUserId(string userId,string projectId,string applicationUserId);
        // public Task<Dictionary<string, string>> AddMemberToProject(string userId, string projectId, string roleId, string applicationUser);
        // public Task<Dictionary<string,string>> RemoveMemberFromProject(string userId, string applicationUser, string projectId);
        // public Task<Dictionary<string, string>> UpdateRoleOfMemberInProject(string userId, string applicationUser, string projectId, string roleId);
    }
}