using Microsoft.AspNetCore.Mvc;

namespace CotdQualifierRankWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RankController : ControllerBase
    {
        [HttpGet]
        [Route("{mapUID}/{time:int}")]
        public IActionResult GetAction(string mapUID, int time)
        {
            return Ok(new { pb = mapUID, rank = time });
        }
    }
}
