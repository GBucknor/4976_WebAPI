using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GroupGradingAPI.Models
{
    public class Instructor : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public string InstructorRoleId { get; set; }
    }
}
