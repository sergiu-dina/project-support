using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models
{
    public class GanttTaskRelation
    {
        public int GanttTaskId { get; set; }
        public virtual GanttTask GanttTask { get; set; }
        public int RelatedTaskId { get; set; }
        public virtual GanttTask RelatedGanttTask { get; set; }
    }
}
