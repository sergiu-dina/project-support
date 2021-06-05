using ProjectSupport.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models.Services
{
    public class SqlChatUserData : IChatUserData
    {
        private readonly AppDbContext db;

        public SqlChatUserData(AppDbContext db)
        {
            this.db = db;
        }

        public void Add(ChatUser chatUser)
        {
            db.ChatUsers.Add(chatUser);
            db.SaveChanges();
        }

        public void Delete(string userId, int chatId)
        {
            var chatUser = db.ChatUsers.Find(userId, chatId);
            db.ChatUsers.Remove(chatUser);
        }

        public ChatUser Get(int chatId, string userId)
        {
            var chatUser = db.ChatUsers.FirstOrDefault(p => p.ChatId == chatId && p.UserId == userId);
            return chatUser;
        }

        public IEnumerable<ChatUser> GetAll()
        {
            return db.ChatUsers;
        }

        public bool HasUser(int chatId, string userId)
        {
            var result = false;
            foreach (var chatUser in db.ChatUsers)
            {
                if (chatUser.ChatId == chatId && chatUser.UserId == userId)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
}
