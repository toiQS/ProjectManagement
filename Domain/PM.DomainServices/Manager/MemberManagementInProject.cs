using System.Threading.Tasks.Dataflow;
using PM.DomainServices.IManager;
using PM.Persistence.IServices;

namespace PM.DomainServices.Manager
{
    public class MemberManagementInProject : IMemberManagerInProject
    {
        private readonly IProjectServices _projectServices;
        private readonly IApplicationUserServices _applicationUserServices;
        private readonly IRoleInProjectServices _roleInProjectServices;
        private readonly IRoleApplicationUserInProjectServices _roleApplicationUserInProjectServices;
        private readonly Dictionary<string, string> _finalResult;
        private readonly List<Dictionary<string, object>> _finalResultList;
        public MemberManagementInProject(IProjectServices projectServices, IApplicationUserServices applicationUserServices,
         IRoleInProjectServices roleInProjectServices, IRoleApplicationUserInProjectServices roleApplicationUserInProjectServices)
        {
            _projectServices = projectServices;
            _applicationUserServices = applicationUserServices;
            _roleInProjectServices = roleInProjectServices;
            _roleApplicationUserInProjectServices = roleApplicationUserInProjectServices;
            _finalResult = new Dictionary<string, string>();
            _finalResultList = new List<Dictionary<string, object>>();
        }
        /// <summary>
        /// get all user name in system and if only admin role can get it
        /// </summary>
        /// <param name="userId">id of user current</param>
        /// <param name="roleInSystemId">id role of user current </param>
        /// <returns>list dictionary<string,object>></returns>
        public async Task<IEnumerable<Dictionary<string, object>>> GetAllMember(string userId, string roleInSystemId)
        {
            if (userId == null || roleInSystemId == null)
            {
                _finalResultList.Add(new Dictionary<string, object>()
                {
                    {"Message","Data input are invalid"}
                });
                return _finalResultList;
            }
            else
            {
                var findUser = await _applicationUserServices.GetApplicationUserAsync(userId);
                var findRole = await _applicationUserServices.GetRoleAsync(roleInSystemId);
                if (findUser == null || findRole == null)
                {
                    _finalResultList.Add(new Dictionary<string, object>()
                    {
                        {
                            "Message","User is not found."
                        }
                    });
                    return _finalResultList;
                }
                else
                {
                    if (!findRole.Name.Equals("Admin"))
                    {
                        _finalResultList.Add(new Dictionary<string, object>()
                        {
                            {"Message","You must have administrator role to use this feature."},
                        });
                        return _finalResultList;
                    }
                    else
                    {
                        var getAllMemberInSystem = await _applicationUserServices.GetAllUser();
                        if (!getAllMemberInSystem.Any() || getAllMemberInSystem == null)
                        {
                            _finalResultList.Add(new Dictionary<string, object>()
                            {
                                {"Message","Can't find all user."}
                            });
                            return _finalResultList;
                        }
                        else
                        {
                            foreach(var member in getAllMemberInSystem)
                            {
                                int i = 1;
                                _finalResultList.Add(new Dictionary<string, object>()
                                {
                                    {"Index",i++},
                                    {"User Name",member.UserName}
                                });
                                
                            }
                        }
                    }
                }
            }
            return _finalResultList;
        }
        /// <summary>
        /// get list member in a project specific and when you is a member in this project
        /// </summary>
        /// <param name="userId">id of user current</param>
        /// <param name="projectId">id of a project specific</param>
        /// <returns></returns>
        public async Task<IEnumerable<Dictionary<string, object>>> GetListMemberInProjectByProjectId(string userId, string projectId)
        {
            if (userId == null || projectId == null)
            {
                _finalResultList.Add(new Dictionary<string, object>()
                {
                    {"Message","Data input are invalid"}
                });
                return _finalResultList;
            }
            var findUser = await _applicationUserServices.GetApplicationUserAsync(userId);
            var findMember = await _roleApplicationUserInProjectServices.GetRoleApplicationUserInProjectByUserIdAndProjectId(userId, projectId);
            if(findUser == null || findMember == null)
            {
                _finalResultList.Add( new Dictionary<string, object>()
                {
                    {"Message","Can't authorize you are a user or a member in this project."}
                });
                return _finalResultList;
            } 
            var getMembers = await _roleApplicationUserInProjectServices.GetRoleApplicationUserInProjectsByProjectId(projectId);
            if(getMembers == null || !getMembers.Any())
            {
                _finalResultList.Add(new Dictionary<string, object>()
                {
                    {"Message","Can't get all memeber in this project"}
                });
                return _finalResultList;
            }
            foreach(var member in getMembers)
            {
                var getRoleName = await _roleInProjectServices.GetNameRoleByRoleId(member.RoleInProjectId);
                if(getRoleName == null || !getRoleName.Any())
                {
                    _finalResultList.Add(new Dictionary<string, object>()
                    {
                        {"Message","Can't get role name"}
                    });
                    break;
                }
                var getUser = await _applicationUserServices.GetApplicationUserAsync(member.ApplicationUserId);
                if(getUser == null )
                {
                    _finalResultList.Add(new Dictionary<string, object>()
                    {
                        {"Message","Can't find user"}
                    });
                    break;
                }
                _finalResultList.Add(new Dictionary<string, object>()
                {
                    {"User Name",getUser.UserName},
                    {"Role Name",getRoleName}
                });
                return _finalResultList;
            }
            return _finalResultList;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="projectId"></param>
        /// <param name="applicationUserId"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetMemberInProjectByProjectIdAndApplicationUserId(string userId, string projectId, string applicationUserId)
        {

            if(userId == null || projectId == null || applicationUserId == null)
            {
                
            }
            return _finalResult;
        }
        // public Task<Dictionary<string, string>> AddMemberToProject(string userId, string projectId, string roleId, string applicationUser);
        // public Task<Dictionary<string, string>> RemoveMemberFromProject(string userId, string applicationUser, string projectId);
        // public Task<Dictionary<string, string>> UpdateRoleOfMemberInProject(string userId, string applicationUser, string projectId, string roleId);
    }
}