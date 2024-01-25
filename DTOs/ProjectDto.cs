public class ProjectDto
{
    public int ProjectID { get; set; }  
    public string ProjectName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }     
    public DateTime? EndDate { get; set; }
    public int? ProjectManagerID { get; set; }
    public List<int> TaskIDs { get; set; } //ids associated tasks

    public ProjectDto()
    {
        TaskIDs = new List<int>();
    }
}
