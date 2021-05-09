using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models
{
    public class GanttTask
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime StartDate { get; set; }
        public int Duration { get; set; }
        public decimal Progress { get; set; }
        public int SortOrder { get; set; }
        public int? ParentId { get; set; }
        public string Type { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
