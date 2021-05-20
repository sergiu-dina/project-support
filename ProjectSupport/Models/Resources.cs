using ProjectSupport.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models
{
    public class Resources
    {
        public int TaskId { get; set; }
        public GanttTask Task { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}
