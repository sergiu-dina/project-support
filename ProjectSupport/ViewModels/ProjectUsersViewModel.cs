using ProjectSupport.Areas.Identity.Data;
using ProjectSupport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.ViewModels
{
    public class ProjectUsersViewModel
    {
        public Project Project { get; set; }
        public AppUser Manager { get; set; }
        public List<AppUser> Developers { get; set; }
        public List<GanttTask> Tasks { get; set; }
        public int DevelopersCount { get; set; }
    }
}
