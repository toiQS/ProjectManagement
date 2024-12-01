using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.SqlTypes;
using PM.Domain;
using PM.DomainServices.ILogic;
using PM.DomainServices.Shared;
using PM.Persistence.IServices;
using PM.Persistence.Services;

namespace PM.DomainServices.Logic
{
    public class PositionInProjectLogic : IPositionInProjectLogic
    {
        private readonly IPositionInProjectServices _positionInProjectServices;
        private readonly IApplicationUserServices _user;
        private readonly IRoleApplicationUserInProjectServices _roleUserInProjectServices;
        private readonly IRoleInProjectServices _roleInProjectServices;
        private readonly IProjectServices _projectServices;

        public PositionInProjectLogic(
            IPositionInProjectServices positionInProjectServices, 
            IApplicationUserServices applicationUserServices, 
            IRoleApplicationUserInProjectServices roleApplicationUserInProjectServices, 
            IRoleInProjectServices roleInProjectServices, 
            IProjectServices projectServices)
        {
            _projectServices = projectServices;
            _positionInProjectServices = positionInProjectServices;
            _user = applicationUserServices;
            _roleInProjectServices = roleInProjectServices;
            _roleUserInProjectServices = roleApplicationUserInProjectServices;
        }

        #region Add Position
        /// <summary>
        /// Adds a new position in a project.
        /// </summary>
        public async Task<ServicesResult<bool>> Add(string userId, string name, string description, string projectId)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(name))
                return ServicesResult<bool>.Failure("Invalid input parameters.");

            // Check if user exists
            var findUser = await _user.GetUser(userId);
            if (findUser == null) return ServicesResult<bool>.Failure("User not found.");

            // Check user's role in the project
            var getRoleUserInProject = (await _roleUserInProjectServices.GetAllAsync())
                                        .FirstOrDefault(x => x.ApplicationUserId == userId);
            if (getRoleUserInProject == null) return ServicesResult<bool>.Failure("User has no role in project.");

            var isRoleNameInProject = (await _roleInProjectServices.GetAllAsync())
                                      .FirstOrDefault(x => x.Id == getRoleUserInProject.RoleInProjectId);
            if (isRoleNameInProject == null) return ServicesResult<bool>.Failure("Role not found.");

            // Validate position and project ownership
            if ((await _positionInProjectServices.GetAllAsync()).Any(x => x.PositionName.ToLower() == name.ToLower()))
                return ServicesResult<bool>.Failure("Position name already exists.");

            if ((await _projectServices.GetAllAsync()).All(x => x.Id != projectId) || isRoleNameInProject.RoleName != "Owner")
                return ServicesResult<bool>.Failure("Invalid project or insufficient permissions.");

            // Create a new position
            var randomSuffix = new Random().Next(100000, 999999);
            var positionInProject = new PositionInProject
            {
                Id = $"1004-{randomSuffix}-{DateTime.Now}",
                PositionName = name,
                ProjectId = projectId,
                PositionDescription = description
            };

            // Save the position
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
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(positionInProjectId) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description))
                return ServicesResult<bool>.Failure("Invalid input parameters.");

            // Check if user exists
            var findUser = await _user.GetUser(userId);
            if (findUser == null) return ServicesResult<bool>.Failure("User not found.");

            // Validate user's role and position
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

            // Update position details
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
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(positionInProjectId))
                return ServicesResult<bool>.Failure("Invalid input parameters.");

            // Check if user exists
            var findUser = await _user.GetUser(userId);
            if (findUser == null) return ServicesResult<bool>.Failure("User not found.");

            // Validate user's role and position
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

            // Delete position
            if (await _positionInProjectServices.DeleteAsync(getPosition.Id))
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
