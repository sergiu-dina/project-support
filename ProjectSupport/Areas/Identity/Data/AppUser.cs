using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ProjectSupport.Models;
using ProjectSupport.Models.Services;

namespace ProjectSupport.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the AppUser class
    public class AppUser : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string FirstName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }

        public byte[] ProfilePicture { get; set; }

        [Display(Name = "Hourly Rate")]
        public int HourlyRate { get; set; }
        public List<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();

        public List<Resources> Resources { get; set; } = new List<Resources>();
    }
}
