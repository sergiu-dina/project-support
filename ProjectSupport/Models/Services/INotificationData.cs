using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models.Services
{
    public interface INotificationData
    {
        IEnumerable<Notification> GetAll();
        Notification Get(int id);
        void Add(Notification task);
        void Update(Notification task);
        Task Delete(int id);
    }
}
