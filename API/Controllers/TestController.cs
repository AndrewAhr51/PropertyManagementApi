using Microsoft.AspNetCore.Mvc;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        public TestController(ILogger<TestController> logger)
        {
            logger.LogInformation("✅ TestController constructed");
        }

        [HttpGet]
        public string Echo() => "Test successful.";
    }
}
