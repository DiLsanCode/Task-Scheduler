using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;
using TaskApp.Business.Constants;
using TaskApp.Data.Models;
using TaskList.Business.Constants;
using TaskList.Data.Data;
using TaskList.Data.Models;

namespace VotingApp.ApplicationInitializer
{
    public class AppInitializer
    {
        private readonly ApplicationDbContext _dbContext;
        private UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IConfiguration _config;

        public AppInitializer(ApplicationDbContext dbContext,
                                UserManager<User> userManager,
                                RoleManager<Role> roleManager,
                                IConfiguration config)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
            _config = config;
        }

        public async Task InitializeAsync()
        {
            await ApplyMigrationsAsync();
            await SeedRolesAsync();
            await SeedUsersAsync();
            await SeedProjectsAsync();
            await SeedSprints();
            await SeedTasksAsync();
        }

        private async Task ApplyMigrationsAsync()
        {
            var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                await _dbContext.Database.MigrateAsync();
            }
        }

        private async Task SeedRolesAsync()
        {
            if (await _dbContext.Roles.AnyAsync())
            {
                return;
            }

            await _roleManager.CreateAsync(new Role { Name = Roles.Admin });
            await _roleManager.CreateAsync(new Role { Name = Roles.User });
        }

        private async Task SeedProjectsAsync()
        {
            string[] projectNames = { "SLOW CARE",
                "POSSESSIVE NATION",
                "BOUNCY INK",
                "POETIC CREAM",
                "GUARDED ROOT",
                "DOMINEERING PORTER",
                "GUARDED AUTHORITY",
                "OBSOLETE MUSCLE",
                "REDUNDANT CARRIAGE",
                "LUMPY ROBIN" };

            if (await _dbContext.Projects.AnyAsync())
            {
                return;
            }

            for (int i = 1; i < 10; i++)
            {
                await _dbContext.Projects.AddAsync(new Project()
                {
                    Name = $"{projectNames[i]}",
                });
            }
            await _dbContext.SaveChangesAsync();
        }

        private async Task SeedSprints()
        {
            if (await _dbContext.Sprints.AnyAsync())
            {
                return;
            }

            foreach(var project in _dbContext.Projects)
            {
                for (int i = 1; i < 10; i++)
                {
                    await _dbContext.Sprints.AddAsync(new Sprint()
                    {
                        Name = $"{project.Name} Sprint {i}",
                        ProjectId = project.Id,
                    });
                }

                await _dbContext.Sprints.AddAsync(new Sprint()
                {
                    Name = $"{project.Name} Buffer Sprint",
                    ProjectId = project.Id,
                });
            }
            await _dbContext.SaveChangesAsync();
        }

        private async Task SeedTasksAsync()
        {
            Random random = new Random();
            string[] taskNames = { "Convert Age to Days",
            "Return the Sum of Two Numbers",
            "Convert Minutes into Seconds",
            "Return the Next Number from the Integer Passed",
            "Area of a Triangle"
            };
            string[] taskDescriptions = { "Create a function that takes the age in years and returns the age in days.",
            "Create a function that takes two numbers as arguments and returns their sum.",
            "Write a function that takes an integer minutes and converts it to seconds.",
            "Create a function that takes a number as an argument, increments the number by +1 and returns the result.",
            "Write a function that takes the base and height of a triangle and return its area."
            };

            if (await _dbContext.Tasks.AnyAsync())
            {
                return;
            }

            foreach (var sprint in _dbContext.Sprints)
            {
                if (!sprint.Name.Contains("Buffer"))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        await _dbContext.Tasks.AddAsync(new Assignment()
                        {
                            Name = $"{taskNames[i]}",
                            UserId = random.Next(3, 8),
                            SprintId = sprint.Id,
                            Description = $"{taskDescriptions[i]}",
                            Score = FibonacciNumbers.GetList()[random.Next(0, 10)],
                            Status = Status.ToDo.ToString(),
                            DateStart = DateTime.Now,
                            DateEnd = DateTime.Now.AddDays(14),
                        });
                    }
                }
                
            }
            await _dbContext.SaveChangesAsync();
        }

        private async Task SeedUsersAsync()
        {
            string[] firstName = { "Антон", "Алекса", "Звездиян", "Добринка", "Спартак", "Николет" };
            string[] middleName = { "Николов", "Георгиева", "Божинов", "Каменова", "Андреев", "Попова" };
            string[] lastName = { "Иванов", "Арнаудова", "Събев", "Михайлова", "Пешев", "Иванова" };

            if (await _dbContext.Users.AnyAsync())
            {
                return;
            }

            var adminUser = new User
            {
                Email = "admin@admin.com",
                FirstName = "Admin",
                MiddleName = "Admin",
                LastName = "Admin",
                EmailConfirmed = true,
                UserName = "admin@admin.com",
            };

            await _userManager.CreateAsync(adminUser, "12345dD!");
            await _userManager.AddToRoleAsync(adminUser, Roles.Admin);

                var user = new User
            {
                Email = "user@user.com",
                FirstName = "User",
                MiddleName = "User",
                LastName = "User",
                EmailConfirmed = true,
                UserName = "user@user.com",
            };

            await _userManager.CreateAsync(user, "12345dD!");
            await _userManager.AddToRoleAsync(user, Roles.User);

            for (int i = 0; i < 6; i++)
            {
                var users = new User
                {
                    Email = $"user{i}@user.com",
                    FirstName = $"{firstName[i]}",
                    MiddleName = $"{middleName[i]}",
                    LastName = $"{lastName[i]}",
                    EmailConfirmed = true,
                    UserName = $"user{i}@user.com",
                };
                await _userManager.CreateAsync(users, "12345dD!");
                await _userManager.AddToRoleAsync(users, Roles.User);
            }

        }
    }
}
