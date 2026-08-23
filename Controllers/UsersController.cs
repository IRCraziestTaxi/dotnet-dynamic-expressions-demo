using DotnetDynamicExpressionsDemo.Models;
using DotnetDynamicExpressionsDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotnetDynamicExpressionsDemo.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUserService _userService) : ControllerBase {
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] AddUser addUser) {
            var id = await _userService.AddUser(addUser);

            return Ok(id);
        }

        [HttpPost("{userId}/skills")]
        public async Task<IActionResult> UpsertSkill(int userId, [FromBody] AddSkill addSkill) {
            var id = await _userService.UpsertSkill(userId, addSkill);

            return Ok(id);
        }
    }
}
