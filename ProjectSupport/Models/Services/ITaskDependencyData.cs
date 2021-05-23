using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models.Services
{
    public interface ITaskDependencyData
    {
        TaskDependency Get(int taskId, int dependencyId);
        IEnumerable<TaskDependency> GetAll();
        void Add(TaskDependency dependency);
        void Delete(int dependencyId, int taskId);
    }
}
