﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models
{
    public class GanttTask
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        public int Duration { get; set; }
        public int Progress { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public List<Resources> Resources { get; set; } = new List<Resources>();
        public virtual List<GanttTaskRelation> GanttTaskRelations { get; set; } = new List<GanttTaskRelation>();
        public virtual List<GanttTaskRelation> GanttTaskRelationsOf { get; set; } = new List<GanttTaskRelation>();

    }
}
