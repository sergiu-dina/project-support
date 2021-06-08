using ProjectSupport.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.ViewModels
{
    public class ChatUsersViewModel
    {
        public AppUser User { get; set; }
        public bool existingChat { get; set; }
        public int chatId { get; set; }
    }
}
