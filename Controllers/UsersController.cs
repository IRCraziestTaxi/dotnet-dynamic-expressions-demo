using DotnetDynamicExpressionsDemo.Entities;
using DotnetDynamicExpressionsDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotnetDynamicExpressionsDemo.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUserService _userService) : ControllerBase {
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] User user) {
            var id = await _userService.AddUser(user);

            return Ok(id);
        }

        [HttpPost("{userId}/skills")]
        public async Task<IActionResult> UpsertSkill(int userId, [FromBody] Skill skill) {
            var id = await _userService.UpsertSkill(userId, skill);

            return Ok(id);
        }
    }
}
