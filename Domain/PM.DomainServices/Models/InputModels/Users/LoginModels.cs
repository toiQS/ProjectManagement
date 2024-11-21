using System.ComponentModel.DataAnnotations;

namespace PM.DomainServices.Models.InputModels.Users
{
    public class LoginModel
    {
        [EmailAddress]
        public string Email {get; set;} = string.Empty;
        public string Passwrd {get; set;} = string.Empty;
    }
}