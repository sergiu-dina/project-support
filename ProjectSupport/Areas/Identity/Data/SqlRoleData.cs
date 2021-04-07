using Microsoft.AspNetCore.Identity;
using ProjectSupport.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Areas.Identity.Data
{
    public class SqlRoleData : IRoleData
    {
        private readonly AppDbContext db;

        public SqlRoleData(AppDbContext db)
        {
            this.db = db;
        }
        public void Delete(string id)
        {
            var role = db.Roles.Find(id);
            db.Roles.Remove(role);
            db.SaveChanges();
        }

        public IdentityRole Get(string id)
        {
            var role = db.Roles.FirstOrDefault(r => r.Id == id);
            return role;
        }

        public IEnumerable<IdentityRole> GetAll()
        {
            return db.Roles;
        }
    }
}
