using Microsoft.AspNetCore.Mvc;
using ProjectSupport.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Controllers
{
    public class ChatController : Controller
    {
        private readonly IChatData chatData;
        private readonly IChatUserData chatUserData;
        private readonly IMessageData messageData;

        public ChatController(IChatData chatData, IChatUserData chatUserData, IMessageData messageData)
        {
            this.chatData = chatData;
            this.chatUserData = chatUserData;
            this.messageData = messageData;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
