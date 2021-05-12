﻿using Microsoft.EntityFrameworkCore;
using ProjectSupport.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models.Services
{
    public class SqlProjectData : IProjectData
    {
        private readonly AppDbContext db;

        public SqlProjectData(AppDbContext db)
        {
            this.db = db;
        }

        public void Add(Project project)
        {
            db.Projects.Add(project);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            var project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
        }

        public Project Get(int id)
        {
            return db.Projects.FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Project> GetAll()
        {
            return db.Projects.OrderBy(p => p.Name);
        }

        public Project GetByName(string name)
        {
            return db.Projects.FirstOrDefault(p => p.Name == name);
        }
        
        public void Update(Project project)
        {
            var entry = db.Entry(project);
            entry.State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
