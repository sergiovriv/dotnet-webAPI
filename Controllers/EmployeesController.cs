using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using taskmanagementAPI.Data;
using taskmanagementAPI.Models;

namespace taskmanagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly TaskManagementContext _context;

        public EmployeesController(TaskManagementContext context)
        {
            _context = context;
        }

        // GET: api/employees/{id}
        // get info of certain employee by its key
        
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
        {
            var employee = await _context.Employees
                                         .Select(e => new EmployeeDto
                                         {
                                             EmployeeID = e.EmployeeID,
                                             FirstName = e.FirstName,
                                             LastName = e.LastName,
                                             Email = e.Email,
                                             Department = e.Department
                                         })
                                         .FirstOrDefaultAsync(e => e.EmployeeID == id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        // GET: api/employees
        // get all the employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Object>>> GetAllEmployees()
        {
            var employees = await _context.Employees
                .Select(e => new
                {
                    e.EmployeeID,
                    e.FirstName,
                    e.LastName,
                    e.Email,
                    e.Department
                })
                .ToListAsync();

            return Ok(employees);
        }

        // GET: api/employees/{id}/tasks
        // prints the tasks of an employee
        [HttpGet("{id}/tasks")]
        public async Task<ActionResult<IEnumerable<Object>>> GetEmployeeTasks(int id)
        {
            var employeeTasks = await _context.Tasks
                .Where(t => t.AssignedTo == id)
                .Select(t => new
                {
                    t.TaskID,
                    t.TaskName
                })
                .ToListAsync();

            if (!employeeTasks.Any())
            {
                return NotFound();
            }

            return Ok(employeeTasks);
        }


        // POST: api/employees
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee(EmployeeDto employeeDto)
        {
            var employee = new Employee
            {
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Email = employeeDto.Email,
                Department = employeeDto.Department
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeID }, employee);
        }

        // PUT: api/employees/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, EmployeeDto employeeDto)
        {
            if (id != employeeDto.EmployeeID)
            {
                return BadRequest();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            employee.FirstName = employeeDto.FirstName;
            employee.LastName = employeeDto.LastName;
            employee.Email = employeeDto.Email;
            employee.Department = employeeDto.Department;

            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/employees/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

