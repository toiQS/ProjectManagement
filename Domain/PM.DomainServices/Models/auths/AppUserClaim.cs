using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.DomainServices.Models.auths
{
    public class AppUserClaim
    {
        public string Email { get; set; }  = string.Empty;
        public string RoleUser { get; set; } = string.Empty;
    }
}
