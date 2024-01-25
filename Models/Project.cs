using System.ComponentModel.DataAnnotations.Schema;


//the data anotations were applied to improve the comprehension
//by making the writings clear in all tables

namespace taskmanagementAPI.Models
{
    [Table("projects")]
    public class Project
    {
        [Column("projectid")]
        public int ProjectID { get; set; }

        [Column("projectname")]
        public string ProjectName { get; set; }

        [Column("startdate")]
        public DateTime StartDate { get; set; }

        [Column("enddate")]
        public DateTime? EndDate { get; set; }

        [Column("projectmanagerid")]
        public int? ProjectManagerID { get; set; }


        public Employee ProjectManager { get; set; }
        public ICollection<Task> Tasks { get; set; }
    }
}
