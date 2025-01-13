using PM.Domain;
using PM.DomainServices.ILogic;
using PM.DomainServices.Models;
using PM.Persistence.IServices;

namespace PM.DomainServices.Logic
{
    public class RoleLogic : IRoleLogic
    {
        private readonly IRoleInProjectServices _roleInProjectServices;

        private List<RoleInProject> _roles = new List<RoleInProject>();
        
        public RoleLogic(IRoleInProjectServices roleInProjectServices)
        {
            _roleInProjectServices = roleInProjectServices;
            InitializeRoles();
        }



        #region private method
        #region GetAllRoles
        /// <summary>
        /// Retrieves all roles in the system.
        /// </summary>
        /// <returns>A service result containing the list of roles.</returns>
        private async Task<ServicesResult<List<RoleInProject>>> GetAllRoles()
        {
            // Retrieve all roles from the service
            var rolesResult = await _roleInProjectServices.GetAllAsync();

            // Check if the retrieval was successful and roles data is not null
            if (!rolesResult.Status || rolesResult.Data == null)
            {
                return ServicesResult<List<RoleInProject>>.Failure(
                    rolesResult.Message ?? "Roles not found."
                );
            }

            // Return the list of roles as a success result
            return ServicesResult<List<RoleInProject>>.Success(rolesResult.Data.ToList(), string.Empty);
        }
        #endregion

        #region InitializeRoles
        /// <summary>
        /// Initializes role data and stores it in the local context.
        /// This method will repeatedly attempt to retrieve the roles until successful.
        /// </summary>
        private void InitializeRoles()
        {
            ServicesResult<List<RoleInProject>> roles;

            // Keep trying to initialize roles until data is successfully retrieved
            do
            {
                roles = GetAllRoles().GetAwaiter().GetResult();
            }
            while (!roles.Status || roles.Data == null); // Continue until valid roles data is retrieved

            // Store the successfully retrieved roles in the local _roles context
            _roles = roles.Data.ToList();
        }
        #endregion

        #endregion
        #region suport method
        #region GetOwnerRole
        /// <summary>
        /// Retrieves the "Owner" role from the list of roles.
        /// </summary>
        /// <returns>A service result containing the "Owner" role if found, or a failure result if not found.</returns>
        public async Task<ServicesResult<RoleInProject>> GetOwnerRole()
        {
            // Find the "Owner" role from the list of roles
            var ownerRole = _roles.FirstOrDefault(x => x.RoleName == "Owner");

            // Return a failure result if the "Owner" role is not found
            if (ownerRole == null)
                return ServicesResult<RoleInProject>.Failure("Cannot retrieve the owner role.");

            // Return the found "Owner" role as a success result
            return ServicesResult<RoleInProject>.Success(ownerRole, string.Empty);
        }
        #endregion

        #region GetLeaderRole
        /// <summary>
        /// Retrieves the "Leader" role from the list of roles.
        /// </summary>
        /// <returns>A service result containing the "Leader" role if found, or a failure result if not found.</returns>
        public async Task<ServicesResult<RoleInProject>> GetLeaderRole()
        {
            // Find the "Leader" role from the list of roles
            var leaderRole = _roles.FirstOrDefault(x => x.RoleName == "Leader");

            // Return a failure result if the "Leader" role is not found
            if (leaderRole == null)
                return ServicesResult<RoleInProject>.Failure("Cannot retrieve the leader role.");

            // Return the found "Leader" role as a success result
            return ServicesResult<RoleInProject>.Success(leaderRole, string.Empty);
        }
        #endregion

        #region GetManagerRole
        /// <summary>
        /// Retrieves the "Manager" role from the list of roles.
        /// </summary>
        /// <returns>A service result containing the "Manager" role if found, or a failure result if not found.</returns>
        public async Task<ServicesResult<RoleInProject>> GetManagerRole()
        {
            // Find the "Manager" role from the list of roles
            var managerRole = _roles.FirstOrDefault(x => x.RoleName == "Manager");

            // Return a failure result if the "Manager" role is not found
            if (managerRole == null)
                return ServicesResult<RoleInProject>.Failure("Cannot retrieve the manager role.");

            // Return the found "Manager" role as a success result
            return ServicesResult<RoleInProject>.Success(managerRole, string.Empty);
        }
        #endregion

        #region GetInfoRole
        /// <summary>
        /// Retrieves detailed information about a role by its ID.
        /// </summary>
        /// <param name="roleId">The unique identifier of the role to retrieve.</param>
        /// <returns>A service result containing the role information or a failure result if the role is not found.</returns>
        public async Task<ServicesResult<RoleInProject>> GetInfoRole(string roleId)
        {
            // Return failure if the roleId is null or empty
            if (string.IsNullOrEmpty(roleId))
                return ServicesResult<RoleInProject>.Failure("Role ID cannot be null or empty.");

            // Retrieve role information using the role ID
            var infoRole = await _roleInProjectServices.GetValueByPrimaryKeyAsync(roleId);

            // Check for failure in retrieving role data
            if (infoRole.Status == false)
                return ServicesResult<RoleInProject>.Failure(infoRole.Message);

            // Return success with an empty role if not found
            if (infoRole.Data == null)
                return ServicesResult<RoleInProject>.Success(new RoleInProject(), $"Can't find this role with ID {roleId}");

            // Return the retrieved role as a success result
            return ServicesResult<RoleInProject>.Success(infoRole.Data, string.Empty);
        }
        #endregion

        #endregion
        #region primary method
        #endregion

    }
}
