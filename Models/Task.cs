using System.ComponentModel.DataAnnotations.Schema;

//the data anotations were applied to improve the comprehension
//by making the writings clear in all tables

namespace taskmanagementAPI.Models
{
    [Table("tasks")]
    public class Task
    {
        [Column("taskid")]
        public int TaskID { get; set; }

        [Column("projectid")]
        public int ProjectID { get; set; }

        [Column("taskname")]
        public string TaskName { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("assignedto")]
        public int? AssignedTo { get; set; }

        [Column("startdate")]
        public DateTime StartDate { get; set; }

        [Column("enddate")]
        public DateTime? EndDate { get; set; }

        [Column("status")]
        public string Status { get; set; }

        //foregin keys
        public Project Project { get; set; }
        public Employee AssignedEmployee { get; set; }
    }
}

