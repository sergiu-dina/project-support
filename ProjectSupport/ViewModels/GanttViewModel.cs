using ProjectSupport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.ViewModels
{
    public class GanttViewModel
    {
        public string TaskId { get; set; }
        public string Name { get; set; }

        public int StartYear { get; set; }
        public int StartMonth { get; set; }
        public int StartDay { get; set; }


        public int EndYear { get; set; }
        public int EndMonth { get; set; }
        public int EndDay { get; set; }

        public int Duration { get; set; }
        public decimal Progress { get; set; }
        public string Dependencies { get; set; }
    }
}


