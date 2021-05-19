using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectSupport.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.ViewModels
{
    public class TaskProjectViewModel
    {
        public GanttTask Task { get; set; }
        public List<SelectListItem> Tasks { get; set; }

        [Display(Name ="Dependency")]
        public string Selected { get; set; }
        public int ProjectId { get; set; }
    }
}
