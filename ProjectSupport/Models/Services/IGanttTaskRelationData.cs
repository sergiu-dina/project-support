using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models.Services
{
    public interface IGanttTaskRelationData
    {
        GanttTaskRelation Get(int taskId, int taskRelationId);
        IEnumerable<GanttTaskRelation> GetAll();
        void Add(GanttTaskRelation project);
        void Delete(int taskId, int taskRelationId);
        bool HasRelation(int taskId, int taskRelationId);
    }
}
