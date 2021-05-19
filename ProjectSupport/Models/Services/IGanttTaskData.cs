using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models.Services
{
    public interface IGanttTaskData
    {
        IEnumerable<GanttTask> GetAll();
        GanttTask Get(int id);
        GanttTask GetByName(string name);
        void Add(GanttTask task);
        void Update(GanttTask task);
        void Delete(int id);
    }
}
