using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.ViewModels
{
    public class StackedChartViewModel
    {
        public string ProjectName { get; set; }
        public int Completed { get; set; }
        public int Inprogress { get; set; }
        public int Notstarted { get; set; }
        public int Tasks { get; set; }
    }
}
