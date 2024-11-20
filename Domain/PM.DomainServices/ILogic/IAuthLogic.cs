using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.DomainServices.ILogic
{
    public interface IAuthLogic
    {
        public Task<bool> Login(string email, string password);
        
    }
}
