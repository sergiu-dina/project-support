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

        public ManagerController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, AppDbContext db, IProjectData projectData, IProjectUserData projectUserData)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.db = db;
            this.projectData = projectData;
            this.projectUserData = projectUserData;
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

        [HttpGet]
        public IActionResult SeeProjects()
        {
            var model = projectData.GetAll().OrderBy(p => p.Name);
            return View(model);
        }

        [HttpGet]
        public IActionResult AddDevelopers(string sortOrder, string searchString, string currentFilter, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var projects = from s in projectData.GetAll()
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                projects = projects.Where(p => p.Name.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    projects = projects.OrderByDescending(s => s.Name);
                    break;
                default:
                    projects = projects.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);

            return View(projects.ToPagedList(pageNumber, pageSize));
        }

        
        [HttpGet]
        public async Task<IActionResult> SelectDevelopers(int id, string searchString)
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

            if (!String.IsNullOrEmpty(searchString))
            {
                model = model.Where(m => m.UserName.Contains(searchString)).ToList();
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
