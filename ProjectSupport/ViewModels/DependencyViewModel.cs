using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.ViewModels
{
    public class DependencyViewModel
    {
        public int TaskId { get; set; }
        public int DependencyId { get; set; }
        public string TaskName { get; set; }
        public bool IsSelected { get; set; }
    }
}
