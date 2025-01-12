using PM.Domain;
using PM.DomainServices.ILogic;
using PM.DomainServices.Models;
using PM.DomainServices.Models.members;
using PM.Persistence.IServices;

namespace PM.DomainServices.Logic
{
    public class MemberLogic : IMemberLogic
    {
        //intialize services
        private readonly IRoleApplicationUserInProjectServices _roleApplicationUserInProjectServices;
        private readonly IRoleInProjectServices _roleInProjectServices;
        
        //intialize logic
        private readonly UserLogic _userLogic;
        private readonly PositionLogic _positionLogic;
        private readonly IRoleLogic _roleLogic; 
        //intialize primary value
        private List<RoleApplicationUserInProject> _member;

        #region private method
        private async Task<ServicesResult<IEnumerable<RoleApplicationUserInProject>>> GetAyncs()
        {
            var result = await _roleApplicationUserInProjectServices.GetAllAsync();
            if (result.Data == null) return ServicesResult<IEnumerable<RoleApplicationUserInProject>>.Success(new List<RoleApplicationUserInProject>(), "no data in datbase");
            if (result.Status == false) return ServicesResult<IEnumerable<RoleApplicationUserInProject>>.Failure(result.Message);
            _member = result.Data.ToList();
            return ServicesResult<IEnumerable<RoleApplicationUserInProject>>.Success(result.Data, string.Empty);
        }
        private async Task<ServicesResult<RoleApplicationUserInProject>> GetDetailMember(string memberId)
        {
            if (memberId == null) return ServicesResult<RoleApplicationUserInProject>.Failure("");
            var getMember = await _roleApplicationUserInProjectServices.GetValueByPrimaryKeyAsync(memberId);
            if (getMember.Data == null || getMember.Status == false) return ServicesResult<RoleApplicationUserInProject>.Failure(getMember.Message);
            return ServicesResult<RoleApplicationUserInProject>.Success(getMember.Data, string.Empty);
        }
        #endregion
        #region suport method
        public async Task<ServicesResult<string>> GetRoleMemberInProject(string memberId)
        {
            if (memberId == null) return ServicesResult<string>.Failure("");
            var getMember = await _roleApplicationUserInProjectServices.GetValueByPrimaryKeyAsync(memberId);
            if(getMember.Data == null || getMember.Status == false) return ServicesResult<string>.Failure(getMember.Message);
            var role = await _roleInProjectServices.GetValueByPrimaryKeyAsync(getMember.Data.RoleInProjectId);
            if(role.Data == null || role.Status == false)
                return ServicesResult<string>.Failure(role.Message);
            return ServicesResult<string>.Success(role.Data.RoleName, string.Empty);
        }
        public async Task<ServicesResult<IndexMember>> GetInfoOfOwnerInProject(string projectId)
        {
            if (string.IsNullOrEmpty(projectId)) return ServicesResult<IndexMember>.Failure("");
           
            var owner = await _roleLogic.GetOwnerRole();
            if (owner.Status == false) return ServicesResult<IndexMember>.Failure(owner.Message);
            var ownerProject = _member.FirstOrDefault(x => x.ProjectId == projectId && x.RoleInProjectId == owner.Data.Id);
            if (ownerProject == null) return ServicesResult<IndexMember>.Success(new IndexMember(), "no member is owner in this project");
            var ownerInfo = await _userLogic.GetInfoOtherUserByUserId(ownerProject.ApplicationUserId);
            if (ownerInfo.Status == false || ownerInfo.Data == null) return ServicesResult<IndexMember>.Failure(ownerInfo.Message);
            var index = new IndexMember()
            {
                UserName = ownerInfo.Data.UserName,
                UserAvata = ownerInfo.Data.Avata,
                PositionWorkName = string.Empty,
                RoleUserInProjectId = ownerProject.Id
            };
            return ServicesResult<IndexMember>.Success(index, string.Empty);
        }

        #endregion
        #region primary method
        public async Task<ServicesResult<IEnumerable<IndexMember>>> GetAll()
        {
            var data = new List<IndexMember>();
            foreach (var member in _member)
            {
                var getInfoUser = await _userLogic.GetInfoOtherUserByUserId(member.ApplicationUserId);
                if (getInfoUser.Status == false) return ServicesResult<IEnumerable<IndexMember>>.Failure(getInfoUser.Message);
                var getPostion = await _positionLogic.GetPositionWorkByMemberId(member.Id);
                if (getPostion.Status == false) return ServicesResult<IEnumerable<IndexMember>>.Failure(getInfoUser.Message);
                var index = new IndexMember()
                {
                    PositionWorkName = getPostion.Data,
                    UserName = getInfoUser.Data.UserName,
                    UserAvata = getInfoUser.Data.Avata,
                    RoleUserInProjectId = member.Id,
                };
                data.Add(index);
            }
            return ServicesResult<IEnumerable<IndexMember>>.Success(data, string.Empty);
        }
        
        
        #endregion
    }
}
