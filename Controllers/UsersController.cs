using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using taskmanagementAPI.Models;
using taskmanagementAPI.Services;

namespace taskmanagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userModel)
        {
            var user = await _userService.CreateUser(userModel.Username, userModel.Password);
            if (user == null)
            {
                return BadRequest("Unable to create user.");
            }

            return Ok(user);
        }

    }
}

