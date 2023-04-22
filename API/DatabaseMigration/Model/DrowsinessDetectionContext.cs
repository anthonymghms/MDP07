using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using DatabaseMigration.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace DatabaseMigration
{
    public class DrowsinessDetectionContext : IdentityDbContext<AppUser>
    {
        public DrowsinessDetectionContext(DbContextOptions options) : base(options) { }

        //public DbSet<User> Users { get; set; }
        public DbSet<EmergencyContact> EmergencyContact { get; set; }
        public DbSet<UserConfig> UserConfig { get; set; }
        public DbSet<UserAlert> UserAlert { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<EmergencyContact>()
                .HasKey(ec => ec.Id);
            builder.Entity<EmergencyContact>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<UserConfig>()
                .HasKey(ec => ec.Id);
            builder.Entity<UserConfig>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<UserAlert>()
                .HasKey(ec => ec.Id);
            builder.Entity<UserAlert>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<AlertMessage>()
                .HasOne(e => e.User)
                .WithMany(d => d.AlertMessages)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            base.OnModelCreating(builder);
            this.SeedRoles(builder);
        }
        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole()
                {
                    Name = "Admin",
                    ConcurrencyStamp = "1",
                    NormalizedName = "Admin"
                },
                new IdentityRole()
                {
                    Name = "User",
                    ConcurrencyStamp = "2",
                    NormalizedName = "User"
                }
            );
        }
    }
}
