using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PagedList;
using ProjectSupport.Areas.Identity.Data;
using ProjectSupport.Data;
using ProjectSupport.Models;
using ProjectSupport.Models.Services;
using ProjectSupport.SignalR;
using ProjectSupport.SignalR.Services;
using ProjectSupport.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AppUser> userManager;
        private readonly AppDbContext db;
        private readonly IProjectData projectData;
        private readonly IProjectUserData projectUserData;
        private readonly IGanttTaskData ganttTaskData;
        private readonly IResourcesData resourcesData;
        private readonly IUserData userData;
        private readonly INotificationData notificationData;
        private readonly IUserConnectionManager userConnectionManager;
        private readonly IHubContext<NotificationsHub> notificationsHubContext;
        private readonly IChatData chatData;
        private readonly IChatUserData chatUserData;

        public ManagerController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, AppDbContext db,
            IProjectData projectData, IProjectUserData projectUserData, IGanttTaskData ganttTaskData, IResourcesData resourcesData,
            IUserData userData, INotificationData notificationData, IUserConnectionManager userConnectionManager,
            IHubContext<NotificationsHub> notificationsHubContext, IChatData chatData, IChatUserData chatUserData)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.db = db;
            this.projectData = projectData;
            this.projectUserData = projectUserData;
            this.ganttTaskData = ganttTaskData;
            this.resourcesData = resourcesData;
            this.userData = userData;
            this.notificationData = notificationData;
            this.userConnectionManager = userConnectionManager;
            this.notificationsHubContext = notificationsHubContext;
            this.chatData = chatData;
            this.chatUserData = chatUserData;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> EditProject(int id, string user)
        {
            var manager = false;
            var managerRole = await roleManager.FindByNameAsync("Manager");
            var appUser = await userManager.FindByIdAsync(user);

            var project = projectData.Get(id);
            var projectUsers = projectUserData.GetAll();

            foreach (var projectUser in projectUsers)
            {
                if (user == projectUser.UserId && project.Id == projectUser.ProjectId)
                {
                    if (await userManager.IsInRoleAsync(appUser, role: managerRole.Name))
                    {
                        manager = true;
                    }
                }
            }


            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            else if (manager == false)
            {
                ViewBag.ErrorMessage = $"This User is not the manager of this project";
                return View("NotFound");
            }

            var model = new EditProjectViewModel();
            model.Project = projectData.Get(id);
            model.UserId = user;
            if (model == null)
            {
                return View("NotFound");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProject(EditProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                projectData.Update(model.Project);
                return RedirectToAction("SeeProjects", new { id = model.UserId });
            }
            return View();
        }

        public async Task<IActionResult> SeeProjects(string id, string SearchText="", int pg = 1)
        {
            var model = new List<ProjectIndexViewModel>();

            var managerRole = await roleManager.FindByNameAsync("Manager");

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var manager = await userManager.IsInRoleAsync(user, managerRole.Name);

            foreach (var project in projectData.GetAll())
            {
                var managerProject = new ProjectIndexViewModel();
                if (projectUserData.HasUser(project.Id, user.Id))
                {
                    managerProject.IsManager = true;
                }
                else
                {
                    managerProject.IsManager = false;
                }
                managerProject.Project = project;
                model.Add(managerProject);
            }


            if (SearchText != "" && SearchText != null)
            {
                model = model.OrderByDescending(p => p.IsManager).ThenBy(p => p.Project.Name).Where(m => m.Project.Name.Contains(SearchText)).ToList();
            }
            else
            {
                model = model.OrderByDescending(p => p.IsManager).ThenBy(p => p.Project.Name).ToList();
            }
            const int pageSize = 5;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = model.Count();

            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data = model.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> AddDevelopers(string id ,string sortOrder, int pg = 1, string SearchText = "")
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var model = new List<ProjectIndexViewModel>();

            var managerRole = await roleManager.FindByNameAsync("Manager");

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var manager = await userManager.IsInRoleAsync(user, managerRole.Name);

            foreach (var project in projectData.GetAll())
            {
                var managerProject = new ProjectIndexViewModel();
                if (projectUserData.HasUser(project.Id, user.Id))
                {
                    managerProject.IsManager = true;
                }
                else
                {
                    managerProject.IsManager = false;
                }
                managerProject.Project = project;
                model.Add(managerProject);
            }

            if (SearchText != "" && SearchText != null)
            {
                model = model.OrderByDescending(p => p.IsManager).ThenBy(p => p.Project.Name).Where(m => m.Project.Name.Contains(SearchText)).ToList();
            }
            else
            {
                model = model.OrderByDescending(p => p.IsManager).ThenBy(p => p.Project.Name).ToList();
            }

            const int pageSize = 5;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = model.Count();

            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data = model.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            switch (sortOrder)
            {
                case "name_desc":
                    data = data.OrderByDescending(s => s.IsManager).ThenBy(s => s.Project.Name).ToList();
                    break;
                default:
                    data = data.OrderByDescending(s => s.IsManager).ThenBy(s => s.Project.Name).ToList();
                    break;
            }

            return View(data);
        }

        
        [HttpGet]
        public async Task<IActionResult> SelectDevelopers(int id, string user, int pg = 1, string SearchText = "")
        {
            var project = projectData.Get(id);
            var projectUser = projectUserData.GetAll();
            if (project == null)
            {
                ViewBag.ErrorMessage = $"Role with Id= {id} cannot be found";
                return View("NotFound");
            }

            var manager = false;
            var managerRole = await roleManager.FindByNameAsync("Manager");
            var tempUser = await userManager.FindByIdAsync(user);

            var projectUsers = projectUserData.GetAll();

            foreach (var item in projectUsers)
            {
                if (user == item.UserId && project.Id == item.ProjectId)
                {
                    if (await userManager.IsInRoleAsync(tempUser, role: managerRole.Name))
                    {
                        manager = true;
                    }
                }
            }


            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            else if (manager == false)
            {
                ViewBag.ErrorMessage = $"This User is not the manager of this project";
                return View("NotFound");
            }

            var developerRole = await roleManager.FindByNameAsync("Developer");
            var users = new List<AppUser>();
            foreach (var appUser in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(appUser, role: developerRole.Name))
                {
                    users.Add(appUser);
                }
            }

            const int pageSize = 5;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = users.Count();

            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data = users.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            var model = new List<UserProjectViewModel>();
            foreach (var appUser in data)
            {
                var userProjectViewModel = new UserProjectViewModel
                {
                    ProjectId = project.Id,
                    UserId = appUser.Id,
                    UserName = appUser.UserName,
                    User = user
                };
                foreach (var existingUser in projectUser)
                    if (appUser.Id == existingUser.UserId && project.Id == existingUser.ProjectId)
                    {
                        userProjectViewModel.IsSelected = true;
                        break;
                    }
                    else
                    {
                        userProjectViewModel.IsSelected = false;
                    }
                model.Add(userProjectViewModel);
            }

            if (SearchText != "" && SearchText != null)
            {
                model = model.OrderBy(p => p.UserName).Where(m => m.UserName.Contains(SearchText)).ToList();
            }
            else
            {
                model = model.OrderBy(p => p.UserName).ToList();
            }

            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> SelectDevelopers(List<UserProjectViewModel> model)
        {
            var project = projectData.Get(model[0].ProjectId);
            if (project == null)
            {
                ViewBag.ErrorMessage = $"Role with Id= {model[0].ProjectId} cannot be found";
                return View("NotFound");
            }
            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                if (model[i].IsSelected && !(projectUserData.HasUser(project.Id, user.Id)))
                {
                    var temp = new ProjectUser();
                    temp.ProjectId = project.Id;
                    temp.UserId = model[i].UserId;
                    projectUserData.Add(temp);

                    var chat = chatData.GetByName(project.Name);
                    var chatTemp = new ChatUser
                    {
                        UserId = model[i].UserId,
                        ChatId = chat.Id
                    };
                    chatUserData.Add(chatTemp);

                    var notification = new Notification
                    {
                        Description = $"You have been added as a developer to the '{project.Name}' project",
                        Created = DateTime.Now,
                        IsRead = false,
                        UserId = temp.UserId,
                        IsSuccess = true
                    };

                    await SendNotification(notification);

                    notificationData.Add(notification);

                    db.SaveChanges();
                }
                else if (!model[i].IsSelected && projectUserData.HasUser(project.Id, user.Id))
                {
                    projectUserData.Delete(user.Id, project.Id);

                    var chat = chatData.GetByName(project.Name);
                    chatUserData.Delete(model[i].UserId, chat.Id);

                    var tasks = ganttTaskData.GetAll();
                    var projectTasks = new List<GanttTask>();
                    foreach(var task in tasks)
                    {
                        if(task.ProjectId == project.Id)
                        {
                            projectTasks.Add(task);
                        }
                    }
                    foreach (var task in projectTasks)
                    {
                        if (resourcesData.HasUser(task.Id, user.Id))
                        {
                            resourcesData.Delete(user.Id, task.Id);
                        }
                    }

                    var notification = new Notification
                    {
                        Description = $"You have been removed from the '{project.Name}' project",
                        Created = DateTime.Now,
                        IsRead = false,
                        UserId = user.Id,
                        IsSuccess = false
                    };

                    await SendNotification(notification);

                    notificationData.Add(notification);

                    db.SaveChanges();
                }
                else
                {
                    continue;
                }

                if (i < (model.Count - 1))
                    continue;
                else
                    return RedirectToAction("AddDevelopers", new { id = model[0].User });

            }
            return RedirectToAction("AddDevelopers", new { id = model[0].User });
        }
        
        [HttpGet]
        public async Task<IActionResult> SeeDevelopers(string id, string sortOrder, int pg = 1, string SearchText = "")
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.EmailSortParm = String.IsNullOrEmpty(sortOrder) ? "email_desc" : "";
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var developerRole = await roleManager.FindByNameAsync("Developer");
            var users = new List<AppUser>();
            foreach (var appUser in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(appUser, role: developerRole.Name))
                {
                    users.Add(appUser);
                }
            }

            if (SearchText != "" && SearchText != null)
            {
                users = users.OrderBy(p => p.UserName).Where(m => m.UserName.Contains(SearchText)).ToList();
            }
            else
            {
                users = users.OrderBy(p => p.UserName).ToList();
            }

            const int pageSize = 5;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = users.Count();

            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data = users.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            switch (sortOrder)
            {
                case "email_desc":
                    data = data.OrderByDescending(u => u.Email).ToList();
                    break;
                case "name_desc":
                    data = data.OrderByDescending(s => s.LastName).ToList();
                    break;
                default:
                    data = data.OrderBy(u => u.LastName).ToList();
                    break;
            }

            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> AddSalary(string id)
        {
            var appUser = await userManager.FindByIdAsync(id);

            var model = new EditUserViewModel();
            model.AppUser = appUser;
            model.UserId = id;
            if (model == null)
            {
                return View("NotFound");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSalary(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var notification = new Notification
                {
                    Description = $"Your hourly rate has been updated to {model.AppUser.HourlyRate} $",
                    Created = DateTime.Now,
                    IsRead = false,
                    UserId = model.UserId,
                    IsSuccess = true
                };

                await SendNotification(notification);

                notificationData.Add(notification);

                db.SaveChanges();

                userData.Update(model.AppUser);
                return RedirectToAction("SeeDevelopers", new { id = model.UserId });
            }
            return View();
        }

        private async Task<bool> SendNotification(Notification notification)
        {
            var connections = userConnectionManager.GetUserConnections(notification.UserId);
            if (connections != null && connections.Count > 0)
            {
                foreach (var connectionId in connections)
                {
                    await notificationsHubContext.Clients.Client(connectionId).SendAsync("sendToUser", notification.Description, notification.Created);
                }
                return true;
            }
            return false;
        }

    }
}
