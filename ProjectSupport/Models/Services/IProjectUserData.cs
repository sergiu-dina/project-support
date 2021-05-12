using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models.Services
{
    public interface IProjectUserData
    {
        ProjectUser Get(int projectId, string userId);
        IEnumerable<ProjectUser> GetAll();
        void Add(ProjectUser project);
        void Delete(string userId, int projectId);
        bool HasUser(int projectId, string userId);
    }
}
