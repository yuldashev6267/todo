using Microsoft.AspNetCore.Mvc;

namespace Todo.WebAPi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        public TestController(){}

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello");
        }
    }
}