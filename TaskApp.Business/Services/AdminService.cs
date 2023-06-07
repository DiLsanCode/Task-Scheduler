using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskApp.Business.dto;
using TaskApp.Business.Interfaces;
using TaskApp.Data.Models;
using TaskList.Business.Constants;
using TaskList.Data.Data;
using TaskList.Data.Models;

namespace TaskApp.Business.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _dbContext;

        public AdminService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Project> AddNewProject(dtoProject project)
        {
            var existringProject = await _dbContext.Projects
                .FirstOrDefaultAsync(x => x.Name == project.Name);

            if(existringProject == null)
            {
                var newProject = await _dbContext.AddAsync(new Project()
                {
                    Name = project.Name,
                });

                await _dbContext.SaveChangesAsync();
                return newProject.Entity;
            }
            throw new Exception("Project with that name already exists!");
        }

        public async Task<Assignment> AddNewTask(dtoTask task)
        {
            if (task.ProjectId.Equals(0))
            {
                throw new Exception("Project was not selected");
            }
            var newTask = await _dbContext.AddAsync(new Assignment()
            {
                Name = task.Name,
                Description = task.Description,
                Status = task.Status,
                UserId = task.UserId,
                ProjectId = task.ProjectId,
                DateStart = task.DateStart,
                DateEnd = task.DateEnd,
            });

            await _dbContext.SaveChangesAsync();
            return newTask.Entity;
        }

        public async Task<dtoProject> GetProjectById(int id)
        {
            var project = await _dbContext.Projects
                .Select(x => new dtoProject
                {
                    Id = x.Id,
                    Name = x.Name,
                }).FirstOrDefaultAsync(x => x.Id == id);

            if (project == null)
            {
                throw new Exception("Project Not Found!");
            }
            return project;
        }

        public async Task<dtoProject> GetProjectByName(string name)
        {
            var project = await _dbContext.Projects
                .Select(x => new dtoProject
                {
                    Id = x.Id,
                    Name = x.Name,
                }).FirstOrDefaultAsync(x => x.Name == name);

            return project;
        }

        public async Task DeleteProject(int id)
        {
            var proojectToRemove = await _dbContext.Projects
                .FirstOrDefaultAsync(x => x.Id == id);

            var tasks = await _dbContext.Tasks
                .Include(c => c.Comments)
                .Where(c => c.ProjectId == id)
                .ToListAsync();

            if (proojectToRemove == null)
            {
                throw new Exception("Participant Not Found");
            }

            foreach (var task in tasks)
            {
                _dbContext.Comments.RemoveRange(task.Comments);
            }
            await _dbContext.SaveChangesAsync();

            _dbContext.Projects.Remove(proojectToRemove);
            await _dbContext.SaveChangesAsync();
        }

        public async Task EditProject(int id, dtoProject project)
        {
            var projectToEdit = await _dbContext.Projects
               .FirstOrDefaultAsync(x => x.Id == id);
            var projectWithSimilarName = await _dbContext.Projects
                .FirstOrDefaultAsync(x => x.Name == project.Name);

            if (projectToEdit != null)
            {
                if(projectWithSimilarName == null)
                {
                    projectToEdit.Name = project.Name;

                    _dbContext.Projects.Update(projectToEdit);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("There is project with that name!");
                }
                
            }
            else
            {
                throw new Exception("Project Not Found");
            }
        }

        public async Task<IEnumerable<dtoUser>> GetListOfUsers()
        {
            var users = await _dbContext.Users
                .Select(x => new dtoUser
                {
                    Id = x.Id,
                    Name = x.FirstName + " " + x.MiddleName + " " + x.LastName,
                })
                .ToListAsync();

            return users;
        }

        public async Task<IEnumerable<dtoProject>> GetListOfProjects()
        {
            var projects = await _dbContext.Projects
                .Select(x => new dtoProject
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .ToListAsync();

            return projects;
        }

        public List<dtoStatus> StatusList()
        {
            List<dtoStatus> enums = ((Status[])Enum.GetValues(typeof(Status)))
                .Select(c => new dtoStatus()
                {
                    Id = (int)c,
                    Name = c.ToString()
                }).ToList();

            return enums;
        }

        public async Task DeleteTask(int id)
        {
            var taskToRemove = await _dbContext.Tasks
               .FirstOrDefaultAsync(x => x.Id == id);

            var commentForEdit = await _dbContext.Comments
                .Where(c => c.AssignmentId == taskToRemove.Id)
            .ToListAsync();

            if (taskToRemove == null)
            {
                throw new Exception("Task Not Found");
            }

            _dbContext.Comments.RemoveRange(commentForEdit);
            _dbContext.Tasks.Remove(taskToRemove);
            await _dbContext.SaveChangesAsync();
        }

        public async Task EditTask(int id, dtoTask task)
        {
            var taskToEdit = await _dbContext.Tasks
               .FirstOrDefaultAsync(x => x.Id == id);

            if (taskToEdit != null)
            {
                taskToEdit.Name = task.Name;
                taskToEdit.UserId= task.UserId;
                taskToEdit.ProjectId= task.ProjectId;
                taskToEdit.Status = task.Status;
                taskToEdit.Description = task.Description;
                taskToEdit.DateStart= task.DateStart;
                taskToEdit.DateEnd = task.DateEnd;

                _dbContext.Tasks.Update(taskToEdit);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Project Not Found");
            }
        }

        public async Task<dtoTask> GetTaskById(int id)
        {
            var task = await _dbContext.Tasks
                .Select(x => new dtoTask
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    ProjectId = x.ProjectId,
                    UserId = x.UserId,
                    Status = x.Status,
                    DateStart = x.DateStart,
                    DateEnd = x.DateEnd,
                }).FirstOrDefaultAsync(x => x.Id == id);

            if (task == null)
            {
                throw new Exception("Project Not Found!");
            }
            return task;
        }

        public async Task<IEnumerable<dtoTask>> GetInReviewTask()
        {
            var tasks = await _dbContext.Tasks
                .Select(p => new dtoTask
                {
                    Id = p.Id,
                    Name = p.Name,
                    ProjectId = p.ProjectId,
                    Description = p.Description,
                    UserId = p.UserId,
                    Status = p.Status,
                    userName = _dbContext.Users
                    .Select(x => new dtoUser
                    {
                        Id = x.Id,
                        Name = x.FirstName + " " + x.LastName
                    }).FirstOrDefault(x => x.Id == p.UserId)
                }).Where(p => p.Status == Status.InReview.ToString())
                .ToListAsync();

            return tasks;
        }
    }
}
