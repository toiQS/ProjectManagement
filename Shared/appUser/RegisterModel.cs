﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.appUser
{
    public class RegisterModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty ;
        public string Email { get; set; } = string.Empty ;
    }
}
