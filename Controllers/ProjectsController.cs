using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using taskmanagementAPI.Data;
using taskmanagementAPI.Models;

[Route("api/[controller]")]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly TaskManagementContext _context;

    public ProjectsController(TaskManagementContext context)
    {
        _context = context;
    }

    // GET: api/projects/{id}
    // specific project info by id, includes taksids and coordinator
    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDto>> GetProject(int id)
    {
        var project = await _context.Projects
            .Where(p => p.ProjectID == id)
            .Select(p => new ProjectDto
            {
                ProjectID = p.ProjectID,
                ProjectName = p.ProjectName,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                ProjectManagerID = p.ProjectManagerID,
                TaskIDs = p.Tasks.Select(t => t.TaskID).ToList()
            })
            .FirstOrDefaultAsync();

        if (project == null)
        {
            return NotFound();
        }

        return Ok(project);
    }

    // GET: api/projects
    // all projects info
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAllProjects()
    {
        var projects = await _context.Projects
            .Select(p => new ProjectDto
            {
                ProjectID = p.ProjectID,
                ProjectName = p.ProjectName,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                ProjectManagerID = p.ProjectManagerID,
                TaskIDs = p.Tasks.Select(t => t.TaskID).ToList()
            })
            .ToListAsync();

        return Ok(projects);
    }

    // POST: api/projects
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ProjectDto>> CreateProject(ProjectDto projectDto)
    {
        var project = new Project
        {
            ProjectName = projectDto.ProjectName,
            StartDate = projectDto.StartDate,
            EndDate = projectDto.EndDate,
            ProjectManagerID = projectDto.ProjectManagerID
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProject), new { id = project.ProjectID }, projectDto);
    }

    // PUT: api/projects/{id}
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, ProjectDto projectDto)
    {
        if (id != projectDto.ProjectID)
        {
            return BadRequest();
        }

        var project = await _context.Projects.FindAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        project.ProjectName = projectDto.ProjectName;
        project.StartDate = projectDto.StartDate;
        project.EndDate = projectDto.EndDate;
        project.ProjectManagerID = projectDto.ProjectManagerID;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/projects/{id}
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
