using System.ComponentModel.DataAnnotations.Schema;
using PM.Domain;
using PM.DomainServices.ILogic;
using PM.DomainServices.Shared;
using PM.Persistence.IServices;

namespace PM.DomainServices.Logic
{
    public class PositionInProjectLogic : IPositionInProjectLogic
    {
        private readonly IPositionInProjectServices _positionInProjectServices;
        private readonly IApplicationUserServices _user;
        private readonly IRoleApplicationUserInProjectServices _roleUserInProjectServices;
        private readonly IRoleInProjectServices _roleInProjectServices;
        private readonly IProjectServices _projectServices;
        private readonly IPositionWorkOfMemberServices _positionWorkOfMemberServices;
        private readonly IMemberInTaskServices _memberInTaskServices;

        public PositionInProjectLogic(
            IPositionInProjectServices positionInProjectServices,
            IApplicationUserServices applicationUserServices,
            IRoleApplicationUserInProjectServices roleApplicationUserInProjectServices,
            IRoleInProjectServices roleInProjectServices,
            IProjectServices projectServices,
            IPositionWorkOfMemberServices positionWorkOfMemberServices,
            IMemberInTaskServices memberInTaskServices)
        {
            _positionInProjectServices = positionInProjectServices;
            _user = applicationUserServices;
            _roleUserInProjectServices = roleApplicationUserInProjectServices;
            _roleInProjectServices = roleInProjectServices;
            _projectServices = projectServices;
            _positionWorkOfMemberServices = positionWorkOfMemberServices;
            _memberInTaskServices = memberInTaskServices;
        }

        #region Add Position
        /// <summary>
        /// Adds a new position in a project.
        /// </summary>
        public async Task<ServicesResult<bool>> Add(string userId, string name, string description, string projectId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(name))
                return ServicesResult<bool>.Failure("Invalid input parameters.");

            var findUser = await _user.GetUser(userId);
            if (findUser == null) return ServicesResult<bool>.Failure("User not found.");

            var getRoleUserInProject = (await _roleUserInProjectServices.GetAllAsync())
                                        .FirstOrDefault(x => x.ApplicationUserId == userId);
            if (getRoleUserInProject == null) return ServicesResult<bool>.Failure("User has no role in project.");

            var isRoleNameInProject = (await _roleInProjectServices.GetAllAsync())
                                      .FirstOrDefault(x => x.Id == getRoleUserInProject.RoleInProjectId);
            if (isRoleNameInProject == null) return ServicesResult<bool>.Failure("Role not found.");

            if ((await _positionInProjectServices.GetAllAsync()).Any(x => x.PositionName.ToLower() == name.ToLower()))
                return ServicesResult<bool>.Failure("Position name already exists.");

            if ((await _projectServices.GetAllAsync()).All(x => x.Id != projectId) || isRoleNameInProject.RoleName != "Owner")
                return ServicesResult<bool>.Failure("Invalid project or insufficient permissions.");

            var randomSuffix = new Random().Next(100000, 999999);
            var positionInProject = new PositionInProject
            {
                Id = $"1004-{randomSuffix}-{DateTime.Now}",
                PositionName = name,
                ProjectId = projectId,
                PositionDescription = description
            };

            if (await _positionInProjectServices.AddAsync(positionInProject))
                return ServicesResult<bool>.Success(true);

            return ServicesResult<bool>.Failure("Failed to add position.");
        }
        #endregion

        #region Update Position
        /// <summary>
        /// Updates an existing position in a project.
        /// </summary>
        public async Task<ServicesResult<bool>> Update(string userId, string positionInProjectId, string name, string description)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(positionInProjectId) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description))
                return ServicesResult<bool>.Failure("Invalid input parameters.");

            var findUser = await _user.GetUser(userId);
            if (findUser == null) return ServicesResult<bool>.Failure("User not found.");

            var getRoleUserInProject = (await _roleUserInProjectServices.GetAllAsync())
                                        .FirstOrDefault(x => x.ApplicationUserId == userId);
            if (getRoleUserInProject == null) return ServicesResult<bool>.Failure("User has no role in project.");

            var isRoleNameInProject = (await _roleInProjectServices.GetAllAsync())
                                      .FirstOrDefault(x => x.Id == getRoleUserInProject.RoleInProjectId);
            if (isRoleNameInProject == null) return ServicesResult<bool>.Failure("Role not found.");

            var getPosition = (await _positionInProjectServices.GetAllAsync())
                              .FirstOrDefault(x => x.Id == positionInProjectId);
            if (getPosition == null || isRoleNameInProject.RoleName != "Owner")
                return ServicesResult<bool>.Failure("Position not found or insufficient permissions.");

            getPosition.PositionName = name;
            getPosition.PositionDescription = description;

            if (await _positionInProjectServices.UpdateAsync(positionInProjectId, getPosition))
                return ServicesResult<bool>.Success(true);

            return ServicesResult<bool>.Failure("Failed to update position.");
        }
        #endregion

        #region Delete Position
        /// <summary>
        /// Deletes a position from a project.
        /// </summary>
        public async Task<ServicesResult<bool>> Delete(string userId, string positionInProjectId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(positionInProjectId))
                return ServicesResult<bool>.Failure("Invalid input parameters.");

            var findUser = await _user.GetUser(userId);
            if (findUser == null) return ServicesResult<bool>.Failure("User not found.");

            var getRoleUserInProject = (await _roleUserInProjectServices.GetAllAsync())
                                        .FirstOrDefault(x => x.ApplicationUserId == userId);
            if (getRoleUserInProject == null) return ServicesResult<bool>.Failure("User has no role in project.");

            var isRoleNameInProject = (await _roleInProjectServices.GetAllAsync())
                                      .FirstOrDefault(x => x.Id == getRoleUserInProject.RoleInProjectId);
            if (isRoleNameInProject == null) return ServicesResult<bool>.Failure("Role not found.");

            var getPosition = (await _positionInProjectServices.GetAllAsync())
                              .FirstOrDefault(x => x.Id == positionInProjectId);
            if (getPosition == null || isRoleNameInProject.RoleName != "Owner")
                return ServicesResult<bool>.Failure("Position not found or insufficient permissions.");

            return await DeletePositionWithDependencies(positionInProjectId);
        }
        #endregion

        #region Helper Methods for Delete
        private async Task<ServicesResult<bool>> DeletePositionWithDependencies(string positionId)
        {
            var positionWorks = (await _positionWorkOfMemberServices.GetAllAsync()).Where(x => x.PostitionInProjectId == positionId);

            foreach (var positionWork in positionWorks)
            {
                var memberTasks = (await _memberInTaskServices.GetAllAsync()).Where(x => x.PositionWorkOfMemberId == positionWork.Id);

                foreach (var memberTask in memberTasks)
                {
                    if (!await _memberInTaskServices.DeleteAsync(memberTask.Id))
                        return ServicesResult<bool>.Failure("Failed to delete associated member tasks.");
                }

                if (!await _positionWorkOfMemberServices.DeleteAsync(positionWork.Id))
                    return ServicesResult<bool>.Failure("Failed to delete associated position work.");
            }

            if (await _positionInProjectServices.DeleteAsync(positionId))
                return ServicesResult<bool>.Success(true);

            return ServicesResult<bool>.Failure("Failed to delete position.");
        }
        #endregion

        #region Get Positions by Project ID
        /// <summary>
        /// Retrieves positions associated with a specific project.
        /// </summary>
        public async Task<ServicesResult<IEnumerable<PositionInProject>>> GetPositionsInProjectByProjectId(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
                return ServicesResult<IEnumerable<PositionInProject>>.Failure("Invalid project ID.");

            var data = (await _positionInProjectServices.GetAllAsync())
                       .Where(x => x.ProjectId == projectId);

            if (!data.Any()) return ServicesResult<IEnumerable<PositionInProject>>.Failure("No positions found.");

            return ServicesResult<IEnumerable<PositionInProject>>.Success(data);
        }
        #endregion

        #region Get Position by ID
        /// <summary>
        /// Retrieves a specific position by its ID.
        /// </summary>
        public async Task<ServicesResult<PositionInProject>> GetPositionInProjectByPositionId(string positionId)
        {
            if (string.IsNullOrEmpty(positionId))
                return ServicesResult<PositionInProject>.Failure("Invalid position ID.");

            var data = (await _positionInProjectServices.GetAllAsync())
                       .FirstOrDefault(x => x.Id == positionId);

            if (data == null) return ServicesResult<PositionInProject>.Failure("Position not found.");

            return ServicesResult<PositionInProject>.Success(data);
        }
        #endregion
    }
}
