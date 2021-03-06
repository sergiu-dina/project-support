﻿using System;
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
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public DbSet<Resources> Resources { get; set; }
        public DbSet<GanttTaskRelation> GanttTaskRelations { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatUser> ChatUsers { get; set; }
        public DbSet<Message> Messages { get; set; }

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

            builder.Entity<Resources>()
                .HasKey(r => new { r.UserId, r.TaskId });
            builder.Entity<Resources>()
                .HasOne(r => r.User)
                .WithMany(u => u.Resources)
                .HasForeignKey(r => r.UserId);
            builder.Entity<Resources>()
                .HasOne(r => r.Task)
                .WithMany(t => t.Resources)
                .HasForeignKey(r => r.TaskId);

            builder.Entity<GanttTaskRelation>()
                .HasKey(gtr=>new { gtr.GanttTaskId, gtr.RelatedTaskId});
            builder.Entity<GanttTask>()
                .HasMany(gt => gt.GanttTaskRelations)
                .WithOne(gtr => gtr.GanttTask)
                .HasForeignKey(gt => gt.GanttTaskId);
            builder.Entity<GanttTask>()
                .HasMany(gt => gt.GanttTaskRelationsOf)
                .WithOne(gtr => gtr.RelatedGanttTask)
                .HasForeignKey(gt => gt.RelatedTaskId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ChatUser>()
                .HasKey(pu => new { pu.UserId, pu.ChatId });
            builder.Entity<ChatUser>()
                .HasOne(pu => pu.User)
                .WithMany(m => m.ChatUsers)
                .HasForeignKey(pu => pu.UserId);
            builder.Entity<ChatUser>()
                .HasOne(pu => pu.Chat)
                .WithMany(p => p.ChatUsers)
                .HasForeignKey(pu => pu.ChatId);

            base.OnModelCreating(builder);
        }
    }
}
