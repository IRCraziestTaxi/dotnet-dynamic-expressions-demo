using DotnetDynamicExpressionsDemo.Models;
using DotnetDynamicExpressionsDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotnetDynamicExpressionsDemo.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class QueriesController(IQueryService _queryService) : ControllerBase {
        [HttpPost]
        public async Task<IActionResult> QueryEntities([FromBody] Query query) {
            var results = await _queryService.ExecuteQuery(query);

            return Ok(results);
        }
    }
}
