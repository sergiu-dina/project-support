using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.ViewModels
{
    public class ColumnChartViewModel
    {
        public string ProjectName { get; set; }
        public int Users { get; set; }
        public int Tasks { get; set; }
        public int Dependencies { get; set; }
    }
}
