using Microsoft.EntityFrameworkCore;
using ProjectSupport.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models.Services
{
    public class SqlNotificationData : INotificationData
    {
        private readonly AppDbContext db;

        public SqlNotificationData(AppDbContext db)
        {
            this.db = db;
        }

        public void Add(Notification notification)
        {
            db.Notifications.Add(notification);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            var notification = db.Notifications.Find(id);
            db.Notifications.Remove(notification);
            db.SaveChanges();
        }

        public Notification Get(int id)
        {
            return db.Notifications.FirstOrDefault(t => t.Id == id);
        }

        public IEnumerable<Notification> GetAll()
        {
            return db.Notifications.OrderBy(t => t.Created);
        }

        public void Update(Notification notification)
        {
            var entry = db.Entry(notification);
            entry.State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
