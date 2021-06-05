using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models.Services
{
    public interface IChatData
    {
        IEnumerable<Chat> GetAll();
        Chat Get(int id);
        Chat GetByName(string name);
        void Add(Chat chat);
        void Update(Chat chat);
        void Delete(int id);
    }
}
