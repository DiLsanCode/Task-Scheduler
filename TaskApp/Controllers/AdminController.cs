using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        public IActionResult AddNewProject() 
        {
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            ViewBag.errors = allErrors.ToList();
            return View(); 
        }

        [HttpPost]
        public async Task<IActionResult> AddNewProject(dtoProject newProject)
        {
            if (ModelState.IsValid)
            {
                var existingProject = await _adminService.GetProjectByName(newProject.Name);
                if(existingProject == null)
                {
                    await _adminService.AddNewProject(newProject);
                    return RedirectToAction("ListOfAllProjects", "User");
                }
                else
                {
                    ModelState.AddModelError("PropertyNameInViewModelToBeHighlighted", "Project with that name already exists!");
                }
            }
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            ViewBag.errors = allErrors.ToList();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AddNewTaskToProject(int id)
        {

            ViewBag.userList = await _adminService.GetListOfUsers();
            ViewBag.statusList = _adminService.StatusList();
            ViewBag.currentProjectId = id;
            if(id == 0)
            {
                ModelState.AddModelError("PropertyNameInViewModelToBeHighlighted", "Project was not selected!");
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                ViewBag.errors = allErrors.ToList();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewTaskToProject(dtoTask newTask)
        {
            if (ModelState.IsValid)
            {
                if(newTask.Id != 0)
                {
                    await _adminService.AddNewTask(newTask);
                }
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
            var currentProject = await _adminService.GetProjectById(id);
            if (ModelState.IsValid)
            {
                var existingProject = await _adminService.GetProjectByName(project.Name);

                if (existingProject == null)
                {
                    await _adminService.EditProject(id, project);
                    return RedirectToAction("ListOfAllProjects", "User");
                }
                else
                {
                    ModelState.AddModelError("PropertyNameInViewModelToBeHighlighted", "There is project with that name!");
                    IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                    ViewBag.errors = allErrors.ToList();
                }

            }
            return View(currentProject);
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
