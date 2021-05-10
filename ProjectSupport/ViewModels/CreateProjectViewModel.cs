using ProjectSupport.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.ViewModels
{
    public class CreateProjectViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string ProjectName { get; set; }
        public IEnumerable<Project> Projects { get; set; }
    }
}
