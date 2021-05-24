using ProjectSupport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.ViewModels
{
    public class EditTaskViewModel
    {
        public GanttTask Task { get; set; }
        public string UserId { get; set; }
    }
}
