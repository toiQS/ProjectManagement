using PM.Persistence.IServices;
using PM.Domain;
using PM.DomainServices.Shared;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace PM.DomainServices.Logic
{
    public class RoleUserInProjectLogic
    {
        private readonly IApplicationUserServices _applicationUserServices;
        private readonly IProjectServices _projectServices;
        private readonly IPositionWorkOfMemberServices _positionWorkOfMemberServices;
        private readonly IRoleApplicationUserInProjectServices _roleUserInProjectServices;
        private readonly IRoleInProjectServices _roleInProjectServices;
        private readonly IMemberInTaskServices _memberInTaskServices;

        public RoleUserInProjectLogic(
            IApplicationUserServices applicationUserServices,
            IProjectServices projectServices,
            IPositionWorkOfMemberServices positionWorkOfMemberServices,
            IRoleApplicationUserInProjectServices roleUserInProjectServices,
            IRoleInProjectServices roleInProjectServices,
            IMemberInTaskServices memberInTaskServices)
        {
            _applicationUserServices = applicationUserServices;
            _projectServices = projectServices;
            _positionWorkOfMemberServices = positionWorkOfMemberServices;
            _roleUserInProjectServices = roleUserInProjectServices;
            _roleInProjectServices = roleInProjectServices;
            _memberInTaskServices = memberInTaskServices;
        }

        #region Add
        /// <summary>
        /// Adds a new role to a user in a project.
        /// </summary>
        public async Task<ServicesResult<bool>> Add(string userId, string projectId, string applicationUserId, string roleInProjectId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(applicationUserId) || string.IsNullOrEmpty(roleInProjectId))
                return ServicesResult<bool>.Failure("Invalid input parameters.");

            var getOwnerRoleId = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner")?.Id;
            if (getOwnerRoleId == null)
                return ServicesResult<bool>.Failure("Owner role not found.");

            if (!(await _projectServices.GetAllAsync()).Any(x => x.Id == projectId))
                return ServicesResult<bool>.Failure("Project not found.");

            var userHasPermission = (await _roleUserInProjectServices.GetAllAsync())
                .Any(x => x.ApplicationUserId == userId && x.ProjectId == projectId && x.RoleInProjectId == getOwnerRoleId);
            if (!userHasPermission)
                return ServicesResult<bool>.Failure("User does not have permission.");

            if ((await _roleUserInProjectServices.GetAllAsync()).Any(x => x.ProjectId == projectId && x.ApplicationUserId == applicationUserId))
                return ServicesResult<bool>.Failure("User already has a role in this project.");

            var randomSuffix = new Random().Next(100000, 999999);
            var userRole = new RoleApplicationUserInProject
            {
                Id = $"1003-{randomSuffix}-{DateTime.Now}",
                ProjectId = projectId,
                ApplicationUserId = applicationUserId,
                RoleInProjectId = roleInProjectId,
            };

            if (await _roleUserInProjectServices.AddAsync(userRole))
                return ServicesResult<bool>.Success(true);

            return ServicesResult<bool>.Failure("Failed to add role.");
        }
        #endregion

        #region Update
        /// <summary>
        /// Updates the role of a user in a project.
        /// </summary>
        public async Task<ServicesResult<bool>> Update(string userId, string roleUserInProjectId, string applicationUserId, string roleInProjectId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(applicationUserId) || string.IsNullOrEmpty(roleInProjectId) || string.IsNullOrEmpty(roleUserInProjectId))
                return ServicesResult<bool>.Failure("Invalid input parameters.");

            var getOwnerRoleId = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner")?.Id;
            if (getOwnerRoleId == null)
                return ServicesResult<bool>.Failure("Owner role not found.");

            var userHasPermission = (await _roleUserInProjectServices.GetAllAsync())
                .Any(x => x.ApplicationUserId == userId && x.RoleInProjectId == getOwnerRoleId);
            if (!userHasPermission)
                return ServicesResult<bool>.Failure("User does not have permission.");

            var getRoleUserInProject = (await _roleUserInProjectServices.GetAllAsync()).FirstOrDefault(x => x.Id == roleUserInProjectId);
            if (getRoleUserInProject == null)
                return ServicesResult<bool>.Failure("Role not found.");

            getRoleUserInProject.ApplicationUserId = applicationUserId;
            getRoleUserInProject.RoleInProjectId = roleInProjectId;

            if (await _roleUserInProjectServices.UpdateAsync(roleUserInProjectId, getRoleUserInProject))
                return ServicesResult<bool>.Success(true);

            return ServicesResult<bool>.Failure("Failed to update role.");
        }
        #endregion

        #region Delete
        public async Task<ServicesResult<bool>> Delete(string userId, string roleUserInProjectId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleUserInProjectId))
                return ServicesResult<bool>.Failure("Invalid input parameters.");

            var user = await _applicationUserServices.GetUser(userId);
            if (user == null)
                return ServicesResult<bool>.Failure("User not found.");

            var roleUser = (await _roleUserInProjectServices.GetAllAsync()).FirstOrDefault(x => x.Id == roleUserInProjectId);
            if (roleUser == null)
                return ServicesResult<bool>.Failure("Role not found.");

            return await DeleteMethod(roleUserInProjectId);
        }

        /// <summary>
        /// Handles deletion of roles, including cascading deletions of related tasks.
        /// </summary>
        private async Task<ServicesResult<bool>> DeleteMethod(string roleUserId)
        {
            // Fetch all related position works
            var relatedPositionWorks = (await _positionWorkOfMemberServices.GetAllAsync())
                .Where(x => x.RoleApplicationUserInProjectId == roleUserId);

            foreach (var positionWork in relatedPositionWorks)
            {
                // Remove related tasks first
                var memberTasks = (await _memberInTaskServices.GetAllAsync())
                    .Where(x => x.PositionWorkOfMemberId == positionWork.Id);
                
                foreach (var task in memberTasks)
                {
                    if (!await _memberInTaskServices.DeleteAsync(task.Id))
                        return ServicesResult<bool>.Failure("Failed to delete related tasks.");
                }

                // Remove the position work
                if (!await _positionWorkOfMemberServices.DeleteAsync(positionWork.Id))
                    return ServicesResult<bool>.Failure("Failed to delete related position work.");
            }

            // Finally, remove the user role from the project
            if (await _roleUserInProjectServices.DeleteAsync(roleUserId))
                return ServicesResult<bool>.Success(true);

            return ServicesResult<bool>.Failure("Failed to delete role.");
        }
        #endregion

        #region Get Role User in Project
        public async Task<ServicesResult<IEnumerable<RoleApplicationUserInProject>>> GetRoleUserInProject(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
                return ServicesResult<IEnumerable<RoleApplicationUserInProject>>.Failure("Invalid project ID.");

            var roles = (await _roleUserInProjectServices.GetAllAsync())
                .Where(x => x.ProjectId == projectId);

            if (!roles.Any())
                return ServicesResult<IEnumerable<RoleApplicationUserInProject>>.Failure("No roles found.");

            return ServicesResult<IEnumerable<RoleApplicationUserInProject>>.Success(roles);
        }
        #endregion
    }
}
