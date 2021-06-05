using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models.Services
{
    public interface IMessageData
    {
        IEnumerable<Message> GetAll();
        Message Get(int id);
        void Add(Message message);
        void Update(Message message);
        void Delete(int id);
    }
}
