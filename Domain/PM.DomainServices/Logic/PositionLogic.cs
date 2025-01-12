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
        private async Task<ServicesResult<IEnumerable<PositionInProject>>> GetPositionsInProject()
        {
            var positions= await _positionInProjectServices.GetAllAsync();
            if (positions.Data == null) return ServicesResult<IEnumerable<PositionInProject>>.Success(new List<PositionInProject>(), "No data");
            if(positions.Status == false) return ServicesResult<IEnumerable<PositionInProject>>.Failure(positions.Message);
            _positionInProjectList = positions.Data.ToList();
            return ServicesResult<IEnumerable<PositionInProject>>.Success(_positionInProjectList, string.Empty);


        }
        private async Task<ServicesResult<IEnumerable<PositionWorkOfMember>>> GetPositionWorksInProject()
        {
            var positions = await _positionWorkOfMemberServices.GetAllAsync();
            if (positions.Data == null) return ServicesResult<IEnumerable<PositionWorkOfMember>>.Success(new List<PositionWorkOfMember>(), "No data");
            if (positions.Status == false) return ServicesResult<IEnumerable<PositionWorkOfMember>>.Failure(positions.Message);
            _positionWorkOfMemberList = positions.Data.ToList();
            return ServicesResult<IEnumerable<PositionWorkOfMember>>.Success(_positionWorkOfMemberList, string.Empty);
        }
        #endregion
        #region suport method
        #endregion
        #region primary method
        public async Task<ServicesResult<string>> GetPositionWorkByMemberId(string memberId)
        {
            if (memberId == null) return ServicesResult<string>.Failure("");
            var getPositionWorkOfMember = _positionWorkOfMemberList.Where(x => x.RoleApplicationUserInProjectId == memberId).FirstOrDefault();
            if (getPositionWorkOfMember == null) return ServicesResult<string>.Failure("can't find postion work of member in database");
            var getPosition = await _positionInProjectServices.GetValueByPrimaryKeyAsync(getPositionWorkOfMember.PostitionInProjectId);
            if(getPosition.Data == null || getPosition.Status == false ) return ServicesResult<string>.Failure(getPosition.Message);
            return ServicesResult<string>.Success(getPosition.Data.PositionName, string.Empty);
        }

        #endregion
    }
}
