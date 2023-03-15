using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskApp.Business.dto;
using TaskApp.Business.Interfaces;
using TaskApp.Business.Services;
using TaskList.Business.Constants;

namespace TaskApp.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public IActionResult AddNewProject() { return View(); }

        [HttpPost]
        public async Task<IActionResult> AddNewProject(dtoProject newProject)
        {
            if (ModelState.IsValid)
            {
                await _adminService.AddNewProject(newProject);
                return RedirectToAction("ListOfAllProjects", "User");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AddNewTaskToProject(int id)
        {
            ViewBag.userList = await _adminService.GetListOfUsers();
            ViewBag.statusList = _adminService.StatusList();
            ViewBag.currentProjectId = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewTaskToProject(dtoTask newTask)
        {
            if (ModelState.IsValid)
            {
                await _adminService.AddNewTask(newTask);
                return RedirectToAction("AddNewTaskToProject", "Admin");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProject(int id)
        {
            if (ModelState.IsValid)
            {
                await _adminService.DeleteProject(id);
                return RedirectToAction("ListOfAllProjects", "User");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTask(int id)
        {
            if (ModelState.IsValid)
            {
                await _adminService.DeleteTask(id);
                return RedirectToAction("ListOfAllProjects", "User");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditProject(int id)
        {
            var project = await _adminService.GetProjectById(id);
            return View(project);
        }

        [HttpPost]
        public async Task<IActionResult> EditProject(int id, dtoProject project)
        {
            if (ModelState.IsValid)
            {
                await _adminService.EditProject(id, project);
                return RedirectToAction("ListOfAllProjects", "User");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditTask(int id)
        {
            ViewBag.userList = await _adminService.GetListOfUsers();
            ViewBag.statusList = _adminService.StatusList();
            ViewBag.projectList = await _adminService.GetListOfProjects();  
            var task = await _adminService.GetTaskById(id);
            ViewBag.currentProjectId = task.ProjectId;
            return View(task);
        }

        [HttpPost]
        public async Task<IActionResult> EditTask(int id, dtoTask task)
        {
            if (ModelState.IsValid)
            {
                await _adminService.EditTask(id, task);
                return RedirectToAction("ListOfAllTasksFromProject", "User", new { projectId = task.ProjectId });
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> TaskBoard()
        {
            var tasks = await _adminService.GetInReviewTask();
            return View(tasks);
        }
    }
}
