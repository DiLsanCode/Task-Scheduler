using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using TaskApp.Business.dto;
using TaskApp.Business.Interfaces;
using TaskApp.Business.Services;
using TaskList.Business.Constants;
using TaskList.Data.Models;

namespace TaskApp.Controllers
{
    [Authorize(Roles = Roles.User + "," + Roles.Admin)]

    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> ListOfAllProjects(string name)
        {
            var projects = await _userService.GetAllProjects();
            if (name != null)
            {
                projects = await _userService.GetAllSearchedProjects(name);
                return View(projects);
            }
            return View(projects);
        }

        [HttpGet]
        public async Task<IActionResult> ListOfAllTasksFromProject(int projectId)
        {
            ViewBag.currentProjectId = projectId;
            var tasks = await _userService.GetAllTasksFromProject(projectId);
            return View(tasks);
        }

        [HttpGet]
        public async Task<IActionResult> TaskInformation(int id)
        {
            var task = await _userService.GetTaskById(id);
            ViewBag.user = await _userService.GetUserById(task.UserId);
            ViewBag.comments = await _userService.GetComments(task.Id);
            return View(task);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status, int currentUser)
        {
            await _userService.UpdateStatus(id, status, currentUser);
            return RedirectToAction("TaskInformation", "User", new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(string text, int userId, int taskId)
        {
            if(text != null)
            {
                await _userService.AddComment(text, userId, taskId);
            }
            return RedirectToAction("TaskInformation", "User", new { id = taskId });
        }
    }
}
