﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSupport.Areas.Identity.Data;
using ProjectSupport.Data;
using ProjectSupport.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PagedList;
using Microsoft.AspNetCore.Authorization;
using ProjectSupport.Models;
using ProjectSupport.Models.Services;
using ProjectSupport.SignalR.Services;
using Microsoft.AspNetCore.SignalR;
using ProjectSupport.SignalR;

namespace ProjectSupport.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AppUser> userManager;
        private readonly IGanttTaskData ganttTaskData;
        private readonly AppDbContext db;
        private readonly IProjectData projectData;
        private readonly IProjectUserData projectUserData;
        private readonly IResourcesData resourcesData;
        private readonly IGanttTaskRelationData ganttTaskRelationData;
        private readonly INotificationData notificationData;
        private readonly IUserConnectionManager userConnectionManager;
        private readonly IHubContext<NotificationsHub> notificationsHubContext;
        private readonly IChatData chatData;
        private readonly IChatUserData chatUserData;

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, IGanttTaskData ganttTaskData,
            AppDbContext db, IProjectData projectData, IProjectUserData projectUserData, IResourcesData resourcesData,
            IGanttTaskRelationData ganttTaskRelationData, INotificationData notificationData, IUserConnectionManager userConnectionManager,
            IHubContext<NotificationsHub> notificationsHubContext, IChatData chatData, IChatUserData chatUserData)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.ganttTaskData = ganttTaskData;
            this.db = db;
            this.projectData = projectData;
            this.projectUserData = projectUserData;
            this.resourcesData = resourcesData;
            this.ganttTaskRelationData = ganttTaskRelationData;
            this.notificationData = notificationData;
            this.userConnectionManager = userConnectionManager;
            this.notificationsHubContext = notificationsHubContext;
            this.chatData = chatData;
            this.chatUserData = chatUserData;
        }
        public async Task<IActionResult> Index()
        {
            var cost = 0;
            var projects = projectData.GetAll();
            var tasks = ganttTaskData.GetAll();
            var resources = resourcesData.GetAll();

            foreach (var project in projects)
            {
                foreach (var task in tasks)
                {
                    if (task.ProjectId == project.Id)
                    {
                        foreach (var resource in resources)
                        {
                            if (resource.TaskId == task.Id)
                            {
                                var user = await userManager.FindByIdAsync(resource.UserId);
                                cost += user.HourlyRate * task.Duration * 8;
                            }
                        }
                    }
                }
            }
            return View(cost);
        }

        [HttpGet]
        public JsonResult GetDonutJsonData()
        {
            var model = new List<DonutChartViewModel>();

            var tasks = ganttTaskData.GetAll();
            var completed = 0;
            var inprogress = 0;
            var notstarted = 0;

            foreach (var task in tasks)
            {
                if (task.Progress == 0)
                {
                    notstarted++;
                }
                else if(task.Progress == 100)
                {
                    completed++;
                }
                else
                {
                    inprogress++;
                }
            }

            var temp1 = new DonutChartViewModel
            {
                ProjectName = "Completed",
                Duration = completed
            };
            var temp2 = new DonutChartViewModel
            {
                ProjectName = "In Progress",
                Duration = inprogress
            };
            var temp3 = new DonutChartViewModel
            {
                ProjectName = "Not Started",
                Duration = notstarted
            };

            model.Add(temp1);
            model.Add(temp2);
            model.Add(temp3);

            return Json(model);
        }

        [HttpGet]
        public JsonResult GetDonut2JsonData()
        {
            var model = new List<DonutChartViewModel>();
            var projects = projectData.GetAll();
            var tasks = ganttTaskData.GetAll();
            var resources = resourcesData.GetAll();

            foreach (var project in projects)
            {
                var temp = new DonutChartViewModel();
                temp.ProjectName = project.Name;
                var time = 0;
                foreach (var task in tasks)
                {
                    if (task.ProjectId == project.Id)
                    {
                        var duration = (task.EndDate - task.StartDate).Days * 8;
                        int percentComplete = (int)Math.Round((double)(task.Progress/100) * duration);
                        time += percentComplete;
                    }
                }
                temp.Duration = time;

                model.Add(temp);
            }

            var data = model.OrderByDescending(m => m.Duration).Take(6).ToList();

            return Json(data);
        }

        [HttpGet]
        public JsonResult GetColumnJsonData()
        {
            var model = new List<StackedChartViewModel>();
            var projects = projectData.GetAll();
            var tasks = ganttTaskData.GetAll();

            foreach (var project in projects)
            {
                var temp = new StackedChartViewModel();
                temp.ProjectName = project.Name;
                foreach (var task in tasks)
                {
                    if (task.ProjectId == project.Id)
                    {
                        if (task.Progress == 0)
                        {
                            temp.Notstarted++;
                        }
                        else if (task.Progress == 100)
                        {
                            temp.Completed++;
                        }
                        else
                        {
                            temp.Inprogress++;
                        }
                        temp.Tasks++;
                    }
                }
                model.Add(temp);
            }

            var data = model.OrderByDescending(m => m.Tasks).Take(10).ToList();

            return Json(data);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            var model = new CreateRoleViewModel();
            model.Roles= roleManager.Roles.OrderBy(r => r.Name);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };
                IdentityResult result = await roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("CreateRole", "Admin");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            model.Roles= roleManager.Roles.OrderBy(r => r.Name);
            return View(model);
        }

        [HttpGet]
        public IActionResult DeleteRole(string id)
        {
            var model = new DeleteRoleViewModel();
            model.Role =roleManager.Roles.FirstOrDefault(r=>r.Id==id);

            if (model == null)
            {
                return View("NotFound");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(IdentityRole role)
        {
            var ro = roleManager.Roles.FirstOrDefault(r => r.Id == role.Id);
            var result = await roleManager.DeleteAsync(ro);

            if (result.Succeeded)
            {
                return RedirectToAction("CreateRole");
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View("Index");
        }

        public async Task<ViewResult> EditUsers(string sortOrder, string searchString, int pg, string SearchText = "")
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.EmailSortParm = String.IsNullOrEmpty(sortOrder) ? "email_desc" : "";
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var query = from u in db.Users
                         select u;

            List<AppUser> users; 

            if (SearchText != "" && SearchText != null)
            {
                users = query.OrderBy(p => p.UserName).Where(m => m.UserName.Contains(SearchText)).ToList();
            }
            else
            {
                users = query.OrderBy(p => p.UserName).ToList();
            }

            switch (sortOrder)
            {
                case "email_desc":
                    users = users.OrderByDescending(u => u.Email).ToList();
                    break;
                case "name_desc":
                    users = users.OrderByDescending(s => s.LastName).ToList();
                    break;
                default:
                    users = users.OrderBy(u => u.LastName).ToList();
                    break;
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

            if (!String.IsNullOrEmpty(searchString))
            {
                data = data.Where(s => s.LastName.Contains(searchString)
                                       || s.Email.Contains(searchString)).ToList();
            }

            var userRolesViewModel = new List<UserRolesViewModel>();
            foreach(AppUser user in data)
            {
                var temp = new UserRolesViewModel();
                temp.UserId = user.Id;
                temp.FirstName = user.FirstName;
                temp.LastName = user.LastName;
                temp.Email = user.Email;
                temp.Roles = await GetUserRoles(user);
                userRolesViewModel.Add(temp);
            }

            return View(userRolesViewModel.ToList());
        }

        private async Task<List<string>> GetUserRoles(AppUser user)
        {
            return new List<string>(await userManager.GetRolesAsync(user));
        }

        [HttpGet]
        public async Task<IActionResult> AddRole (string userId, int pg)
        {
            ViewBag.userId = userId;
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }
            ViewBag.UserName = user.UserName;

            var existingRole = false;

            var model = new List<AddRoleViewModel>();
            foreach (var role in roleManager.Roles)
            {
                var userRolesViewModel = new AddRoleViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.Selected = true;
                    existingRole = true;
                }
                else
                {
                    userRolesViewModel.Selected = false;
                }
                model.Add(userRolesViewModel);
            }

            ViewBag.ExistingRole = existingRole;
            ViewBag.Page = pg;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(List<AddRoleViewModel> model, string userId, int pg)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View();
            }

            var removed = false;
            var roles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }
            else
            {
                var role = model.Where(x => x.Selected).Select(y => y.RoleName).FirstOrDefault();


                if (roles.Count > 0)
                {
                    if (roles[0] != role)
                    {
                        var projects = projectData.GetAll();
                        var projectUsers = projectUserData.GetAll();
                        foreach (var project in projects)
                        {
                            if (projectUserData.HasUser(project.Id, user.Id))
                            {
                                projectUserData.Delete(user.Id, project.Id);
                            }
                        }

                        var chatUsers = chatUserData.GetAll();
                        foreach(var chatUser in chatUsers)
                        {
                            if(chatUser.UserId == user.Id)
                            {
                                chatUserData.Delete(chatUser.UserId, chatUser.ChatId);
                            }
                        }

                        var tasks = ganttTaskData.GetAll();
                        var resources = resourcesData.GetAll();
                        foreach(var task in tasks)
                        {
                            if(resourcesData.HasUser(task.Id, user.Id))
                            {
                                resourcesData.Delete(user.Id, task.Id);
                            }
                        }

                        var notification = new Notification
                        {
                            Description = $"You have been removed from the role of {roles[0]}",
                            Created = DateTime.Now,
                            IsRead = false,
                            UserId = user.Id,
                            IsSuccess = false
                        };

                        await SendNotification(notification);

                        notificationData.Add(notification);

                        db.SaveChanges();

                        removed = true;
                    }
                }
            }
            result = await userManager.AddToRolesAsync(user, model.Where(x => x.Selected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }
            else
            {
                if (removed == false)
                {
                    var notification = new Notification
                    {
                        Description = $"You have been added in the role of {model.Where(x => x.Selected).Select(y => y.RoleName).FirstOrDefault()}",
                        Created = DateTime.Now,
                        IsRead = false,
                        UserId = user.Id,
                        IsSuccess = true
                    };

                    await SendNotification(notification);

                    notificationData.Add(notification);

                    db.SaveChanges();
                }
            }

            db.SaveChanges();
            return RedirectToAction("EditUsers", new { pg = pg});
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var model = await userManager.FindByIdAsync(id);
            if(model==null)
            {
                return View("NotFound");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(AppUser user)
        {
            var model = await userManager.FindByIdAsync(user.Id);
            var notifications = notificationData.GetAll();
            foreach(var notification in notifications)
            {
                if(notification.UserId == model.Id)
                {
                    await notificationData.Delete(notification.Id);
                }
            }
            await db.SaveChangesAsync();
            var result = await userManager.DeleteAsync(model);
            if(result.Succeeded)
            {
                var projects = projectData.GetAll();
                var projectUsers = projectUserData.GetAll();
                foreach (var project in projects)
                {
                    if (projectUserData.HasUser(project.Id, user.Id))
                    {
                        projectUserData.Delete(user.Id, project.Id);
                    }
                }

                var tasks = ganttTaskData.GetAll();
                var resources = resourcesData.GetAll();
                foreach (var task in tasks)
                {
                    if (resourcesData.HasUser(task.Id, user.Id))
                    {
                        resourcesData.Delete(user.Id, task.Id);
                    }
                }

                return RedirectToAction("EditUsers");
            }
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View("Index");
        }

        [HttpGet]
        public IActionResult CreateProject(int pg = 1)
        {
            var model = new CreateProjectViewModel();
            var projects = projectData.GetAll().OrderBy(p => p.Name);

            const int pageSize = 5;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = projects.Count();

            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data = projects.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            model.Projects = data;

            return View(model);
        }

        [HttpPost]
        public IActionResult CreateProject(CreateProjectViewModel model, int pg=1)
        {
            if (ModelState.IsValid)
            {
                var project = new Project();
                project.Name = model.ProjectName;
                projectData.Add(project);

                var chat = new Chat
                {
                    Name = project.Name,
                    Type = ChatType.Room
                };
                chatData.Add(chat);
            }

            var projects = projectData.GetAll().OrderBy(p => p.Name);

            const int pageSize = 5;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = projects.Count();

            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data = projects.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            model.Projects = data;

            return View(model);
        }

        [HttpGet]
        public IActionResult DeleteProject(int id)
        {
            var model = projectData.Get(id);

            if (model == null)
            {
                return View("NotFound");
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteProject(Project project)
        {
            var model = projectData.Get(project.Id);
            projectData.Delete(model.Id);

            foreach (var projectUser in projectUserData.GetAll().ToList())
            {
                if (projectUser.ProjectId==project.Id)
                {
                    projectUserData.Delete(projectUser.UserId, project.Id);
                }
            }

            var resources = resourcesData.GetAll();
            var relations = ganttTaskRelationData.GetAll();
            foreach (var task in ganttTaskData.GetAll().ToList())
            {
                if(task.ProjectId == project.Id)
                {
                    ganttTaskData.Delete(task.Id);
                    foreach (var resource in resources.ToList())
                    {
                        if(resource.TaskId == task.Id)
                        {
                            resourcesData.Delete(resource.UserId, task.Id);
                        }
                    }
                    foreach (var relation in relations.ToList())
                    {
                        if (relation.GanttTaskId == task.Id || relation.RelatedTaskId == task.Id)
                        {
                            ganttTaskRelationData.Delete(relation.GanttTaskId, relation.RelatedTaskId);
                        }
                    }
                    ganttTaskData.Delete(task.Id);
                }
            }

            var chat = chatData.GetByName(model.Name);
            chatData.Delete(chat.Id);
            
            db.SaveChanges();

            return RedirectToAction("CreateProject");
        }

        [HttpGet]
        public IActionResult AddManager(string sortOrder, int pg=1, string SearchText = "")
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var projects = from s in projectData.GetAll()
                           select s;
            var projectUsers = projectUserData.GetAll();
            var addManagerViewModel = new List<AddManagerViewModel>();

            foreach(var project in projects)
            {
                var manager = false;
                foreach(var projectUser in projectUsers)
                {
                    if(projectUser.ProjectId == project.Id)
                    {
                        var user = db.Users.Find(projectUser.UserId);
                        var result = userManager.IsInRoleAsync(user, "Manager");
                        if(result.Result == true)
                        {
                            manager = true;
                            break;
                        }
                    }
                }
                var temp = new AddManagerViewModel();
                temp.Project = project;
                temp.HasManager = manager;
                addManagerViewModel.Add(temp);
            }

            if (SearchText != "" && SearchText != null)
            {
                addManagerViewModel = addManagerViewModel.OrderBy(p => p.Project.Name).Where(m => m.Project.Name.Contains(SearchText)).ToList();
            }
            else
            {
                addManagerViewModel = addManagerViewModel.OrderBy(p => p.Project.Name).ToList();
            }

            const int pageSize = 5;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = addManagerViewModel.Count();

            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data = addManagerViewModel.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            switch (sortOrder)
            {
                case "name_desc":
                    data = data.OrderByDescending(s => s.Project.Name).ToList();
                    break;
                default:
                    data = data.OrderBy(s => s.Project.Name).ToList();
                    break;
            }

            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> SelectManager(int id, int pg = 1, string SearchText = "")
        {
            var project = projectData.Get(id);
            var projectUser = projectUserData.GetAll();
            if (project == null)
            {
                ViewBag.ErrorMessage = $"Role with Id= {id} cannot be found";
                return View("NotFound");
            }

            var managerRole = await roleManager.FindByNameAsync("Manager");
            var users = new List<AppUser>();
            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, role: managerRole.Name))
                {
                    users.Add(user);
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

            var model = new List<UserProjectViewModel>();
            foreach (var user in users)
            {
                var userProjectViewModel = new UserProjectViewModel
                {
                    ProjectId = project.Id,
                    UserId = user.Id,
                    UserName = user.UserName
                };
                foreach (var existingUser in projectUser)
                    if (user.Id == existingUser.UserId && project.Id == existingUser.ProjectId)
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
        
        [HttpPost]
        public async Task<IActionResult> SelectManager(List<UserProjectViewModel> model)
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
                    foreach(var projectUser in projectUserData.GetAll().ToList())
                    {
                        var tempChat = chatData.GetByName(project.Name);
                        if(projectUser.ProjectId == project.Id)
                        {
                            var tempUser = db.Users.Find(projectUser.UserId);
                            if(userManager.IsInRoleAsync(tempUser,"Manager").Result)
                            {
                                projectUserData.Delete(tempUser.Id, project.Id);
                                chatUserData.Delete(tempUser.Id, tempChat.Id);
                            }
                            await db.SaveChangesAsync();
                        }
                    }
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
                        Description = $"You have been added as a manager to the '{project.Name}' project",
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
                    return RedirectToAction("AddManager");

            }
            return RedirectToAction("AddManager");
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
