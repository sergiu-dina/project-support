using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectSupport.Areas.Identity.Data;
using ProjectSupport.Data;
using ProjectSupport.Models;
using ProjectSupport.Models.Services;
using ProjectSupport.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Controllers
{
    [Authorize]
    public class GanttController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly AppDbContext db;
        private readonly IProjectData projectData;
        private readonly IGanttTaskData ganttTaskData;
        private readonly IProjectUserData projectUserData;
        private readonly IResourcesData resourcesData;

        public GanttController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext db,
            IProjectData projectData, IGanttTaskData ganttTaskData, IProjectUserData projectUserData, IResourcesData resourcesData)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.db = db;
            this.projectData = projectData;
            this.ganttTaskData = ganttTaskData;
            this.projectUserData = projectUserData;
            this.resourcesData = resourcesData;
        }
        public async Task<IActionResult> Index(string id, string sortOrder, int pg = 1, string SearchText = "")
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var projects = from s in projectData.GetAll()
                           select s;

            var managerProjects = new List<ProjectIndexViewModel>();
            var managerRole = await roleManager.FindByNameAsync("Manager");

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            foreach (var project in projects)
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
                managerProjects.Add(managerProject);
            }


            if (SearchText != "" && SearchText != null)
            {
                managerProjects = managerProjects.OrderByDescending(p => p.IsManager).Where(m => m.Project.Name.Contains(SearchText)).ToList();
            }
            else
            {
                managerProjects = managerProjects.OrderByDescending(p => p.IsManager).ToList();
            }

            const int pageSize = 5;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = managerProjects.Count();

            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data = managerProjects.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            switch (sortOrder)
            {
                case "name_desc":
                    data = data.OrderByDescending(s => s.IsManager).ThenByDescending(s => s.Project.Name).ToList();
                    break;
                default:
                    data = data.OrderByDescending(s => s.IsManager).ThenBy(s => s.Project.Name).ToList();
                    break;
            }  

            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, string sortOrder, int pg = 1)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var model = new ProjectUsersViewModel();

            var manager = new AppUser();
            var managerRole = await roleManager.FindByNameAsync("Manager");
            var developers = new List<AppUser>();

            var project = projectData.Get(id);
            var projectUsers = projectUserData.GetAll();
            var users = userManager.Users;

            foreach (var user in users)
            {
                foreach (var projectUser in projectUsers)
                {
                    if (user.Id == projectUser.UserId && project.Id == projectUser.ProjectId)
                    {
                        if (await userManager.IsInRoleAsync(user, role: managerRole.Name))
                        {
                            manager = user;
                        }
                        else
                        {
                            developers.Add(user);
                        }
                    }
                }
            }

            model.Project = project;
            model.Manager = manager;
            model.Developers = developers;

            const int pageSize = 3;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = model.Developers.Count();

            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            model.Developers = model.Developers.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            switch (sortOrder)
            {
                case "name_desc":
                    model.Developers = model.Developers.OrderByDescending(s => s.Email).ToList();
                    break;
                default:
                    model.Developers = model.Developers.OrderBy(s => s.Email).ToList();
                    break;
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Gantt(int id)
        {
            var tasks = ganttTaskData.GetAll();
            var projectTasks = new List<GanttTask>();

            foreach (var task in tasks)
            {
                if (task.ProjectId == id)
                {
                    projectTasks.Add(task);
                }
            }

            return View(projectTasks);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> Actions(int id, string user)
        {
            var manager = await userManager.FindByIdAsync(user);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            return View(id);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public IActionResult AddTask(int id)
        {
            var model = new TaskProjectViewModel();

            var tempTask = new GanttTask();
            var tasks = new List<GanttTask>();
            foreach (var task in ganttTaskData.GetAll())
            {
                if (task.ProjectId == id)
                {
                    tasks.Add(task);
                }
            }
            var project = projectData.Get(id);

            model.ProjectId = id;
            model.Task = tempTask;
            model.Task.ProjectId = id;
            model.Task.Project = project;
            model.Tasks = new List<SelectListItem>();
            foreach (var task in tasks)
            {
                var item = new SelectListItem { Text = task.Name, Value = task.Name };
                model.Tasks.Add(item);
            }

            return View(model);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public IActionResult AddTask(TaskProjectViewModel model)
        {
            var taskName = model.Selected;

            model.Task.Dependency = taskName;
            model.Task.Duration = (model.Task.EndDate - model.Task.StartDate).Days;

            var task = model.Task;
            task.ProjectId = model.ProjectId;

            if (ModelState.IsValid)
            {
                ganttTaskData.Add(task);
                return RedirectToAction("Actions", new { id = model.Task.ProjectId });
            }
            else
            {
                return RedirectToAction("AddTask", new { id = model.ProjectId });
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public IActionResult SeeTasks(int id, string sortOrder, int pg = 1, string SearchText = "")
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";

            var tasks = ganttTaskData.GetAll();
            var model = new List<GanttTask>();

            foreach (var task in tasks)
            {
                if (task.ProjectId == id)
                {
                    task.EndDate = task.EndDate.Date;
                    task.StartDate = task.StartDate.Date;
                    model.Add(task);
                }
            }

            if (SearchText != "" && SearchText != null)
            {
                model = model.OrderBy(p => p.Name).Where(m => m.Name.Contains(SearchText)).ToList();
            }
            else
            {
                model = model.OrderBy(p => p.Name).ToList();
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
                case "date_desc":
                    data = data.OrderByDescending(s => s.StartDate).ToList();
                    break;
                default:
                    data = data.OrderBy(s => s.StartDate).ThenBy(s => s.EndDate).ToList();
                    break;
            }

            ViewBag.id = id;

            return View(data);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public IActionResult EditTask(int id)
        {
            var model = ganttTaskData.Get(id);
            if (model == null)
            {
                return View("NotFound");
            }
            return View(model);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTask(GanttTask task)
        {
            if (ModelState.IsValid)
            {
                ganttTaskData.Update(task);
                return RedirectToAction("SeeTasks", new { id = task.ProjectId });
            }
            return View();
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public IActionResult RemoveTask(int id)
        {
            var model = ganttTaskData.Get(id);

            if (model == null)
            {
                return View("NotFound");
            }

            return View(model);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public IActionResult RemoveTask(GanttTask task)
        {
            var model = ganttTaskData.Get(task.Id);
            ganttTaskData.Delete(model.Id);

            return RedirectToAction("SeeTasks", new { id = task.ProjectId });
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public IActionResult ManageResources(int id, string sortOrder, int pg = 1, string SearchText = "")
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";

            var tasks = ganttTaskData.GetAll();
            var model = new List<GanttTask>();

            foreach (var task in tasks)
            {
                if (task.ProjectId == id)
                {
                    task.EndDate = task.EndDate.Date;
                    task.StartDate = task.StartDate.Date;
                    model.Add(task);
                }
            }

            if (SearchText != "" && SearchText != null)
            {
                model = model.OrderBy(p => p.Name).Where(m => m.Name.Contains(SearchText)).ToList();
            }
            else
            {
                model = model.OrderBy(p => p.Name).ToList();
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
                case "date_desc":
                    data = data.OrderByDescending(s => s.StartDate).ToList();
                    break;
                default:
                    data = data.OrderBy(s => s.StartDate).ThenBy(s => s.EndDate).ToList();
                    break;
            }

            ViewBag.id = id;

            return View(data);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> EditResources(int id, string sortOrder, int pg = 1, string SearchText = "")
        {
            var model = new List<ResourcesViewModel>();
            var task = ganttTaskData.Get(id);

            if (task == null)
            {
                ViewBag.ErrorMessage = $"Role with Id= {id} cannot be found";
                return View("NotFound");
            }

            var project = projectData.Get(task.ProjectId);
            var projectUsers = projectUserData.GetAll();
            var developerRole = await roleManager.FindByNameAsync("Developer");

            var developers = new List<AppUser>();
            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, role: developerRole.Name))
                {
                    developers.Add(user);
                }
            }

            var users = new List<AppUser>();
            foreach (var developer in developers)
            {
                foreach (var projectUser in projectUsers)
                {
                    if (developer.Id == projectUser.UserId && project.Id == projectUser.ProjectId)
                    {
                        users.Add(developer);
                    }
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


            var resources = resourcesData.GetAll();
            foreach (var user in data)
            {
                var resourceViewModel = new ResourcesViewModel
                {
                    TaskId = task.Id,
                    UserId = user.Id,
                    UserName = user.UserName
                };
                foreach (var resource in resources)
                    if (user.Id == resource.UserId && task.Id == resource.TaskId)
                    {
                        resourceViewModel.IsSelected = true;
                        break;
                    }
                    else
                    {
                        resourceViewModel.IsSelected = false;
                    }
                model.Add(resourceViewModel);
            }

            if (SearchText != "" && SearchText != null)
            {
                model = model.OrderBy(p => p.UserName).Where(m => m.UserName.Contains(SearchText)).ToList();
            }
            else
            {
                model = model.OrderBy(p => p.UserName).ToList();
            }

            ViewBag.id = project.Id;

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditResources(List<ResourcesViewModel> model)
        {
            var task = ganttTaskData.Get(model[0].TaskId);
            if (task == null)
            {
                ViewBag.ErrorMessage = $"Role with Id= {model[0].TaskId} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                if (model[i].IsSelected && !(resourcesData.HasUser(task.Id, user.Id)))
                {
                    var temp = new Resources();
                    temp.TaskId = task.Id;
                    temp.UserId = model[i].UserId;
                    resourcesData.Add(temp);
                    db.SaveChanges();
                }
                else if (!model[i].IsSelected && resourcesData.HasUser(task.Id, user.Id))
                {
                    resourcesData.Delete(user.Id, task.Id);
                    db.SaveChanges();
                }
                else
                {
                    continue;
                }

                if (i < (model.Count - 1))
                    continue;
                else
                    return RedirectToAction("ManageResources", new { id = task.ProjectId });
            }

            return RedirectToAction("ManageResources", new { id = task.ProjectId });
        }
    }
}
