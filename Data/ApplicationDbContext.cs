using Microsoft.EntityFrameworkCore;
using MSU_BARODA.Models;

namespace MSU_BARODA.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Database Tables
        public DbSet<Student> Students { get; set; }                         // Students Table
        public DbSet<Admin> Admins { get; set; }                             // Admins Table
        public DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; } // Assignments Table

        public DbSet<Subject> Subjects { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Unique Constraints for Student
            builder.Entity<Student>()
                   .HasIndex(s => s.PRN)
                   .IsUnique();

            builder.Entity<Student>()
                   .HasIndex(s => s.Email)
                   .IsUnique();

            // Unique Constraint for Admin
            builder.Entity<Admin>()
                   .HasIndex(a => a.MailId)
                   .IsUnique();
        }
    }
}
