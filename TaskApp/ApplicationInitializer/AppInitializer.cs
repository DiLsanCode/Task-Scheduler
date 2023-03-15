using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;
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
            //await SeedProjectsAsync();
            //await SeedTasksAsync();
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
            if (await _dbContext.Projects.AnyAsync())
            {
                return;
            }

            for (int i = 1; i < 7; i++)
            {
                await _dbContext.Projects.AddAsync(new Project()
                {
                    Name = $"Project {i}",
                });
            }
            await _dbContext.SaveChangesAsync();
        }

        private async Task SeedTasksAsync()
        {
            if (await _dbContext.Tasks.AnyAsync())
            {
                return;
            }

            foreach (Project project in _dbContext.Projects)
            {
                for (int i = 1; i <= 3; i++)
                {
                    await _dbContext.Tasks.AddAsync(new Assignment()
                    {
                        Name = $"Task {i}",
                        UserId = 1,
                        ProjectId = project.Id,
                        Description = $"Description {i}",
                        Status = Status.ToDo.ToString(),
                        DateStart = DateTime.Now,
                        DateEnd = DateTime.Now.AddDays(14),
                    });
                }
            }
        }

        private async Task SeedUsersAsync()
        {
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

        }
    }
}
