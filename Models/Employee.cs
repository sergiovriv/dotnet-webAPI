using System.ComponentModel.DataAnnotations.Schema;

//the data anotations were applied to improve the comprehension
//by making the writings clear in all tables

namespace taskmanagementAPI.Models
{
    [Table("employees")]
    public class Employee
    {
        [Column("employeeid")]
        public int EmployeeID { get; set; }

        [Column("firstname")]
        public string FirstName { get; set; }

        [Column("lastname")]
        public string LastName { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("department")]
        public string Department { get; set; }

        //foregin keys
        public ICollection<Task> AssignedTasks { get; set; }
        public ICollection<Project> ManagedProjects { get; set; }
    }
}
