using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectSupport.Areas.Identity.Data;
using ProjectSupport.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Controllers
{
    public class GanttController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly AppDbContext db;

        public GanttController(UserManager<AppUser> userManager, AppDbContext db)
        {
            this.userManager = userManager;
            this.db = db;
        }
        public IActionResult Index()
        {
            var model = db.Projects.OrderBy(p => p.Name);
            return View(model);
        }

        public IActionResult Gantt()
        {
            return View();
        }
    }
}
