using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskApp.Business.Constants;
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

            if (existringProject == null)
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
            if (task.SprintId.Equals(0))
            {
                throw new Exception("Sprint was not selected! Please select a sprint from the desired project!");
            }
            var newTask = await _dbContext.AddAsync(new Assignment()
            {
                Name = task.Name,
                Description = task.Description,
                Status = task.Status,
                UserId = task.UserId,
                Score = task.Score,
                SprintId = task.SprintId,
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

        public async Task<dtoSprint> GetSprintById(int id)
        {
            var sprint = await _dbContext.Sprints
                .Select(x => new dtoSprint
                {
                    Id = x.Id,
                    Name = x.Name,
                    ProjectId = x.ProjectId,
                }).FirstOrDefaultAsync(x => x.Id == id);

            if (sprint == null)
            {
                throw new Exception("Sprint Not Found!");
            }
            return sprint;
        }

        public async Task<bool> IsProjectNameTaken(string name)
        {
            var project = await _dbContext.Projects
                .Select(x => new dtoProject
                {
                    Id = x.Id,
                    Name = x.Name,
                }).FirstOrDefaultAsync(x => x.Name == name);

            if (project == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<bool> IsSprintNameTaken(string name)
        {
            var sprint = await _dbContext.Sprints
                .Select(x => new dtoSprint
                {
                    Id = x.Id,
                    Name = x.Name,
                }).FirstOrDefaultAsync(x => x.Name == name);

            if(sprint == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<bool> IsTaskNameTaken(string name)
        {
            var task = await _dbContext.Tasks
                .Select(x => new dtoTask
                {
                    Id = x.Id,
                    Name = x.Name,
                }).FirstOrDefaultAsync(x => x.Name == name);

            if (task == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task DeleteProject(int id)
        {
            var projectToRemove = await _dbContext.Projects
                .FirstOrDefaultAsync(x => x.Id == id);
            if (projectToRemove == null)
            {
                throw new Exception("Project Not Found");
            }

            var sprints = await _dbContext.Sprints
                .Include(x => x.Tasks)
                .Where(x => x.ProjectId == id)
                .ToListAsync();

            List<Assignment> tasks = new List<Assignment>();
            foreach (var sprint in sprints)
            {
                tasks = await _dbContext.Tasks
                .Include(c => c.Comments)
                .Where(c => c.SprintId == sprint.Id)
                .ToListAsync();
            }

            foreach (var task in tasks)
            {
                _dbContext.Comments.RemoveRange(task.Comments);
            }
            await _dbContext.SaveChangesAsync();

            foreach (var sprint in sprints)
            {
                _dbContext.Tasks.RemoveRange(sprint.Tasks);
            }
            await _dbContext.SaveChangesAsync();

            _dbContext.Projects.Remove(projectToRemove);
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
                if (projectWithSimilarName == null)
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

        public async Task<IEnumerable<dtoSprint>> GetListOfSprints()
        {
            var sprints = await _dbContext.Sprints
                .Select(x => new dtoSprint
                {
                    Id = x.Id,
                    Name = x.Name,
                    ProjectId = x.ProjectId,
                })
                .ToListAsync();

            return sprints;
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
                taskToEdit.UserId = task.UserId;
                taskToEdit.SprintId = task.SprintId;
                taskToEdit.Status = task.Status;
                taskToEdit.Score = task.Score;
                taskToEdit.Description = task.Description;
                taskToEdit.DateStart = task.DateStart;
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
                    SprintId = x.SprintId,
                    UserId = x.UserId,
                    Score = x.Score,
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
                    SprintId = p.SprintId,
                    Description = p.Description,
                    UserId = p.UserId,
                    Score = p.Score,
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

        public async Task<Sprint> AddNewSprint(dtoSprint sprint)
        {
            var existringSprint = await _dbContext.Sprints
                .FirstOrDefaultAsync(x => x.Name == sprint.Name);

            if (existringSprint == null)
            {
                var newSprint = await _dbContext.AddAsync(new Sprint()
                {
                    Name = sprint.Name,
                    ProjectId = sprint.ProjectId,

                });

                await _dbContext.SaveChangesAsync();
                return newSprint.Entity;
            }
            throw new Exception("Sprint with that name already exists!");
        }

        public async Task EditSprint(int id, dtoSprint sprint)
        {
            var sprintToEdit = await _dbContext.Sprints
               .FirstOrDefaultAsync(x => x.Id == id);
            var sprintWithSimilarName = await _dbContext.Sprints
                .FirstOrDefaultAsync(x => x.Name == sprint.Name);

            if (sprintToEdit != null)
            {
                if (sprintWithSimilarName == null)
                {
                    sprintToEdit.Name = sprint.Name;
                    sprintToEdit.ProjectId = sprint.ProjectId;

                    _dbContext.Sprints.Update(sprintToEdit);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("There is a sprint with that name!");
                }

            }
            else
            {
                throw new Exception("Sprint Not Found");
            }
        }

        public async Task DeleteSprint(int id)
        {
            var sprintToRemove = await _dbContext.Sprints
                .FirstOrDefaultAsync(x => x.Id == id);
            if (sprintToRemove == null)
            {
                throw new Exception("Sprint Not Found");
            }

            var tasks = await _dbContext.Tasks
            .Include(c => c.Comments)
            .Where(c => c.SprintId == id)
            .ToListAsync();

            foreach (var task in tasks)
            {
                _dbContext.Comments.RemoveRange(task.Comments);
            }
            await _dbContext.SaveChangesAsync();

            _dbContext.Sprints.Remove(sprintToRemove);
            await _dbContext.SaveChangesAsync();
        }

        public List<string> GetFibunacciList()
        {
            return FibonacciNumbers.GetList();
        }
    }
}
