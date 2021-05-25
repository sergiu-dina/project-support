using ProjectSupport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.ViewModels
{
    public class ManageResourcesViewModel
    {
        public GanttTask Task { get; set; }
        public int TaskCost { get; set; }
    }
}
