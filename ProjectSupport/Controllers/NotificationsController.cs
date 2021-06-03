using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ProjectSupport.Areas.Identity.Data;
using ProjectSupport.Data;
using ProjectSupport.Models;
using ProjectSupport.Models.Services;
using ProjectSupport.SignalR;
using ProjectSupport.SignalR.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly INotificationData notificationData;
        private readonly IUserConnectionManager userConnectionManager;
        private readonly IHubContext<NotificationsHub> notificationsHubContext;
        private readonly AppDbContext db;

        public NotificationsController(UserManager<AppUser> userManager, INotificationData notificationData, IUserConnectionManager userConnectionManager,
            IHubContext<NotificationsHub> notificationsHubContext, AppDbContext db)
        {
            this.userManager = userManager;
            this.notificationData = notificationData;
            this.userConnectionManager = userConnectionManager;
            this.notificationsHubContext = notificationsHubContext;
            this.db = db;
        }

        public IActionResult Index(string id)
        {
            var model = new List<Notification>();
            var notifications = notificationData.GetAll();

            foreach(var notification in notifications)
            {
                if(notification.UserId == id && notification.IsRead == false)
                {
                    notification.IsRead = true;
                    model.Add(notification);
                }
            }

            db.SaveChanges();

            return View(model.OrderByDescending(n=>n.Created).ToList());
        }

        [HttpGet("/getNotifications/{userId}")]
        public IEnumerable<Notification> GetNotifications(string userId)
        {
            var notifications = notificationData.GetAll();
            return notifications.Where(n => n.UserId == userId && n.IsRead == false);
        }
    }
}
