using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseMigration.Model
{
    class DrowsinessDetectionContext : DbContext
    {
        public DrowsinessDetectionContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<EmergencyContact> EmergencyContacts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserEmergencyContact>()
                .HasKey(ue => new { ue.UserId, ue.EmergencyContactId });
            builder.Entity<UserEmergencyContact>()
                .HasOne(ue => ue.User)
                .WithMany(ue => ue.EmergencyContacts)
                .HasForeignKey(ue => ue.UserId);
            builder.Entity<UserEmergencyContact>()
                .HasOne(ue => ue.EmergencyContact)
                .WithMany(ue => ue.Users)
                .HasForeignKey(ue => ue.EmergencyContactId);
        }

    }
}
