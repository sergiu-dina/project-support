using ProjectSupport.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models
{
    public class ChatUser
    {
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public int ChatId { get; set; }
        public Chat Chat { get; set; }
    }
}
