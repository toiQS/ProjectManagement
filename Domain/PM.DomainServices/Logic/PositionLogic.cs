using PM.Domain;
using PM.DomainServices.ILogic;
using PM.DomainServices.Models;
using PM.Persistence.IServices;

namespace PM.DomainServices.Logic
{
    public class PositionLogic : IPositionLogic
    {
        //intialize services
        private readonly IPositionInProjectServices _positionInProjectServices;
        private readonly IPositionWorkOfMemberServices _positionWorkOfMemberServices;

        //intialize logic
        //intialize primary value
        private List<PositionInProject> _positionInProjectList;
        private List<PositionWorkOfMember> _positionWorkOfMemberList;

        #region private method
        /// <summary>
        /// Retrieves all positions within a project.
        /// </summary>
        /// <returns>A service result containing a list of positions in the project or an error message.</returns>
        private async Task<ServicesResult<IEnumerable<PositionInProject>>> GetPositionsInProject()
        {
            // Fetch all positions within the project
            var positions = await _positionInProjectServices.GetAllAsync();

            // If no data is returned, return an empty list with a "No data" message
            if (positions.Data == null)
                return ServicesResult<IEnumerable<PositionInProject>>.Success(new List<PositionInProject>(), "No data");

            // If the service call failed, return the failure message
            if (positions.Status == false)
                return ServicesResult<IEnumerable<PositionInProject>>.Failure(positions.Message);

            // Store the positions in the list and return a success result with the data
            return ServicesResult<IEnumerable<PositionInProject>>.Success(_positionInProjectList, string.Empty);
        }

        /// <summary>
        /// Retrieves all position works assigned to members in the project.
        /// </summary>
        /// <returns>A service result containing a list of member position works or an error message.</returns>
        private async Task<ServicesResult<IEnumerable<PositionWorkOfMember>>> GetPositionWorksInProject()
        {
            // Fetch all positions works of members within the project
            var positions = await _positionWorkOfMemberServices.GetAllAsync();

            // If no data is returned, return an empty list with a "No data" message
            if (positions.Data == null)
                return ServicesResult<IEnumerable<PositionWorkOfMember>>.Success(new List<PositionWorkOfMember>(), "No data");

            // If the service call failed, return the failure message
            if (positions.Status == false)
                return ServicesResult<IEnumerable<PositionWorkOfMember>>.Failure(positions.Message);

            // Store the position works in the list and return a success result with the data
            return ServicesResult<IEnumerable<PositionWorkOfMember>>.Success(_positionWorkOfMemberList, string.Empty);
        }

        #endregion
        #region suport method
        #endregion
        #region primary method
        /// <summary>
        /// Retrieves the position work assigned to a member in the project.
        /// </summary>
        /// <param name="memberId">The ID of the member whose position work is being retrieved.</param>
        /// <returns>A service result containing the position name or an error message.</returns>
        public async Task<ServicesResult<string>> GetPositionWorkByMemberId(string memberId)
        {
            // Ensure memberId is not null or empty
            if (string.IsNullOrEmpty(memberId))
                return ServicesResult<string>.Failure("Member ID is required.");

            // Look for the position work assigned to the member
            var getPositionWorkOfMember = _positionWorkOfMemberList
                .Where(x => x.RoleApplicationUserInProjectId == memberId)
                .FirstOrDefault();

            // If no position work is found for the member, return an error message
            if (getPositionWorkOfMember == null)
                return ServicesResult<string>.Failure("Can't find position work for member in the database.");

            // Retrieve the position details from the PositionInProject service
            var getPosition = await _positionInProjectServices.GetValueByPrimaryKeyAsync(getPositionWorkOfMember.PostitionInProjectId);

            // If the position data is null or the retrieval failed, return the failure message
            if (getPosition.Data == null || getPosition.Status == false)
                return ServicesResult<string>.Failure(getPosition.Message);

            // Return the position name on success
            return ServicesResult<string>.Success(getPosition.Data.PositionName, string.Empty);
        }


        #endregion
    }
}
