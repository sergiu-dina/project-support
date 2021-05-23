using ProjectSupport.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models.Services
{
    public class SqlTaskDependencyData : ITaskDependencyData
    {
        private readonly AppDbContext db;

        public SqlTaskDependencyData(AppDbContext db)
        {
            this.db = db;
        }

        public void Add(TaskDependency taskDependency)
        {
            db.TaskDependencies.Add(taskDependency);
            db.SaveChanges();
        }

        public void Delete(int dependencyId, int taskId)
        {
            var taskDependency = db.TaskDependencies.Find(dependencyId, taskId);
            db.TaskDependencies.Remove(taskDependency);
        }

        public TaskDependency Get(int dependencyId, int taskId)
        {
            var taskDependency = db.TaskDependencies.FirstOrDefault(p => p.DependencyId == dependencyId && p.TaskId == taskId);
            return taskDependency;
        }

        public IEnumerable<TaskDependency> GetAll()
        {
            return db.TaskDependencies;
        }
    }
}
