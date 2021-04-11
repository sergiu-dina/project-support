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

namespace ProjectSupport.Controllers
{
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AppUser> userManager;
        private readonly AppDbContext db;

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, AppDbContext db)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.db = db;
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

        [HttpGet]
        public async Task<IActionResult> EditUsers(string sortOrder)
        {
            ViewBag.EmailSortParm = String.IsNullOrEmpty(sortOrder) ? "email_desc" : "";
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var users = from u in db.Users
                         select u;

            switch(sortOrder)
            {
                case "email_desc":
                    users = users.OrderByDescending(u => u.Email);
                    break;
                case "name_desc":
                    users = users.OrderByDescending(s => s.LastName);
                    break;
                default:
                    users = users.OrderBy(u => u.LastName);
                    break;
            }

            var userRolesViewModel = new List<UserRolesViewModel>();
            foreach(AppUser user in users)
            {
                var temp = new UserRolesViewModel();
                temp.UserId = user.Id;
                temp.FirstName = user.FirstName;
                temp.LastName = user.LastName;
                temp.Email = user.Email;
                temp.Roles = await GetUserRoles(user);
                userRolesViewModel.Add(temp);
            }
            return View(userRolesViewModel);
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
                }
                else
                {
                    userRolesViewModel.Selected = false;
                }
                model.Add(userRolesViewModel);
            }
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
            result = await userManager.AddToRolesAsync(user, model.Where(x => x.Selected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }
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
                return RedirectToAction("EditUsers");
            }
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View("Index");
        }
    }
}
