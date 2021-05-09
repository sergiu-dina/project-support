using ProjectSupport.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }
        public List<GanttTask> Tickets { get; set; } = new List<GanttTask>();
        public List<AppUser> Users { get; set; } = new List<AppUser>();
    }
}
