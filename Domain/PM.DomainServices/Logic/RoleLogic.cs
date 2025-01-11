using PM.Domain;
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

        #region
        /// <summary>
        /// Retrieves all roles in the system.
        /// </summary>
        /// <returns>A service result containing the list of roles.</returns>
        private async Task<ServicesResult<List<RoleInProject>>> GetAllRoles()
        {
            var rolesResult = await _roleInProjectServices.GetAllAsync();
            if (!rolesResult.Status || rolesResult.Data == null)
                return ServicesResult<List<RoleInProject>>.Failure(rolesResult.Message ?? "Roles not found.");

            return ServicesResult<List<RoleInProject>>.Success(rolesResult.Data.ToList(), string.Empty);
        }
        #endregion
        #region
        /// <summary>
        /// Initializes role data and stores it in the local context.
        /// </summary>
        private void InitializeRoles()
        {
            ServicesResult<List<RoleInProject>> roles;
            do
            {
                roles = GetAllRoles().GetAwaiter().GetResult();
            }
            while (!roles.Status || roles.Data == null);

            _roles = roles.Data.ToList();
        }
        #endregion
        #region
        /// <summary>
        /// Retrieves the "Owner" role.
        /// </summary>
        /// <returns>A service result containing the owner role.</returns>
        public async Task<ServicesResult<RoleInProject>> GetOwnerRole()
        {
            var ownerRole = _roles.FirstOrDefault(x => x.RoleName == "Owner");
            if (ownerRole == null)
                return ServicesResult<RoleInProject>.Failure("Cannot retrieve the owner role.");

            return ServicesResult<RoleInProject>.Success(ownerRole, string.Empty);
        }
        #endregion
        #region
        /// <summary>
        /// Retrieves the "Leader" role.
        /// </summary>
        /// <returns>A service result containing the leader role.</returns>
        public async Task<ServicesResult<RoleInProject>> GetLeaderRole()
        {
            var leaderRole = _roles.FirstOrDefault(x => x.RoleName == "Leader");
            if (leaderRole == null)
                return ServicesResult<RoleInProject>.Failure("Cannot retrieve the leader role.");

            return ServicesResult<RoleInProject>.Success(leaderRole, string.Empty);
        }
        #endregion
        #region
        /// <summary>
        /// Retrieves the "Manager" role.
        /// </summary>
        /// <returns>A service result containing the manager role.</returns>
        public async Task<ServicesResult<RoleInProject>> GetManagerRole()
        {
            var managerRole = _roles.FirstOrDefault(x => x.RoleName == "Manager");
            if (managerRole == null)
                return ServicesResult<RoleInProject>.Failure("Cannot retrieve the manager role.");

            
            return ServicesResult<RoleInProject>.Success(managerRole, string.Empty);
        }
        #endregion
        #region
        public async Task<ServicesResult<RoleInProject>> GetInfoRole(string roleId)
        {
            if (string.IsNullOrEmpty(roleId)) return ServicesResult<RoleInProject>.Failure("");
            var infoRole = await _roleInProjectServices.GetValueByPrimaryKeyAsync(roleId);
            if (infoRole.Status == false) return ServicesResult<RoleInProject>.Failure(infoRole.Message);
            if (infoRole.Data == null) return ServicesResult<RoleInProject>.Success(new RoleInProject(), $"can't find this role in data {roleId}");
            return ServicesResult<RoleInProject>.Success(infoRole.Data, string.Empty);
        }
        #endregion
    }
}
