using Microsoft.AspNetCore.Identity;
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

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, IGanttTaskData ganttTaskData,
            AppDbContext db, IProjectData projectData, IProjectUserData projectUserData, IResourcesData resourcesData)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.ganttTaskData = ganttTaskData;
            this.db = db;
            this.projectData = projectData;
            this.projectUserData = projectUserData;
            this.resourcesData = resourcesData;
        }
        public IActionResult Index()
        {
            return View();
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

        public async Task<ViewResult> EditUsers(string sortOrder, string searchString, int pg = 1, string SearchText = "")
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
        public async Task<IActionResult> AddRole (string userId)
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

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(List<AddRoleViewModel> model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View();
            }
            
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

                        var tasks = ganttTaskData.GetAll();
                        var resources = resourcesData.GetAll();
                        foreach(var task in tasks)
                        {
                            if(resourcesData.HasUser(task.Id, user.Id))
                            {
                                resourcesData.Delete(user.Id, task.Id);
                            }
                        }
                    }
                }
            }
            result = await userManager.AddToRolesAsync(user, model.Where(x => x.Selected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }
            db.SaveChanges();
            return RedirectToAction("EditUsers");
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

            foreach (var projectUser in projectUserData.GetAll())
            {
                if (projectUser.ProjectId==project.Id)
                {
                    projectUserData.Delete(projectUser.UserId, project.Id);
                }
            }

            var resources = resourcesData.GetAll();
            foreach (var task in ganttTaskData.GetAll())
            {
                if(task.ProjectId == project.Id)
                {
                    ganttTaskData.Delete(task.ProjectId);
                    foreach(var resource in resources)
                    {
                        if(resource.TaskId == task.Id)
                        {
                            resourcesData.Delete(resource.UserId, task.Id);
                        }
                    }
                }
            }

            return RedirectToAction("CreateProject");
        }

        [HttpGet]
        public IActionResult AddManager(string sortOrder, int pg=1, string SearchText = "")
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var projects = from s in projectData.GetAll()
                           select s;

            if (SearchText != "" && SearchText != null)
            {
                projects = projects.OrderBy(p => p.Name).Where(m => m.Name.Contains(SearchText)).ToList();
            }
            else
            {
                projects = projects.OrderBy(p => p.Name).ToList();
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
                    projectUserData.Add(temp);
                    db.SaveChanges();
                }
                else if (!model[i].IsSelected && projectUserData.HasUser(project.Id, user.Id))
                {
                    projectUserData.Delete(user.Id, project.Id);
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
        
    }
}
