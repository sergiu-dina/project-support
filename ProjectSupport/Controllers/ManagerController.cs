using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PagedList;
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

        public ManagerController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, AppDbContext db,
            IProjectData projectData, IProjectUserData projectUserData, IGanttTaskData ganttTaskData, IResourcesData resourcesData)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.db = db;
            this.projectData = projectData;
            this.projectUserData = projectUserData;
            this.ganttTaskData = ganttTaskData;
            this.resourcesData = resourcesData;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult EditProject(int id)
        {
            var model = projectData.Get(id);
            if (model == null)
            {
                return View("NotFound");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProject(Project project)
        {
            if (ModelState.IsValid)
            {
                projectData.Update(project);
                return RedirectToAction("SeeProjects");
            }
            return View();
        }

        public IActionResult SeeProjects(string SearchText="", int pg = 1)
        {
            List<Project> model;
            if (SearchText != "" && SearchText != null)
            {
                model = projectData.GetAll().OrderBy(p => p.Name).Where(m => m.Name.Contains(SearchText)).ToList();
            }
            else
            {
                model = projectData.GetAll().OrderBy(p => p.Name).ToList();
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
        public IActionResult AddDevelopers(string sortOrder, int pg = 1, string SearchText = "")
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var projects = from s in projectData.GetAll()
                           select s;

            if (SearchText != "" && SearchText != null)
            {
                projects = projectData.GetAll().OrderBy(p => p.Name).Where(m => m.Name.Contains(SearchText)).ToList();
            }
            else
            {
                projects = projectData.GetAll().OrderBy(p => p.Name).ToList();
            }

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

            switch (sortOrder)
            {
                case "name_desc":
                    data = data.OrderByDescending(s => s.Name).ToList();
                    break;
                default:
                    data = data.OrderBy(s => s.Name).ToList();
                    break;
            }

            return View(data);
        }

        
        [HttpGet]
        public async Task<IActionResult> SelectDevelopers(int id, int pg = 1, string SearchText = "")
        {
            var project = projectData.Get(id);
            var projectUser = projectUserData.GetAll();
            if (project == null)
            {
                ViewBag.ErrorMessage = $"Role with Id= {id} cannot be found";
                return View("NotFound");
            }

            var developerRole = await roleManager.FindByNameAsync("Developer");
            var users = new List<AppUser>();
            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, role: developerRole.Name))
                {
                    users.Add(user);
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
            foreach (var user in data)
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
                    db.SaveChanges();
                }
                else if (!model[i].IsSelected && projectUserData.HasUser(project.Id, user.Id))
                {
                    projectUserData.Delete(user.Id, project.Id);

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
                    db.SaveChanges();
                }
                else
                {
                    continue;
                }

                if (i < (model.Count - 1))
                    continue;
                else
                    return RedirectToAction("AddDevelopers");

            }
            return RedirectToAction("AddDevelopers");
        }
        
    }
}
