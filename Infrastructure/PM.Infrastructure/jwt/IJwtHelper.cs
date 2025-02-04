using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Infrastructure.jwt
{
    public interface IJwtHelper 
    {
        public string GenerateTokenString(string email, string role);
    }
}
