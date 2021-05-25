using ProjectSupport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.ViewModels
{
    public class AssignedTaskViewModel
    {
        public GanttTask Task { get; set; }
        public bool IsAssigned { get; set; }
    }
}
