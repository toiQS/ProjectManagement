using PM.DomainServices.Models;
using PM.DomainServices.Models.members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.DomainServices.ILogic
{
    public interface IMemberLogic
    {
        Task<ServicesResult<string>> GetRoleMemberInProject(string memberId);
        Task<ServicesResult<IndexMember>> GetInfoOfOwnerInProject(string projectId); 
        Task<ServicesResult<IEnumerable<IndexMember>>> GetAll();
        Task<ServicesResult<IEnumerable<IndexMember>>> GetMemberInProject(string projectId);    

    }
}
