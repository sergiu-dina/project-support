using Microsoft.EntityFrameworkCore;
using ProjectSupport.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models.Services
{
    public class SqlDependencyData : IDependencyData
    {
        private readonly AppDbContext db;

        public SqlDependencyData(AppDbContext db)
        {
            this.db = db;
        }

        public void Add(Dependency dependency)
        {
            db.Dependencies.Add(dependency);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            var dependency = db.Dependencies.Find(id);
            db.Dependencies.Remove(dependency);
            db.SaveChanges();
        }

        public Dependency Get(int id)
        {
            return db.Dependencies.FirstOrDefault(d => d.Id == id);
        }

        public IEnumerable<Dependency> GetAll()
        {
            return db.Dependencies.OrderBy(p => p.TaskId);
        }

        public void Update(Dependency dependency)
        {
            var entry = db.Entry(dependency);
            entry.State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
