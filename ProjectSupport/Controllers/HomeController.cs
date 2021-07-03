using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectSupport.Areas.Identity.Data;
using ProjectSupport.Models;
using ProjectSupport.Models.Services;
using ProjectSupport.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> userManager;
        private readonly IProjectData projectData;
        private readonly IGanttTaskData ganttTaskData;
        private readonly IProjectUserData projectUserData;
        private readonly IResourcesData resourcesData;
        private readonly IGanttTaskRelationData ganttTaskRelationData;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, IProjectData projectData, IGanttTaskData ganttTaskData,
            IProjectUserData projectUserData, IResourcesData resourcesData, IGanttTaskRelationData ganttTaskRelationData)
        {
            _logger = logger;
            this.userManager = userManager;
            this.projectData = projectData;
            this.ganttTaskData = ganttTaskData;
            this.projectUserData = projectUserData;
            this.resourcesData = resourcesData;
            this.ganttTaskRelationData = ganttTaskRelationData;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetPieJsonData()
        {
            var model = new List<PieChartViewModel>();
            var projects = projectData.GetAll();
            var tasks = ganttTaskData.GetAll();
            var resources = resourcesData.GetAll();

            foreach(var project in projects)
            {
                var temp = new PieChartViewModel();
                temp.ProjectName = project.Name;
                foreach(var task in tasks)
                {
                    if(task.ProjectId==project.Id)
                    {
                        foreach(var resource in resources)
                        {
                            if(resource.TaskId== task.Id)
                            {
                                var user = await userManager.FindByIdAsync(resource.UserId);
                                temp.Cost += user.HourlyRate * task.Duration * 8;
                            }
                        }
                    }
                }
                model.Add(temp);
            }

            var data = model.OrderByDescending(m => m.Cost).Take(5).ToList();

            return Json(data);
        }

        [HttpGet]
        public JsonResult GetColumnJsonData()
        {
            var model = new List<ColumnChartViewModel>();
            var projects = projectData.GetAll();
            var tasks = ganttTaskData.GetAll();
            var projectUsers = projectUserData.GetAll();
            var dependencies = ganttTaskRelationData.GetAll();

            foreach (var project in projects)
            {
                var temp = new ColumnChartViewModel();
                temp.ProjectName = project.Name;
                foreach (var task in tasks)
                {
                    if (task.ProjectId == project.Id)
                    {
                        temp.Tasks++;
                        foreach(var relation in dependencies)
                        {
                            if (relation.GanttTaskId == task.Id)
                            {
                                temp.Dependencies++;
                            }
                        }
                    }
                }
                foreach(var projectUser in projectUsers)
                {
                    if(projectUser.ProjectId == project.Id)
                    {
                        temp.Users++;
                    }
                }
                model.Add(temp);
            }

            var data = model.OrderByDescending(m => m.Tasks).Take(9).ToList();
           
            return Json(data);
        }

        [HttpGet]
        public JsonResult GetDonutJsonData()
        {
            var model = new List<DonutChartViewModel>();
            var projects = projectData.GetAll();
            var tasks = ganttTaskData.GetAll();
            var resources = resourcesData.GetAll();

            foreach (var project in projects)
            {
                var temp = new DonutChartViewModel();
                temp.ProjectName = project.Name;
                var projectTasks = new List<GanttTask>();
                foreach (var task in tasks)
                {
                    if (task.ProjectId == project.Id)
                    {
                        projectTasks.Add(task);
                    }
                }
                if (projectTasks.Count() > 0)
                {
                    projectTasks = projectTasks.OrderBy(pt => pt.StartDate).ThenBy(pt => pt.EndDate).ToList();
                    temp.Duration = (projectTasks[projectTasks.Count() - 1].EndDate - projectTasks[0].StartDate).Days;
                }

                model.Add(temp);
            }

            var data = model.OrderByDescending(m => m.Duration).Take(5).ToList();

            return Json(data);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
