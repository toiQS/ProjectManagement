﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM.Domain
{
    [Table(name: "Application User")]
    public class ApplicationUser : IdentityUser
    {
        [Column(name: "First Name")]
        public string FirstName { get; set; } = string.Empty;
        [Column(name: "Last Name")]
        public string LastName { get; set; } = string.Empty;
        [Column(name: "Full Name")]
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        [Column(name: "Path Image")]
        public string PathImage { get; set; } = string.Empty;
    }
}
