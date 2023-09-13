using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TaskApp.Business.dto;
using TaskApp.Business.Interfaces;
using TaskApp.Data.Models;
using TaskList.Business.Constants;
using TaskList.Data.Data;
using TaskList.Data.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskApp.Business.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<dtoProject>> GetAllProjects()
        {
            var projects = await _dbContext.Projects
                .Select(p => new dtoProject
                {
                    Id = p.Id,
                    Name = p.Name,
                }).ToListAsync();
            return projects;
        }

        public async Task<IEnumerable<dtoSprint>> GetAllSprints(int projectId)
        {
            var sprints = await _dbContext.Sprints
                .Select(x => new dtoSprint
                { 
                    Id = x.Id,
                    Name = x.Name,
                    ProjectId = x.ProjectId,
                }).Where(c => c.ProjectId == projectId)
                .ToListAsync();
            return sprints;
        }

        public async Task<IEnumerable<dtoTask>> GetAllTasksFromProject(int sprintId, int userId)
        {
            if(userId == 1)
            {
                var tasks = await _dbContext.Tasks
                .Select(p => new dtoTask
                {
                    Id = p.Id,
                    Name = p.Name,
                    SprintId = p.SprintId,
                    Description = p.Description,
                    Score = p.Score,
                    UserId = p.UserId,
                    Status = p.Status,
                    DateStart = p.DateStart,
                    DateEnd = p.DateEnd,
                    userName = _dbContext.Users
                    .Select(x => new dtoUser
                    {
                        Id = x.Id,
                        Name = x.FirstName + " " + x.LastName
                    }).FirstOrDefault(x => x.Id == p.UserId)
                }).Where(c => c.SprintId == sprintId)
                .OrderBy(x => x.Name)
                .ToListAsync();

                return tasks;
            }
            else
            {
                var tasks = await _dbContext.Tasks
                .Select(p => new dtoTask
                {
                    Id = p.Id,
                    Name = p.Name,
                    SprintId = p.SprintId,
                    Description = p.Description,
                    Score = p.Score,
                    UserId = p.UserId,
                    Status = p.Status,
                    DateStart = p.DateStart,
                    DateEnd = p.DateEnd,
                    userName = _dbContext.Users
                    .Select(x => new dtoUser
                    {
                        Id = x.Id,
                        Name = x.FirstName + " " + x.LastName
                    }).FirstOrDefault(x => x.Id == p.UserId)
                }).Where(c => c.SprintId == sprintId && c.UserId == userId)
                .OrderBy(x => x.Name)
                .ToListAsync();

                return tasks;
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
                    Status = x.Status,
                    Score = x.Score,
                    UserId = x.UserId,
                    SprintId = x.SprintId,
                    DateStart = x.DateStart,
                    DateEnd = x.DateEnd,
                }).FirstOrDefaultAsync(x => x.Id == id);

            if (task == null)
            {
                throw new Exception("Task Not Found!");
            }
            return task;
        }

        public async Task UpdateStatus(int id, string status, int currentUser)
        {
            if(currentUser == 0 || currentUser == null)
            {
                throw new Exception("You are not logged!");
            }
            else
            {
                var taskStatusToEdit = await _dbContext.Tasks
               .FirstOrDefaultAsync(x => x.Id == id);

                if (taskStatusToEdit != null)
                {
                    taskStatusToEdit.Status = status;

                    if(taskStatusToEdit.UserId == currentUser || currentUser == 1)
                    {
                        var updatedOrder = _dbContext.Tasks.Update(taskStatusToEdit);
                        await _dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception("You can only change your task's status!");
                    }
                }
                else
                {
                    throw new Exception("Task Not Found");
                }
            }
        }

        public async Task<IEnumerable<dtoProject>> GetAllSearchedProjects(string name)
        {
            var projects = await _dbContext.Projects
                .Select(p => new dtoProject
                {
                    Id = p.Id,
                    Name = p.Name,
                }).Where(x => x.Name.Contains(name))
                .ToListAsync();

            return projects;
        }

        public async Task<dtoUser> GetUserById(int id)
        {
            var user = await _dbContext.Users
                .Select(x => new dtoUser
                {
                    Id = x.Id,
                    Name = x.FirstName + " " + x.LastName,
                }).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                throw new Exception("User Not Found!");
            }
            return user;
        }

        public dtoStatus GetStatusByName(string id)
        {
            List<dtoStatus> enums = ((Status[])Enum.GetValues(typeof(Status)))
                .Select(c => new dtoStatus()
                {
                    Id = (int)c,
                    Name = c.ToString()
                }).ToList();

            var status = enums
                .Where(x => x.Name == id)
                .FirstOrDefault();
            return status;
        }

        public async Task<Comment> AddComment(string text, int userId, int taskId)
        {
            var newComment = await _dbContext.AddAsync(new Comment()
            {
                Text = text,
                UserId = userId,
                AssignmentId = taskId,
            });

            await _dbContext.SaveChangesAsync();
            return newComment.Entity;
        }

        public async Task<IEnumerable<dtoComment>> GetComments(int id)
        {
            var comments = await _dbContext.Comments
                .Select(p => new dtoComment
                {
                    Text = p.Text,
                    UserId = p.UserId,
                    TaskId = p.AssignmentId,
                    UserName = p.User.FirstName + " " + p.User.LastName,
                })
                .Where(c => c.TaskId == id)
                .ToListAsync();

            return comments;
        }

        public async Task<int> GetPercentageOfDoneWork(int sprintId, int userId)
        {
            var combinedScoreList = await finalScore(sprintId, userId);
            var combinedScoreOfDoneList = await doneScore(sprintId, userId);
            
            var percentage = (int)Math.Round((double)(100 * combinedScoreOfDoneList) / combinedScoreList);

            return percentage;
        }

        public async Task<double> doneScore(int sprintId, int userId)
        {
            if(userId == 1)
            {
                var combinedScoreOfDoneList = await _dbContext.Tasks
                .Where(x => x.SprintId == sprintId && x.Status.Contains("Done"))
                .Select(p => double.Parse(p.Score))
                .ToListAsync();

                double combineDoneScore = 0;
                foreach (var scoreOfDoneTask in combinedScoreOfDoneList)
                {
                    combineDoneScore += scoreOfDoneTask;
                }
                return combineDoneScore;
            }
            else
            {
                var combinedScoreOfDoneList = await _dbContext.Tasks
                .Where(x => x.SprintId == sprintId && x.UserId == userId && x.Status.Contains("Done"))
                .Select(p => double.Parse(p.Score))
                .ToListAsync();

                double combineDoneScore = 0;
                foreach (var scoreOfDoneTask in combinedScoreOfDoneList)
                {
                    combineDoneScore += scoreOfDoneTask;
                }
                return combineDoneScore;
            }
        }

        public async Task<double> finalScore(int sprintId, int userId)
        {
            if(userId == 1)
            {
                var combinedScoreList = await _dbContext.Tasks
                .Where(x => x.SprintId == sprintId)
                .Select(p => double.Parse(p.Score))
                .ToListAsync();

                double combinedScore = 0;
                foreach (var score in combinedScoreList)
                {
                    combinedScore += score;
                }
                return combinedScore;
            }
            else
            {
                var combinedScoreList = await _dbContext.Tasks
                                .Where(x => x.SprintId == sprintId && x.UserId == userId)
                                .Select(p => double.Parse(p.Score))
                                .ToListAsync();

                double combinedScore = 0;
                foreach (var score in combinedScoreList)
                {
                    combinedScore += score;
                }
                return combinedScore;
            }
            
        }
    }
}
