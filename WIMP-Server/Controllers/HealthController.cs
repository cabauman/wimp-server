using Microsoft.AspNetCore.Mvc;

namespace WIMP_Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public ActionResult HealthCheck()
        {
            return Ok();
        }
    }
}