using Microsoft.AspNetCore.Mvc;

namespace WIMP_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public ActionResult HealthCheck()
        {
            return Ok();
        }
    }
}