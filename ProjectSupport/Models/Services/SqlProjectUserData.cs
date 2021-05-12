using ProjectSupport.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models.Services
{
    public class SqlProjectUserData : IProjectUserData
    {
        private readonly AppDbContext db;

        public SqlProjectUserData(AppDbContext db)
        {
            this.db = db;
        }

        public void Add(ProjectUser projectUser)
        {
            db.ProjectUsers.Add(projectUser);
            db.SaveChanges();
        }

        public void Delete(string userId, int projectId)
        {
            var projectUser = db.ProjectUsers.Find(userId, projectId);
            db.ProjectUsers.Remove(projectUser);
            db.SaveChanges();
        }

        public ProjectUser Get(int projectId, string userId)
        {
            var projectUser = db.ProjectUsers.FirstOrDefault(p => p.ProjectId == projectId && p.UserId == userId);
            return projectUser;
        }

        public IEnumerable<ProjectUser> GetAll()
        {
            return db.ProjectUsers;
        }

        public bool HasUser(int projectId, string userId)
        {
            var result = false;
            foreach(var projectUser in db.ProjectUsers)
            {
                if(projectUser.ProjectId == projectId && projectUser.UserId ==  userId)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
}
