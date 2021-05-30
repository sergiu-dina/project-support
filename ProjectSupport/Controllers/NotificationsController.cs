using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ProjectSupport.Areas.Identity.Data;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly INotificationData _notificationData;
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly IHubContext<NotificationsHub> _notificationsHubContext;

        public NotificationsController(UserManager<AppUser> userManager, INotificationData notificationData, IUserConnectionManager userConnectionManager,
            IHubContext<NotificationsHub> notificationsHubContext)
        {
            this._userManager = userManager;
            this._notificationData = notificationData;
            this._userConnectionManager = userConnectionManager;
            this._notificationsHubContext = notificationsHubContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/getNotifications/{userId}")]
        public IEnumerable<Notification> GetNotifications(string userId)
        {
            var notifications = _notificationData.GetAll();
            return notifications.Where(n => n.UserId == userId && n.IsRead == false);
        }
    }
}
