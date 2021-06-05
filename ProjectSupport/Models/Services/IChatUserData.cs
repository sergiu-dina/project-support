using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models.Services
{
    public interface IChatUserData
    {
        ChatUser Get(int chatId, string userId);
        IEnumerable<ChatUser> GetAll();
        void Add(ChatUser chatUser);
        void Delete(string userId, int chatId);
        bool HasUser(int chatId, string userId);
    }
}
