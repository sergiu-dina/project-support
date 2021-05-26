using ProjectSupport.Areas.Identity.Data;
using ProjectSupport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.ViewModels
{
    public class TaskDetailsViewModel
    {
        public GanttTask Task { get; set; }
        public List<AppUser> Users { get; set; }
        public List<GanttTask> Dependencies { get; set; }
        public int TaskCost { get; set; }
    }
}
