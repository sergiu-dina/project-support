using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectSupport.Areas.Identity.Data;
using ProjectSupport.Models;

namespace ProjectSupport.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<GanttTask> GanttTasks { get; set; }
        public DbSet<Link> Links { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ProjectUser>()
                .HasKey(pu => new { pu.UserId, pu.ProjectId });
            builder.Entity<ProjectUser>()
                .HasOne(pu => pu.User)
                .WithMany(m => m.ProjectUsers)
                .HasForeignKey(pu => pu.UserId);
            builder.Entity<ProjectUser>()
                .HasOne(pu => pu.Project)
                .WithMany(p => p.ProjectUsers)
                .HasForeignKey(pu => pu.ProjectId);

            base.OnModelCreating(builder);
        }
    }
}
