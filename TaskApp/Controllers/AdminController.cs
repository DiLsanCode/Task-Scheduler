using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using TaskApp.Business.Constants;
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
                var isTaken = await _adminService.IsProjectNameTaken(newProject.Name);
                if (!isTaken)
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
        public async Task<IActionResult> AddNewSprint(int id)
        {
            ViewBag.currentProject = await _adminService.GetProjectById(id);
            ViewBag.projectList = await _adminService.GetListOfProjects();
            if (id == 0)
            {
                ModelState.AddModelError("PropertyNameInViewModelToBeHighlighted", "Sprint was not selected!");
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                ViewBag.errors = allErrors.ToList();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewSprint(dtoSprint newSprint)
        {
            ViewBag.currentProject = await _adminService.GetProjectById(newSprint.ProjectId);
            ViewBag.projectList = await _adminService.GetListOfProjects();
            if (ModelState.IsValid)
            {
                var isTaken = await _adminService.IsSprintNameTaken(newSprint.Name);
                if (!isTaken)
                {
                    await _adminService.AddNewSprint(newSprint);
                    return RedirectToAction("Sprints", "User", new { projectId = newSprint.ProjectId });
                }
                else
                {
                    ModelState.AddModelError("PropertyNameInViewModelToBeHighlighted", "Sprint with that name already exists!");
                }
            }
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            ViewBag.errors = allErrors.ToList();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditSprint(int id)
        {
            ViewBag.projectList = await _adminService.GetListOfProjects();
            var sprint = await _adminService.GetSprintById(id);
            return View(sprint);
        }

        [HttpPost]
        public async Task<IActionResult> EditSprint(int id, dtoSprint sprint)
        {
            var currentSprint = await _adminService.GetSprintById(id);
            if (ModelState.IsValid)
            {
                var isTaken = await _adminService.IsSprintNameTaken(sprint.Name);

                if (!isTaken)
                {
                    await _adminService.EditSprint(id, sprint);
                    return RedirectToAction("Sprints", "User", new { projectId = sprint.ProjectId });
                }
                else
                {
                    ModelState.AddModelError("PropertyNameInViewModelToBeHighlighted", "There is a sprint with that name!");
                    IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                    ViewBag.errors = allErrors.ToList();
                }

            }
            return View(currentSprint);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSprint(int id)
        {
            if (ModelState.IsValid)
            {
                await _adminService.DeleteSprint(id);
                return RedirectToAction("ListOfAllProjects", "User");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AddNewTaskToProject(int id)
        {
            ViewBag.currentSprintId = await _adminService.GetSprintById(id);
            ViewBag.userList = await _adminService.GetListOfUsers();
            ViewBag.statusList = _adminService.StatusList();
            ViewBag.fibonacciNumbers = _adminService.GetFibunacciList();
            if (id == 0)
            {
                ModelState.AddModelError("PropertyNameInViewModelToBeHighlighted", "Task was not selected!");
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                ViewBag.errors = allErrors.ToList();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewTaskToProject(dtoTask newTask)
        {
            ViewBag.currentSprintId = await _adminService.GetSprintById(newTask.SprintId);
            ViewBag.userList = await _adminService.GetListOfUsers();
            ViewBag.statusList = _adminService.StatusList();
            ViewBag.fibonacciNumbers = _adminService.GetFibunacciList();
            if (ModelState.IsValid)
            {
                var isTaken = await _adminService.IsTaskNameTaken(newTask.Name);
                if (!isTaken)
                {
                    await _adminService.AddNewTask(newTask);
                    return RedirectToAction("AddNewTaskToProject", "Admin");
                }
                else
                {
                    ModelState.AddModelError("PropertyNameInViewModelToBeHighlighted", "Task with that name already exists!");
                    IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                    ViewBag.errors = allErrors.ToList();
                }

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
                var isTaken = await _adminService.IsProjectNameTaken(project.Name);

                if (!isTaken)
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
            ViewBag.sprints = await _adminService.GetListOfSprints();
            ViewBag.fibonacciNumbers = _adminService.GetFibunacciList();
            var task = await _adminService.GetTaskById(id);
            ViewBag.currentProjectId = task.SprintId;
            return View(task);
        }

        [HttpPost]
        public async Task<IActionResult> EditTask(int id, dtoTask task, int currentUserId)
        {
            if (ModelState.IsValid)
            {
                await _adminService.EditTask(id, task);
                return RedirectToAction("ListOfAllTasksFromProject", "User", new { sprintId = task.SprintId, userId = currentUserId });
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
