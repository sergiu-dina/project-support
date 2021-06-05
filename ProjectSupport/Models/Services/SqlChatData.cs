using Microsoft.EntityFrameworkCore;
using ProjectSupport.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models.Services
{
    public class SqlChatData : IChatData
    {
        private readonly AppDbContext db;

        public SqlChatData(AppDbContext db)
        {
            this.db = db;
        }

        public void Add(Chat chat)
        {
            db.Chats.Add(chat);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            var chat = db.Chats.Find(id);
            db.Chats.Remove(chat);
            db.SaveChanges();
        }

        public Chat Get(int id)
        {
            return db.Chats.FirstOrDefault(c => c.Id == id);
        }

        public Chat GetByName(string name)
        {
            return db.Chats.FirstOrDefault(c => c.Name == name);
        }

        public IEnumerable<Chat> GetAll()
        {
            return db.Chats.OrderBy(c => c.Name);
        }

        public void Update(Chat chat)
        {
            var entry = db.Entry(chat);
            entry.State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
