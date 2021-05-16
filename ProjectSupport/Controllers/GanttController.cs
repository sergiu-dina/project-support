using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    public class GanttController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly AppDbContext db;
        private readonly IProjectData projectData;
        private readonly IProjectUserData projectUserData;

        public GanttController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager,AppDbContext db, IProjectData projectData, IProjectUserData projectUserData)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.db = db;
            this.projectData = projectData;
            this.projectUserData = projectUserData;
        }
        public IActionResult Index(string sortOrder, int pg = 1, string SearchText = "")
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

        public IActionResult Gantt(int id)
        {
            return View();
        }
    }
}
