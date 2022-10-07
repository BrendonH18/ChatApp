using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {



        [HttpPost("SendMessage")]
        public IActionResult Public()
        {
            return Ok("You're on public property");
        }
    }
}
