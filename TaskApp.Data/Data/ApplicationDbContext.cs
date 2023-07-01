using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TaskApp.Data.Models;
using TaskList.Data.Models;

namespace TaskList.Data.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, int>
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Assignment> Tasks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Sprint> Sprints { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Comment>()
                .HasOne(c => c.Assignment)
                .WithMany(c => c.Comments)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Sprint>()
                .HasOne(c => c.Project)
                .WithMany(c => c.Sprints)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Assignment>()
                .HasOne(c => c.Sprint)
                .WithMany(c => c.Tasks)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Sprint>()
                .HasMany(c => c.Tasks)
                .WithOne(c => c.Sprint)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Assignment>()
                .HasMany(c => c.Comments)
                .WithOne(c => c.Assignment)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Project>()
                .HasMany(c => c.Sprints)
                .WithOne(c => c.Project)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(builder);
        }

    }
}
