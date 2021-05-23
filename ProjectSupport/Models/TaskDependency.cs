using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models
{
    public class TaskDependency
    {
        public int TaskId { get; set; }
        public GanttTask Task { get; set; }
        public int DependencyId { get; set; }
        public Dependency Dependency { get; set; }
    }
}
