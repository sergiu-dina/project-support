using Microsoft.EntityFrameworkCore;
using ProjectSupport.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models.Services
{
    public class SqlMessageData : IMessageData
    {
        private readonly AppDbContext db;

        public SqlMessageData(AppDbContext db)
        {
            this.db = db;
        }
        public void Add(Message message)
        {
            db.Messages.Add(message);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            var message = db.Messages.Find(id);
            db.Messages.Remove(message);
            db.SaveChanges();
        }

        public Message Get(int id)
        {
            return db.Messages.FirstOrDefault(m => m.Id == id);
        }

        public IEnumerable<Message> GetAll()
        {
            return db.Messages.OrderBy(m => m.Name);
        }

        public void Update(Message message)
        {
            var entry = db.Entry(message);
            entry.State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
