namespace taskmanagementAPI.Data
{
    using Microsoft.EntityFrameworkCore;
    using taskmanagementAPI.Models;

    public class TaskManagementContext : DbContext
    {
        public TaskManagementContext(DbContextOptions<TaskManagementContext> options)
        : base(options)
        {
        }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<User> Users { get; set; } //user pass for auth
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        //primary keys
        modelBuilder.Entity<Employee>()
            .HasKey(e => e.EmployeeID);

        modelBuilder.Entity<Project>()
            .HasKey(p => p.ProjectID);

        modelBuilder.Entity<Task>()
            .HasKey(t => t.TaskID);

        // foregin keys config
        // (not really used at the moment but will in Lab3
        // + error in execution if not config)
        modelBuilder.Entity<Project>()
            .HasOne(p => p.ProjectManager)
            .WithMany(e => e.ManagedProjects)
            .HasForeignKey(p => p.ProjectManagerID);



        modelBuilder.Entity<Task>()
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectID);



        modelBuilder.Entity<Task>()
            .HasOne(t => t.AssignedEmployee)
            .WithMany(e => e.AssignedTasks)
            .HasForeignKey(t => t.AssignedTo);
        }

    }
}
