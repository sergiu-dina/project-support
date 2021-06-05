using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ChatType Type { get; set; }
        public List<Message> Messages { get; set; } = new List<Message>();
        public List<ChatUser> ChatUsers { get; set; } = new List<ChatUser>();
    }
}
