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
using System.Text;
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
        private readonly IGanttTaskRelationData ganttTaskRelationData;

        public GanttController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext db,
            IProjectData projectData, IGanttTaskData ganttTaskData, IProjectUserData projectUserData, IResourcesData resourcesData,
            IGanttTaskRelationData ganttTaskRelationData)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.db = db;
            this.projectData = projectData;
            this.ganttTaskData = ganttTaskData;
            this.projectUserData = projectUserData;
            this.resourcesData = resourcesData;
            this.ganttTaskRelationData = ganttTaskRelationData;
        }
        public async Task<IActionResult> Index(string id, string sortOrder, int pg = 1, string SearchText = "")
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var projects = from s in projectData.GetAll()
                           select s;

            var managerProjects = new List<ProjectIndexViewModel>();
            var managerRole = await roleManager.FindByNameAsync("Manager");
            var developerRole = await roleManager.FindByNameAsync("Developer");

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var manager = await userManager.IsInRoleAsync(user, managerRole.Name);
            var developer = await userManager.IsInRoleAsync(user, developerRole.Name);

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
                            model.DevelopersCount++;
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

            var tasks = ganttTaskData.GetAll();
            foreach(var task in tasks)
            {
                if(project.Id == task.Id)
                {
                    model.Tasks.Add(task);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Gantt(int id)
        {
            var model = new GanttIndexViewModel();
            var tasks = ganttTaskData.GetAll();
            var hasTasks = false;
            foreach(var task in tasks)
            {
                if(task.ProjectId == id)
                {
                    hasTasks = true;
                    break;
                }
            }

            model.ProjectId = id;
            model.HasTasks = hasTasks;

            return View(model);
        }

        [HttpGet]
        public JsonResult GetJsonData(int id)
        {
            var tasks = ganttTaskData.GetAll();
            var ganttViewModel = new List<GanttViewModel>();
            var relations = ganttTaskRelationData.GetAll();

            foreach (var task in tasks)
            {
                if (task.ProjectId == id)
                {
                    var temp = new GanttViewModel();
                    temp.TaskId = task.Id.ToString();
                    temp.Name = task.Name;
                    temp.StartYear = task.StartDate.Year;
                    temp.StartMonth = task.StartDate.Month;
                    temp.StartDay = task.StartDate.Day;

                    temp.EndYear = task.EndDate.Year;
                    temp.EndMonth = task.EndDate.Month;
                    temp.EndDay = task.EndDate.Day;

                    temp.Duration = task.Duration;
                    temp.Progress = task.Progress;
                    

                    var sb = new StringBuilder();
                    var counter = 0;
                    foreach(var relation in relations)
                    {
                        if(task.Id==relation.GanttTaskId)
                        {
                            counter++;
                            var tempId = relation.RelatedTaskId.ToString();
                            sb.Append(tempId);
                            sb.Append(",");
                        }
                    }
                    
                    if (counter > 0)
                    {
                        sb.Remove(sb.Length - 1, 1);
                        temp.Dependencies = sb.ToString();
                    }
                    else
                    {
                        temp.Dependencies = null;
                    }

                    ganttViewModel.Add(temp);
                }
            }

            return Json(ganttViewModel);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> Actions(int id, string user)
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
            else if(manager==false)
            {
                ViewBag.ErrorMessage = $"This User is not the manager of this project";
                return View("NotFound");
            }

            return View(id);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> AddTask(int id, string user)
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
            model.UserId = user;
            model.Task = tempTask;
            model.Task.ProjectId = id;
            model.Task.Project = project;
            model.Tasks = new List<SelectListItem>();
            foreach (var task in tasks)
            {
                var item = new SelectListItem { Text = task.Name, Value = task.Name };
                model.Tasks.Add(item);
            }

            var manager = false;
            var managerRole = await roleManager.FindByNameAsync("Manager");
            var appUser = await userManager.FindByIdAsync(user);

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

            return View(model);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public IActionResult AddTask(TaskProjectViewModel model)
        {
            var taskName = model.Selected;

            model.Task.Duration = (model.Task.EndDate - model.Task.StartDate).Days;

            var task = model.Task;
            task.ProjectId = model.ProjectId;

            if (ModelState.IsValid)
            {
                ganttTaskData.Add(task);
                return RedirectToAction("Actions", new { id = model.Task.ProjectId, user = model.UserId });
            }
            else
            {
                return RedirectToAction("AddTask", new { id = model.ProjectId, user = model.UserId });
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> SeeTasks(int id, string user, string sortOrder, int pg = 1, string SearchText = "")
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

            return View(data);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> EditTask(int id, string user)
        {
            var model = new EditTaskViewModel();
            model.Task = ganttTaskData.Get(id);
            model.UserId = user;

            var manager = false;
            var managerRole = await roleManager.FindByNameAsync("Manager");
            var appUser = await userManager.FindByIdAsync(user);

            var project = projectData.Get(model.Task.ProjectId);
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

            if (model.Task == null)
            {
                return View("NotFound");
            }
            return View(model);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTask(EditTaskViewModel model)
        {
            if (ModelState.IsValid)
            {
                ganttTaskData.Update(model.Task);
                return RedirectToAction("SeeTasks", new { id = model.Task.ProjectId, user= model.UserId});
            }
            return View();
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public IActionResult RemoveTask(int id, string user)
        {
            var model = new EditTaskViewModel();
            model.Task = ganttTaskData.Get(id);
            model.UserId = user;

            if (model == null)
            {
                return View("NotFound");
            }

            return View(model);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public IActionResult RemoveTask(EditTaskViewModel model)
        {
            var relations = ganttTaskRelationData.GetAll();
            
            foreach(var relation in relations)
            {
                if(relation.GanttTaskId == model.Task.Id || relation.RelatedTaskId == model.Task.Id)
                {
                    ganttTaskRelationData.Delete(relation.GanttTaskId, relation.RelatedTaskId);
                }
            }

            ganttTaskData.Delete(model.Task.Id);

            return RedirectToAction("SeeTasks", new { id = model.Task.ProjectId, user = model.UserId });
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> ManageResources(int id, string sortOrder, int pg = 1, string SearchText = "")
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";

            var tasks = ganttTaskData.GetAll();
            var resources = resourcesData.GetAll();
            var model = new List<ManageResourcesViewModel>();

            foreach (var task in tasks)
            {
                var temp = new ManageResourcesViewModel();
                if (task.ProjectId == id)
                {
                    task.EndDate = task.EndDate.Date;
                    task.StartDate = task.StartDate.Date;
                    temp.Task = task;

                    foreach(var resource in resources)
                    {
                        if(resource.TaskId==task.Id)
                        {
                            var user = await userManager.FindByIdAsync(resource.UserId);
                            temp.TaskCost += user.HourlyRate * task.Duration * 8;
                        }
                    }

                    model.Add(temp);
                }
            }

            if (SearchText != "" && SearchText != null)
            {
                model = model.OrderBy(p => p.Task.Name).Where(m => m.Task.Name.Contains(SearchText)).ToList();
            }
            else
            {
                model = model.OrderBy(p => p.Task.Name).ToList();
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
                    data = data.OrderByDescending(s => s.Task.StartDate).ToList();
                    break;
                default:
                    data = data.OrderBy(s => s.Task.StartDate).ThenBy(s => s.Task.EndDate).ToList();
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

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public IActionResult EditDependencies(int id, string sortOrder, int pg = 1, string SearchText = "")
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
        public IActionResult AddDependencies(int id, string sortOrder, int pg = 1, string SearchText = "")
        {
            var model = new List<DependencyViewModel>();
            var task = ganttTaskData.Get(id);

            if (task == null)
            {
                ViewBag.ErrorMessage = $"Role with Id= {id} cannot be found";
                return View("NotFound");
            }

            var project = projectData.Get(task.ProjectId);
            var tasks = ganttTaskData.GetAll();
            var projectTasks = new List<GanttTask>();

            foreach(var projectTask in tasks)
            {
                if(projectTask.ProjectId == project.Id && projectTask.Id != task.Id)
                {
                    projectTasks.Add(projectTask);
                }
            }

            const int pageSize = 5;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = projectTasks.Count();

            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data = projectTasks.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;


            var relations = ganttTaskRelationData.GetAll();
            foreach (var projectTask in data)
            {
                var dependencyViewModel = new DependencyViewModel
                {
                    TaskId = task.Id,
                    RelatedTaskId = projectTask.Id,
                    RelatedTaskName = projectTask.Name
                };
                foreach (var relation in relations)
                    if (projectTask.Id == relation.RelatedTaskId && task.Id == relation.GanttTaskId)
                    {
                        dependencyViewModel.IsSelected = true;
                        break;
                    }
                    else
                    {
                        dependencyViewModel.IsSelected = false;
                    }
                model.Add(dependencyViewModel);
            }

            if (SearchText != "" && SearchText != null)
            {
                model = model.OrderBy(p => p.RelatedTaskName).Where(m => m.RelatedTaskName.Contains(SearchText)).ToList();
            }
            else
            {
                model = model.OrderBy(p => p.RelatedTaskName).ToList();
            }

            ViewBag.id = project.Id;

            return View(model);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public IActionResult AddDependencies(List<DependencyViewModel> model)
        {
            GanttTask task;
            if (model.Count > 0)
            {
                task = ganttTaskData.Get(model[0].TaskId);
            }
            else
            {
                ViewBag.ErrorMessage = $"The Task cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var relatedTask = ganttTaskData.Get(model[i].RelatedTaskId);
                if (model[i].IsSelected && !(ganttTaskRelationData.HasRelation(task.Id, relatedTask.Id)))
                {
                    var temp = new GanttTaskRelation();
                    temp.GanttTaskId = task.Id;
                    temp.RelatedTaskId = model[i].RelatedTaskId;
                    ganttTaskRelationData.Add(temp);
                    db.SaveChanges();
                }
                else if (!model[i].IsSelected && ganttTaskRelationData.HasRelation(task.Id, relatedTask.Id))
                {
                    ganttTaskRelationData.Delete(task.Id, relatedTask.Id);
                    db.SaveChanges();
                }
                else
                {
                    continue;
                }

                if (i < (model.Count - 1))
                    continue;
                else
                    return RedirectToAction("EditDependencies", new { id = task.ProjectId });
            }

            return RedirectToAction("EditDependencies", new { id = task.ProjectId });
        }

        [Authorize(Roles = "Developer")]
        [HttpGet]
        public async Task<IActionResult> DeveloperTasks(int id,string user, string sortOrder, int pg = 1, string SearchText = "")
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";

            var developer = false;
            var developerRole = await roleManager.FindByNameAsync("Developer");
            var appUser = await userManager.FindByIdAsync(user);

            var project = projectData.Get(id);
            var projectUsers = projectUserData.GetAll();

            foreach (var projectUser in projectUsers)
            {
                if (user == projectUser.UserId && project.Id == projectUser.ProjectId)
                {
                    if (await userManager.IsInRoleAsync(appUser, role: developerRole.Name))
                    {
                        developer = true;
                    }
                }
            }


            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            else if (developer == false)
            {
                ViewBag.ErrorMessage = $"This User is not a developer in this project";
                return View("NotFound");
            }

            var tasks = ganttTaskData.GetAll();
            var model = new List<AssignedTaskViewModel>();

            foreach (var task in tasks)
            {
                var assignedTask = new AssignedTaskViewModel();
                if (task.ProjectId == id)
                {
                    if (resourcesData.HasUser(task.Id, user))
                    {
                        assignedTask.IsAssigned = true;
                    }
                    else
                    {
                        assignedTask.IsAssigned = false;
                    }
                    assignedTask.Task = task;
                    model.Add(assignedTask);
                }
            }


            if (SearchText != "" && SearchText != null)
            {
                model = model.OrderByDescending(s => s.IsAssigned).ThenBy(p => p.Task.Name).Where(m => m.Task.Name.Contains(SearchText)).ToList();
            }
            else
            {
                model = model.OrderByDescending(s => s.IsAssigned).ThenBy(p => p.Task.Name).ToList();
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
                    data = data.OrderByDescending(s => s.Task.StartDate).ThenBy(s => s.IsAssigned).ToList();
                    break;
                default:
                    data = data.OrderBy(s => s.Task.StartDate).ThenBy(s => s.Task.EndDate).ThenBy(s => s.IsAssigned).ToList();
                    break;
            }

            ViewBag.id = id;

            return View(data);
        }

        [Authorize(Roles = "Developer")]
        [HttpGet]
        public async Task<IActionResult> AddProgress(int id, string user)
        {
            var model = new EditTaskViewModel();
            model.Task = ganttTaskData.Get(id);
            model.UserId = user;

            var developer = false;
            var developerRole = await roleManager.FindByNameAsync("Developer");
            var appUser = await userManager.FindByIdAsync(user);

            foreach (var resource in resourcesData.GetAll())
            {
                if (user == resource.UserId && model.Task.Id == resource.TaskId)
                {
                    if (await userManager.IsInRoleAsync(appUser, role: developerRole.Name))
                    {
                        developer = true;
                    }
                }
            }

            if (appUser == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            else if (developer == false)
            {
                ViewBag.ErrorMessage = $"This User is not a developer in this Task";
                return View("NotFound");
            }

            if (model.Task == null)
            {
                return View("NotFound");
            }
            return View(model);
        }

        [Authorize(Roles = "Developer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProgress(EditTaskViewModel model)
        {
            if (ModelState.IsValid)
            {
                ganttTaskData.Update(model.Task);
                return RedirectToAction("DeveloperTasks", new { id = model.Task.ProjectId, user = model.UserId });
            }
            return View();
        }
    }
}
