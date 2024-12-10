using Shared.position;
using Shared;
using PM.Persistence.IServices;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.AspNetCore.Mvc;
using PM.Domain;
using System;
using PM.DomainServices.ILogic;

namespace PM.DomainServices.Logic
{
    internal class PositionLogic : IPositionLogic
    {
        #region Private Fields
        // Service to manage application user-related operations
        private readonly IApplicationUserServices _applicationUserServices;

        // Service to manage roles assigned to users within a project
        private readonly IRoleApplicationUserInProjectServices _roleApplicationUserInProjectServices;

        // Service to manage positions within projects
        private readonly IPositionInProjectServices _positionInProjectServices;

        // Service to manage work assignments tied to positions
        private readonly IPositionWorkOfMemberServices _positionWorkOfMemberServices;

        // Service to manage roles within projects
        private readonly IRoleInProjectServices _roleInProjectServices;

        // Service to manage members assigned to tasks
        private readonly IMemberInTaskServices _memberInTaskServices;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PositionLogic"/> class.
        /// Injects the necessary services for managing positions and their dependencies.
        /// </summary>
        /// <param name="applicationUserServices">Service to manage application users.</param>
        /// <param name="roleApplicationUserInProjectServices">Service to manage user roles in projects.</param>
        /// <param name="positionInProjectServices">Service to manage project positions.</param>
        /// <param name="positionWorkOfMemberServices">Service to manage work assigned to members based on positions.</param>
        /// <param name="roleInProjectServices">Service to manage roles in projects.</param>
        /// <param name="memberInTaskServices">Service to manage task assignments for members.</param>
        public PositionLogic(
            IApplicationUserServices applicationUserServices,
            IRoleApplicationUserInProjectServices roleApplicationUserInProjectServices,
            IPositionInProjectServices positionInProjectServices,
            IPositionWorkOfMemberServices positionWorkOfMemberServices,
            IRoleInProjectServices roleInProjectServices,
            IMemberInTaskServices memberInTaskServices)
        {
            _applicationUserServices = applicationUserServices
                ?? throw new ArgumentNullException(nameof(applicationUserServices));
            _roleApplicationUserInProjectServices = roleApplicationUserInProjectServices
                ?? throw new ArgumentNullException(nameof(roleApplicationUserInProjectServices));
            _positionInProjectServices = positionInProjectServices
                ?? throw new ArgumentNullException(nameof(positionInProjectServices));
            _positionWorkOfMemberServices = positionWorkOfMemberServices
                ?? throw new ArgumentNullException(nameof(positionWorkOfMemberServices));
            _roleInProjectServices = roleInProjectServices
                ?? throw new ArgumentNullException(nameof(roleInProjectServices));
            _memberInTaskServices = memberInTaskServices
                ?? throw new ArgumentNullException(nameof(memberInTaskServices));
        }
        #endregion

        #region Position Retrieval
        /// <summary>
        /// Retrieves a list of positions associated with a specific project.
        /// </summary>
        /// <param name="userId">The ID of the user requesting the data.</param>
        /// <param name="projectId">The ID of the project to fetch positions for.</param>
        /// <returns>
        /// A service result containing a list of positions if successful,
        /// or a failure result if validation or retrieval fails.
        /// </returns>
        public async Task<ServicesResult<IEnumerable<IndexPosition>>> Get(string userId, string projectId)
        {
            #region Input Validation
            // Check if the provided userId or projectId is null or empty
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
                return ServicesResult<IEnumerable<IndexPosition>>.Failure("User ID or Project ID cannot be null or empty.");
            #endregion

            #region Authorization Check
            // Verify the user exists and has access to the project
            var userExists = await _applicationUserServices.GetUser(userId);
            var userHasAccess = (await _roleApplicationUserInProjectServices.GetAllAsync())
                .Any(x => x.ProjectId == projectId && x.ApplicationUserId == userId);

            if (userExists == null || userHasAccess)
                return ServicesResult<IEnumerable<IndexPosition>>.Failure("Unauthorized access or user does not exist.");
            #endregion

            #region Data Retrieval
            // Retrieve all positions associated with the project
            var positions = (await _positionInProjectServices.GetAllAsync())
                .Where(x => x.ProjectId == projectId);

            // Check if any positions were found
            if (!positions.Any())
                return ServicesResult<IEnumerable<IndexPosition>>.Failure("No positions found for the specified project.");
            #endregion

            #region Data Transformation
            // Map positions to the IndexPosition DTO
            var data = positions.Select(position => new IndexPosition
            {
                PositionId = position.Id,
                PositionName = position.PositionName,
                PrositionDescription = position.PositionDescription
            }).ToList();
            #endregion

            // Return success result with the retrieved data
            return ServicesResult<IEnumerable<IndexPosition>>.Success(data);
        }
        #endregion
        #region Position Addition
        /// <summary>
        /// Adds a new position to the specified project.
        /// </summary>
        /// <param name="userId">The ID of the user performing the operation.</param>
        /// <param name="projectId">The ID of the project to which the position will be added.</param>
        /// <param name="position">The position details to be added.</param>
        /// <returns>
        /// A service result indicating the success or failure of the operation.
        /// </returns>
        public async Task<ServicesResult<bool>> Add(string userId, string projectId, AddPosition position)
        {
            #region Input Validation
            // Validate userId and projectId are not null or empty
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
                return ServicesResult<bool>.Failure("User ID or Project ID cannot be null or empty.");
            #endregion

            #region Role Validation
            // Retrieve the "Owner" role ID
            var roleOwner = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner")?.Id;
            if (roleOwner == null)
                return ServicesResult<bool>.Failure("Owner role not found.");
            #endregion

            #region Authorization Check
            // Check if the user exists and has the "Owner" role in the project
            var userExists = await _applicationUserServices.GetUser(userId);
            var userHasOwnerRole = (await _roleApplicationUserInProjectServices.GetAllAsync())
                .Any(x => x.ProjectId == projectId && x.ApplicationUserId == userId && x.RoleInProjectId == roleOwner);

            if (userExists == null || !userHasOwnerRole)
                return ServicesResult<bool>.Failure("Unauthorized operation or user does not exist.");
            #endregion

            #region Position Creation
            // Generate a unique ID for the new position
            var randomId = new Random().Next(1000000, 9999999);
            var positionData = new PositionInProject
            {
                Id = $"1004-{randomId}-{DateTime.Now}",
                ProjectId = projectId,
                PositionName = position.PositionName,
                PositionDescription = position.PrositionDescription,
            };

            // Add the position to the project
            var isAdded = await _positionInProjectServices.AddAsync(positionData);
            if (isAdded)
                return ServicesResult<bool>.Success(true);
            #endregion

            // Return failure if addition fails
            return ServicesResult<bool>.Failure("Failed to add the position.");
        }
        #endregion

        #region Position Deletion
        /// <summary>
        /// Deletes a position from the specified project, including any associated works and members.
        /// </summary>
        /// <param name="userId">The ID of the user performing the operation.</param>
        /// <param name="positionId">The ID of the position to be deleted.</param>
        /// <param name="projectId">The ID of the project to which the position belongs.</param>
        /// <returns>
        /// A service result indicating the success or failure of the delete operation.
        /// </returns>
        public async Task<ServicesResult<bool>> Delete(string userId, string positionId, string projectId)
        {
            #region Input Validation
            // Validate userId and positionId
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(positionId))
                return ServicesResult<bool>.Failure("User ID or Position ID cannot be null or empty.");
            #endregion

            #region Role Validation
            // Retrieve the "Owner" role ID
            var roleOwner = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner")?.Id;
            if (roleOwner == null)
                return ServicesResult<bool>.Failure("Owner role not found.");
            #endregion

            #region Authorization and Position Existence Check
            // Retrieve position details
            var position = await _positionInProjectServices.GetValueByPrimaryKeyAsync(positionId);
            if (position == null)
                return ServicesResult<bool>.Failure("Position not found.");

            // Check if the user exists and has the "Owner" role in the project
            var userExists = await _applicationUserServices.GetUser(userId);
            var userHasOwnerRole = (await _roleApplicationUserInProjectServices.GetAllAsync())
                .Any(x => x.ProjectId == position.ProjectId && x.ApplicationUserId == userId && x.RoleInProjectId == roleOwner);

            if (userExists == null || !userHasOwnerRole)
                return ServicesResult<bool>.Failure("Unauthorized operation or user does not exist.");
            #endregion

            #region Position Work and Member Cleanup
            // Retrieve works associated with the position
            var positionWorks = (await _positionWorkOfMemberServices.GetAllAsync())
                .Where(x => x.PostitionInProjectId == positionId);

            // If no works are associated, delete the position directly
            if (!positionWorks.Any())
            {
                var isDeleted = await _positionInProjectServices.DeleteAsync(positionId);
                return isDeleted ? ServicesResult<bool>.Success(true) : ServicesResult<bool>.Failure("Failed to delete the position.");
            }

            // Iterate through position works and delete associated members and works
            foreach (var work in positionWorks)
            {
                var members = (await _memberInTaskServices.GetAllAsync())
                    .Where(x => x.PositionWorkOfMemberId == work.Id);

                // Delete all members associated with the work
                foreach (var member in members)
                {
                    var memberDeleted = await _memberInTaskServices.DeleteAsync(member.Id);
                    if (!memberDeleted)
                        return ServicesResult<bool>.Failure("Failed to delete a member associated with the position.");
                }

                // Delete the work after its members are deleted
                var workDeleted = await _positionWorkOfMemberServices.DeleteAsync(work.Id);
                if (!workDeleted)
                    return ServicesResult<bool>.Failure("Failed to delete a work associated with the position.");
            }
            #endregion

            #region Final Position Deletion
            // Delete the position after cleaning up its associated works and members
            var positionDeleted = await _positionInProjectServices.DeleteAsync(positionId);
            return positionDeleted ? ServicesResult<bool>.Success(true) : ServicesResult<bool>.Failure("Failed to delete the position.");
            #endregion
        }
        #endregion
        #region Update Position
        /// <summary>
        /// Updates the details of a specific position in a project.
        /// </summary>
        /// <param name="userId">The ID of the user performing the update operation.</param>
        /// <param name="positionId">The ID of the position to update.</param>
        /// <param name="position">The updated position details.</param>
        /// <returns>
        /// A service result indicating the success or failure of the update operation.
        /// </returns>
        public async Task<ServicesResult<bool>> Update(string userId, string positionId, UpdatePositon position)
        {
            #region Input Validation
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(positionId) || position == null)
                return ServicesResult<bool>.Failure("Invalid input: User ID, Position ID, or Position details cannot be null.");
            #endregion

            #region Role Validation
            // Retrieve the "Owner" role ID
            var roleOwner = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner")?.Id;
            if (roleOwner == null)
                return ServicesResult<bool>.Failure("Owner role not found.");
            #endregion

            #region Authorization and Position Existence Check
            // Retrieve position details
            var existingPosition = await _positionInProjectServices.GetValueByPrimaryKeyAsync(positionId);
            if (existingPosition == null)
                return ServicesResult<bool>.Failure("Position not found.");

            // Check if the user exists and has the "Owner" role in the project
            var userExists = await _applicationUserServices.GetUser(userId);
            var userHasOwnerRole = (await _roleApplicationUserInProjectServices.GetAllAsync())
                .Any(x => x.ProjectId == existingPosition.ProjectId && x.ApplicationUserId == userId && x.RoleInProjectId == roleOwner);

            if (userExists == null || !userHasOwnerRole)
                return ServicesResult<bool>.Failure("Unauthorized operation or user does not exist.");
            #endregion

            #region Update Position Details
            // Update the position details
            existingPosition.PositionName = position.PositionName;
            existingPosition.PositionDescription = position.PrositionDescription;

            // Save changes
            var updateResult = await _positionInProjectServices.UpdateAsync(positionId, existingPosition);
            return updateResult ? ServicesResult<bool>.Success(true) : ServicesResult<bool>.Failure("Failed to update the position.");
            #endregion
        }
        #endregion

    }
}
