using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace BookingManagement.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        [Route("[action]")]
        public JsonResult CheckIfLive()
        {
            return new JsonResult(new { Live = true });
        }
    }
}
