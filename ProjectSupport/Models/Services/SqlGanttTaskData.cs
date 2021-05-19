using Microsoft.EntityFrameworkCore;
using ProjectSupport.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models.Services
{
    public class SqlGanttTaskData : IGanttTaskData
    {
        private readonly AppDbContext db;

        public SqlGanttTaskData(AppDbContext db)
        {
            this.db = db;
        }
        public void Add(GanttTask task)
        {
            db.GanttTasks.Add(task);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            var task = db.GanttTasks.Find(id);
            db.GanttTasks.Remove(task);
            db.SaveChanges();
        }

        public GanttTask Get(int id)
        {
            return db.GanttTasks.FirstOrDefault(t => t.Id == id);
        }

        public IEnumerable<GanttTask> GetAll()
        {
            return db.GanttTasks.OrderBy(t => t.Name);
        }

        public GanttTask GetByName(string name)
        {
            return db.GanttTasks.FirstOrDefault(t => t.Name == name);
        }

        public void Update(GanttTask project)
        {
            var entry = db.Entry(project);
            entry.State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
