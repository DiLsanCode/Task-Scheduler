using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskApp.Business.dto;
using TaskList.Business.Constants;
using TaskList.Data.Models;

namespace TaskApp.Business.Interfaces
{
    public interface IAdminService
    {
        Task<Project> AddNewProject(dtoProject project);
        Task<Assignment> AddNewTask(dtoTask task);
        Task<dtoProject> GetProjectById(int id);
        Task<dtoTask> GetTaskById(int id);
        Task DeleteProject(int id);
        Task EditProject(int id, dtoProject project);
        Task<IEnumerable<dtoUser>> GetListOfUsers();
        Task<IEnumerable<dtoProject>> GetListOfProjects();
        List<dtoStatus> StatusList();
        Task DeleteTask(int id);
        Task EditTask(int id, dtoTask task);
        Task<IEnumerable<dtoTask>> GetInReviewTask();
    }
}
