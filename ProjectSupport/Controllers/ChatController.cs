using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ProjectSupport.Areas.Identity.Data;
using ProjectSupport.Data;
using ProjectSupport.Models;
using ProjectSupport.Models.Services;
using ProjectSupport.SignalR;
using ProjectSupport.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProjectSupport.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IChatData chatData;
        private readonly IChatUserData chatUserData;
        private readonly IMessageData messageData;
        private readonly AppDbContext db;
        private readonly IProjectData projectData;
        private readonly UserManager<AppUser> userManager;
        private readonly IHubContext<ChatHub> chatHubContext;

        public ChatController(IChatData chatData, IChatUserData chatUserData, IMessageData messageData, AppDbContext db,
            IProjectData projectData, UserManager<AppUser> userManager, IHubContext<ChatHub> chatHubContext )
        {
            this.chatData = chatData;
            this.chatUserData = chatUserData;
            this.messageData = messageData;
            this.db = db;
            this.projectData = projectData;
            this.userManager = userManager;
            this.chatHubContext = chatHubContext;
        }

        public IActionResult Index(int id)
        {
            var project = projectData.Get(id);
            var chat = chatData.GetByName(project.Name);
            var model = db.Chats.Include(c => c.Messages).FirstOrDefault(c => c.Id == chat.Id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int chatId,string userId, string mess)
        {
            var user = await userManager.FindByIdAsync(userId); 

            var message = new Message
            {
                ChatId=chatId,
                Text=mess,
                Name=user.Email,
                Timestamp=DateTime.Now
            };

            messageData.Add(message);
            await db.SaveChangesAsync();

            var chat = chatData.Get(chatId);
            var project = projectData.GetByName(chat.Name);

            return RedirectToAction("Index", new { id = project.Id });
        }

        public async Task<IActionResult> SendMessage(string mess, int chatId, string roomName)
        {
            var message = new Message
            {
                ChatId = chatId,
                Text = mess,
                Name = User.Identity.Name,
                Timestamp = DateTime.Now
            };

            messageData.Add(message);
            await db.SaveChangesAsync();

            await chatHubContext.Clients.Group(roomName).SendAsync("RecieveMessage", message);
            return Ok();
        }

        [HttpGet]
        public IActionResult SeeUsers(string id, string SearchText = "", int pg = 1)
        {
            var users = db.Users.Where(u => u.Id != id).OrderBy(u => u.UserName).ToList();
            var chatUsers = chatUserData.GetAll();
            var chats = chatData.GetAll();
            var model = new List<ChatUsersViewModel>();
            var existingUsers = new List<AppUser>();

            foreach(var user in users)
            {
                foreach(var chat in chats)
                {
                    if(chat.Type == ChatType.Private)
                    {
                        if(chatUserData.HasUser(chat.Id, user.Id) && chatUserData.HasUser(chat.Id, id))
                        {
                            var temp = new ChatUsersViewModel();
                            temp.User = user;
                            temp.existingChat = true;
                            temp.chatId = chat.Id;
                            model.Add(temp);
                            existingUsers.Add(user);
                        }
                    }
                }
            }

            var otherUsers = users.Where(u => existingUsers.All(e => e.Id != u.Id));
            foreach(var user in otherUsers)
            {
                var temp = new ChatUsersViewModel();
                temp.User = user;
                temp.existingChat = false;
                model.Add(temp);
            }

            if (SearchText != "" && SearchText != null)
            {
                model = model.OrderByDescending(s => s.existingChat).ThenBy(p => p.User.UserName).Where(m => m.User.UserName.Contains(SearchText)).ToList();
            }
            else
            {
                model = model.OrderByDescending(s => s.existingChat).ThenBy(p => p.User.UserName).ToList();
            }

            const int pageSize = 5;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = model.Count();

            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data = model.Skip(recSkip).Take(pager.PageSize).OrderByDescending(m => m.existingChat).ThenBy(m => m.User.UserName).ToList();

            this.ViewBag.Pager = pager;

            return View(data);
        }

        public async Task<IActionResult> CreatePrivateRoom(string userId)
        {
            var chat = new Chat
            {
                Type = ChatType.Private
            };

            chat.ChatUsers.Add(new ChatUser
            {
                UserId = userId
            });

            chat.ChatUsers.Add(new ChatUser
            {
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value
            });

            chatData.Add(chat);
            await db.SaveChangesAsync();

            return RedirectToAction("PrivateChat", new { id = chat.Id });
        }

        public IActionResult PrivateChat(int id)
        {
            var chat = chatData.Get(id);
            var model = db.Chats.Include(c => c.Messages).FirstOrDefault(c => c.Id == chat.Id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePrivateMessage(int chatId, string userId, string mess)
        {
            var user = await userManager.FindByIdAsync(userId);

            var message = new Message
            {
                ChatId = chatId,
                Text = mess,
                Name = user.Email,
                Timestamp = DateTime.Now
            };

            messageData.Add(message);
            await db.SaveChangesAsync();

            var chat = chatData.Get(chatId);

            return RedirectToAction("PrivateChat", new { id = chatId });
        }

        public async Task<IActionResult> SendPrivateMessage(string mess, int chatId, int roomName)
        {
            var message = new Message
            {
                ChatId = chatId,
                Text = mess,
                Name = User.Identity.Name,
                Timestamp = DateTime.Now
            };

            messageData.Add(message);
            await db.SaveChangesAsync();

            var room = roomName.ToString();
            await chatHubContext.Clients.Group(room).SendAsync("RecieveMessage", message);
            return Ok();
        }
    }
}
