using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectSupport.Areas.Identity.Data;
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

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
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
    }
}
