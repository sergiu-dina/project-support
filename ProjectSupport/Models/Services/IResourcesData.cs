using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models.Services
{
    public interface IResourcesData
    {
        Resources Get(int taskId, string userId);
        IEnumerable<Resources> GetAll();
        void Add(Resources project);
        void Delete(string userId, int taskId);
        bool HasUser(int taskId, string userId);
    }
}
