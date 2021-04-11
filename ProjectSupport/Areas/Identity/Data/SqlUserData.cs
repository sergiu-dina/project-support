using ProjectSupport.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Areas.Identity.Data
{
    public class SqlUserData : IUserData
    {
        private readonly AppDbContext db;

        public SqlUserData(AppDbContext db)
        {
            this.db = db;
        }

        public void Delete(string id)
        {
            var user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
        }

        public AppUser Get(string email)
        {
            return db.Users.FirstOrDefault(e => e.Email == email);
        }

        public IEnumerable<AppUser> GetAll()
        {
            return db.Users.OrderBy(e => e.Email);
        }
    }
}
