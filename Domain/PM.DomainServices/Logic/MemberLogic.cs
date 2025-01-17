using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using PM.Domain;
using PM.DomainServices.ILogic;
using PM.DomainServices.Models;
using PM.DomainServices.Models.members;
using PM.Persistence.IServices;
using Shared.member;
using System.Diagnostics;

namespace PM.DomainServices.Logic
{
    public class MemberLogic : IMemberLogic
    {
        //intialize services
        private readonly IRoleApplicationUserInProjectServices _roleApplicationUserInProjectServices;
        private readonly IRoleInProjectServices _roleInProjectServices;
        private readonly IMemberInTaskServices _memberInTaskServices;
        
        //intialize logic
        private readonly IUserLogic _userLogic;
        private readonly IPositionLogic _positionLogic;
        private readonly IRoleLogic _roleLogic; 

        //intialize primary value
        private List<RoleApplicationUserInProject> _member;
        private List<MemberInTask> _memberInTask; 
 
        public MemberLogic(IRoleApplicationUserInProjectServices roleApplicationUserInProjectServices, IRoleInProjectServices roleInProjectServices, IMemberInTaskServices memberInTaskServices, IUserLogic userLogic, IPositionLogic positionLogic, IRoleLogic roleLogic)
        {
            _roleApplicationUserInProjectServices = roleApplicationUserInProjectServices;
            _roleInProjectServices = roleInProjectServices;
            _memberInTaskServices = memberInTaskServices;
            _userLogic = userLogic;
            _positionLogic = positionLogic;
            _roleLogic = roleLogic;
            Intialize();
        }



        #region private method
        #region Retrieve all role-application-user mappings
        /// <summary>
        /// Retrieves all role-application-user mappings from the database.
        /// </summary>
        /// <returns>
        /// A service result containing a list of <see cref="RoleApplicationUserInProject"/> objects 
        /// or an error message if the operation fails.
        /// </returns>
        private async Task<ServicesResult<IEnumerable<RoleApplicationUserInProject>>> GetAllRolesAsync()
        {
            // Retrieve all data from the service
            var result = await _roleApplicationUserInProjectServices.GetAllAsync();

            // Handle cases where data is null
            if (result.Data == null)
                return ServicesResult<IEnumerable<RoleApplicationUserInProject>>.Success(
                    new List<RoleApplicationUserInProject>(), "No data found in the database."
                );

            // Handle failure response from the service
            if (!result.Status)
                return ServicesResult<IEnumerable<RoleApplicationUserInProject>>.Failure(result.Message);

            

            return ServicesResult<IEnumerable<RoleApplicationUserInProject>>.Success(result.Data, string.Empty);
        }
        #endregion

        #region Retrieve detailed information about a specific member
        /// <summary>
        /// Retrieves detailed information about a specific member by their ID.
        /// </summary>
        /// <param name="memberId">The ID of the member to retrieve.</param>
        /// <returns>
        /// A service result containing the <see cref="RoleApplicationUserInProject"/> object 
        /// or an error message if the operation fails.
        /// </returns>
        private async Task<ServicesResult<RoleApplicationUserInProject>> GetDetailMemberAsync(string memberId)
        {
            // Validate the input
            if (string.IsNullOrEmpty(memberId))
                return ServicesResult<RoleApplicationUserInProject>.Failure("Member ID is required.");

            // Attempt to fetch member details by ID
            var getMember = await _roleApplicationUserInProjectServices.GetValueByPrimaryKeyAsync(memberId);

            // Handle failure or null data
            if (getMember.Data == null || !getMember.Status)
                return ServicesResult<RoleApplicationUserInProject>.Failure(getMember.Message ?? "Failed to retrieve member details.");

            return ServicesResult<RoleApplicationUserInProject>.Success(getMember.Data, string.Empty);
        }
        #endregion
        #region 
        private async Task<ServicesResult<IEnumerable<MemberInTask>>> GetAllMemberTodo()
        {
            var member = await _memberInTaskServices.GetAllAsync();
            if(member.Status == false ) ServicesResult<IEnumerable<MemberInTask>>.Failure(member.Message);
            return ServicesResult<IEnumerable<MemberInTask>>.Success(member.Data, string.Empty);

        }
        #endregion
        private void Intialize()
        {
            var data1 = new ServicesResult<IEnumerable<RoleApplicationUserInProject>>();
            var data2 = new ServicesResult<IEnumerable<MemberInTask>>();    
            
            do
            {
                data1 = GetAllRolesAsync().GetAwaiter().GetResult();
                data2 = GetAllMemberTodo().GetAwaiter().GetResult();

            }
            while(!data1.Status || !data2.Status);
            _member = data1.Data.ToList();
            _memberInTask = data2.Data.ToList();
        }
        #endregion

        #region suport method
        #region Get Role of a Member in a Project
        /// <summary>
        /// Retrieves the role name of a specific member in a project.
        /// </summary>
        /// <param name="memberId">The ID of the member to retrieve the role for.</param>
        /// <returns>A service result containing the role name or an error message if the operation fails.</returns>
        public async Task<ServicesResult<string>> GetRoleMemberInProjectAsync(string memberId)
        {
            if (string.IsNullOrEmpty(memberId))
                return ServicesResult<string>.Failure("Member ID is required.");

            var memberResult = await _roleApplicationUserInProjectServices.GetValueByPrimaryKeyAsync(memberId);
            if (!memberResult.Status || memberResult.Data == null)
                return ServicesResult<string>.Failure(memberResult.Message ?? "Failed to retrieve member details.");

            var roleResult = await _roleInProjectServices.GetValueByPrimaryKeyAsync(memberResult.Data.RoleInProjectId);
            if (!roleResult.Status || roleResult.Data == null)
                return ServicesResult<string>.Failure(roleResult.Message ?? "Failed to retrieve role details.");

            return ServicesResult<string>.Success(roleResult.Data.RoleName, string.Empty);
        }
        #endregion

        #region Get Owner Information in a Project
        /// <summary>
        /// Retrieves detailed information about the owner of a specific project.
        /// </summary>
        /// <param name="projectId">The ID of the project to retrieve the owner for.</param>
        /// <returns>A service result containing the owner's details or an error message if the operation fails.</returns>
        public async Task<ServicesResult<IndexMember>> GetInfoOfOwnerInProjectAsync(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
                return ServicesResult<IndexMember>.Failure("Project ID is required.");

            var ownerRoleResult = await _roleLogic.GetOwnerRole();
            if (!ownerRoleResult.Status)
                return ServicesResult<IndexMember>.Failure(ownerRoleResult.Message ?? "Failed to retrieve owner role.");

            var owner = _member.FirstOrDefault(x => x.ProjectId == projectId && x.RoleInProjectId == ownerRoleResult.Data.Id);
            if (owner == null)
                return ServicesResult<IndexMember>.Success(new IndexMember(), "No owner found for this project.");

            var ownerInfo = await _userLogic.GetInfoOtherUserByUserId(owner.ApplicationUserId);
            if (!ownerInfo.Status || ownerInfo.Data == null)
                return ServicesResult<IndexMember>.Failure(ownerInfo.Message ?? "Failed to retrieve owner details.");

            var indexMember = new IndexMember
            {
                UserName = ownerInfo.Data.UserName,
                UserAvata = ownerInfo.Data.Avata,
                PositionWorkName = string.Empty,
                RoleUserInProjectId = owner.Id
            };

            return ServicesResult<IndexMember>.Success(indexMember, string.Empty);
        }
        #endregion

        #region Get Members in a Project
        /// <summary>
        /// Retrieves a list of members in a specific project.
        /// </summary>
        /// <param name="projectId">The ID of the project to retrieve members for.</param>
        /// <returns>A service result containing the list of members or an error message if the operation fails.</returns>
        public async Task<ServicesResult<IEnumerable<IndexMember>>> GetMembersInProjectAsync(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
                return ServicesResult<IEnumerable<IndexMember>>.Failure("Project ID is required.");

            var members = _member.Where(x => x.ProjectId == projectId).ToList();
            if (!members.Any())
                return ServicesResult<IEnumerable<IndexMember>>.Success(new List<IndexMember>(), "No members found in this project.");

            var result = new List<IndexMember>();

            foreach (var member in members)
            {
                var memberInfo = await _userLogic.GetInfoOtherUserByUserId(member.ApplicationUserId);
                if (!memberInfo.Status || memberInfo.Data == null)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure(memberInfo.Message ?? "Failed to retrieve member details.");

                result.Add(new IndexMember
                {
                    UserName = memberInfo.Data.UserName,
                    UserAvata = memberInfo.Data.Avata,
                    PositionWorkName = string.Empty,
                    RoleUserInProjectId = member.Id
                });
            }

            return ServicesResult<IEnumerable<IndexMember>>.Success(result, string.Empty);
        }
        #endregion

        #region Get Projects User Has Joined by User ID
        /// <summary>
        /// Retrieves a list of project IDs where a user is a member.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve projects for.</param>
        /// <returns>A service result containing the list of project IDs or an error message if the operation fails.</returns>
        public async Task<ServicesResult<IEnumerable<string>>> GetProjectsUserHasJoinedByUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return ServicesResult<IEnumerable<string>>.Failure("User ID is required.");

            var projects = _member.Where(x => x.ApplicationUserId == userId).Select(x => x.ProjectId).ToList();
            if (!projects.Any())
                return ServicesResult<IEnumerable<string>>.Success(new List<string>(), "The user has not joined any projects.");

            return ServicesResult<IEnumerable<string>>.Success(projects, string.Empty);
        }
        #endregion

        #region Get Projects User Owns
        /// <summary>
        /// Retrieves a list of project IDs where a user is the owner.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve owned projects for.</param>
        /// <returns>A service result containing the list of owned project IDs or an error message if the operation fails.</returns>
        public async Task<ServicesResult<IEnumerable<string>>> GetProjectsUserOwnsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return ServicesResult<IEnumerable<string>>.Failure("User ID is required.");

            var ownerRoleResult = await _roleLogic.GetOwnerRole();
            if (!ownerRoleResult.Status)
                return ServicesResult<IEnumerable<string>>.Failure(ownerRoleResult.Message ?? "Failed to retrieve owner role.");

            var projects = _member.Where(x => x.ApplicationUserId == userId && x.RoleInProjectId == ownerRoleResult.Data.Id)
                                  .Select(x => x.ProjectId).ToList();

            if (!projects.Any())
                return ServicesResult<IEnumerable<string>>.Success(new List<string>(), "The user does not own any projects.");

            return ServicesResult<IEnumerable<string>>.Success(projects, string.Empty);
        }
        #endregion


        #endregion
        #region primary method
        #region
        /// <summary>
        /// Retrieves a list of all members with their details, including position and user information.
        /// </summary>
        /// <returns>A service result containing the list of members or an error message if the operation fails.</returns>
        public async Task<ServicesResult<IEnumerable<IndexMember>>> GetAllAsync()
        {
            if (!_member.Any())
                return ServicesResult<IEnumerable<IndexMember>>.Success(new List<IndexMember>(), "No members found.");

            var data = new List<IndexMember>();

            foreach (var member in _member)
            {
                // Retrieve user information
                var userInfoResult = await _userLogic.GetInfoOtherUserByUserId(member.ApplicationUserId);
                if (!userInfoResult.Status || userInfoResult.Data == null)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure(userInfoResult.Message ?? "Failed to retrieve user information.");

                // Retrieve position work information
                var positionResult = await _positionLogic.GetPositionWorkByMemberId(member.Id);
                if (!positionResult.Status || positionResult.Data == null)
                    return ServicesResult<IEnumerable<IndexMember>>.Failure(positionResult.Message ?? "Failed to retrieve position work information.");

                // Create an IndexMember instance and add to the result list
                data.Add(new IndexMember
                {
                    PositionWorkName = positionResult.Data,
                    UserName = userInfoResult.Data.UserName,
                    UserAvata = userInfoResult.Data.Avata,
                    RoleUserInProjectId = member.Id
                });
            }

            return ServicesResult<IEnumerable<IndexMember>>.Success(data, string.Empty);
        }
        #endregion

        #region
        public async Task<ServicesResult<DetailMember>> GetDetailMember(string memberId)
        {
            var member = await GetDetailMemberAsync(memberId);
            if ((!member.Status)) return ServicesResult<DetailMember>.Failure(member.Message);
            var user = await _userLogic.GetInfoOtherUserByUserId(member.Data.ApplicationUserId);
            if(user.Status == false) return ServicesResult<DetailMember>.Failure(user.Message); 
            var postion = await _positionLogic.GetPositionWorkByMemberId(memberId);
            if (postion.Status == false) return ServicesResult<DetailMember>.Failure(postion.Message);
            var detail = new DetailMember()
            {
                RoleUserInProjectId = member.Data.Id,
                RoleUserNameInProject = postion.Data,
                UserAvata = user.Data.Avata,
                UserName = user.Data.UserName,
            };
            return ServicesResult<DetailMember>.Success(detail, string.Empty);
        }
        #endregion

        #endregion
    }
}
