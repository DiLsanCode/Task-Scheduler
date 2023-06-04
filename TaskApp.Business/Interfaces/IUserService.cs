using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskApp.Business.dto;
using TaskApp.Data.Models;
using TaskList.Data.Models;

namespace TaskApp.Business.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<dtoProject>> GetAllProjects();
        Task<IEnumerable<dtoTask>> GetAllTasksFromProject(int projectId);
        Task<dtoTask> GetTaskById(int id);
        Task UpdateStatus(int id, string status, int currentUser);
        Task<IEnumerable<dtoProject>> GetAllSearchedProjects(string name);
        Task<dtoUser> GetUserById(int id);
        dtoStatus GetStatusByName(string id);
        Task<Comment> AddComment(string text, int userId, int taskId);
        Task<IEnumerable<dtoComment>> GetComments(int id);
    }
}
