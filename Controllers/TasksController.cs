using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using taskmanagementAPI.Data;
using Microsoft.EntityFrameworkCore;
using TaskModel = taskmanagementAPI.Models.Task; //alias for taks model

[Route("api/[controller]")]
[ApiController]
public class TasksController : ControllerBase
{
    private readonly TaskManagementContext _context;

    public TasksController(TaskManagementContext context)
    {
        _context = context;
    }

    // GET: api/tasks/{id}
    // obtain an specific task
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskDto>> GetTask(int id)
    {
        var task = await _context.Tasks
            .Where(t => t.TaskID == id)
            .Select(t => new TaskDto
            {
                TaskID = t.TaskID,
                ProjectID = t.ProjectID,
                TaskName = t.TaskName,
                Description = t.Description,
                AssignedTo = t.AssignedTo,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                Status = t.Status
            })
            .FirstOrDefaultAsync();

        if (task == null)
        {
            return NotFound();
        }

        return Ok(task);
    }

    // GET: api/tasks
    // obtain all tasks
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetAllTasks()
    {
        var tasks = await _context.Tasks
            .Select(t => new TaskDto
            {
                TaskID = t.TaskID,
                ProjectID = t.ProjectID,
                TaskName = t.TaskName,
                Description = t.Description,
                AssignedTo = t.AssignedTo,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                Status = t.Status
            })
            .ToListAsync();

        return Ok(tasks);
    }

    // POST: api/tasks
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<TaskDto>> CreateTask(TaskDto taskDto)
    {
        var task = new TaskModel
        {
            ProjectID = taskDto.ProjectID,
            TaskName = taskDto.TaskName,
            Description = taskDto.Description,
            AssignedTo = taskDto.AssignedTo,
            StartDate = taskDto.StartDate,
            EndDate = taskDto.EndDate,
            Status = taskDto.Status
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTask), new { id = task.TaskID }, taskDto);
    }

    // PUT: api/tasks/{id}
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, TaskDto taskDto)
    {
        if (id != taskDto.TaskID)
        {
            return BadRequest();
        }

        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        task.ProjectID = taskDto.ProjectID;
        task.TaskName = taskDto.TaskName;
        task.Description = taskDto.Description;
        task.AssignedTo = taskDto.AssignedTo;
        task.StartDate = taskDto.StartDate;
        task.EndDate = taskDto.EndDate;
        task.Status = taskDto.Status;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Tasks.Any(e => e.TaskID == id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/tasks/{id}
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
