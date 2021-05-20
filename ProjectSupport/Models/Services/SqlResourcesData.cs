using ProjectSupport.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models.Services
{
    public class SqlResourcesData : IResourcesData
    {
        private readonly AppDbContext db;

        public SqlResourcesData(AppDbContext db)
        {
            this.db = db;
        }

        public void Add(Resources resource)
        {
            db.Resources.Add(resource);
            db.SaveChanges();
        }

        public void Delete(string userId, int taskId)
        {
            var resource = db.Resources.Find(userId, taskId);
            db.Resources.Remove(resource);
        }

        public Resources Get(int taskId, string userId)
        {
            var resource = db.Resources.FirstOrDefault(p => p.TaskId == taskId && p.UserId == userId);
            return resource;
        }

        public IEnumerable<Resources> GetAll()
        {
            return db.Resources;
        }

        public bool HasUser(int taskId, string userId)
        {
            var result = false;
            foreach (var resource in db.Resources)
            {
                if (resource.TaskId == taskId && resource.UserId == userId)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
}
