using PM.Domain.DTOs;
using PM.Persistence.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.DomainServices.Logic
{
    public class AuthLogic
    {
        private readonly ApplicationUserServices _user;
        private Dictionary<string, object> finalResult;
        public AuthLogic(ApplicationUserServices user)
        {
            _user = user;
            finalResult = new Dictionary<string, object>();
        }
        public async Task<Dictionary<string,object>> Login(string email, string password)
        {
            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                finalResult.Add("Message", "Email or password maybe empty");
            }
            var login = await _user.LoginServices(email, password);
            if (login)
            {
                finalResult.Add("Message", "");
                finalResult.Add("Status", true);
                return finalResult;
            }
            finalResult.Add("Message", "Login false");
            return finalResult;
        }
    }
}
