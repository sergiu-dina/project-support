using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Controllers
{
    public class GanttController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Gantt()
        {
            return View();
        }
    }
}
