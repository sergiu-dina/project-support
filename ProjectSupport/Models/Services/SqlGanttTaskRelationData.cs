using ProjectSupport.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models.Services
{
    public class SqlGanttTaskRelationData : IGanttTaskRelationData
    {
        private readonly AppDbContext db;

        public SqlGanttTaskRelationData(AppDbContext db)
        {
            this.db = db;
        }

        public void Add(GanttTaskRelation ganttTaskRelation)
        {
            db.GanttTaskRelations.Add(ganttTaskRelation);
            db.SaveChanges();
        }

        public void Delete(int taskId, int taskRelationId)
        {
            var ganttTaskRelation = db.GanttTaskRelations.Find(taskId, taskRelationId);
            db.GanttTaskRelations.Remove(ganttTaskRelation);
        }

        public GanttTaskRelation Get(int taskId, int taskRelationId)
        {
            var ganttTaskRelation = db.GanttTaskRelations.FirstOrDefault(gtr => gtr.GanttTaskId == taskId && gtr.RelatedTaskId == taskRelationId);
            return ganttTaskRelation;
        }

        public IEnumerable<GanttTaskRelation> GetAll()
        {
            return db.GanttTaskRelations;
        }

        public bool HasRelation(int taskId, int taskRelationId)
        {
            var result = false;
            foreach (var relation in db.GanttTaskRelations)
            {
                if (relation.GanttTaskId == taskId && relation.RelatedTaskId == taskRelationId)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
}
