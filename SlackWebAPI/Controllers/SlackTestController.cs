using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlackWebAPI.Services;

namespace SlackWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlackTestController : ControllerBase
    { 
        private readonly SlackService _slackService;
        public SlackTestController(SlackService slackHelper)
        {
            _slackService = slackHelper;
        }
        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromForm] string message)
        {
            await _slackService.SendMessageAsync(message);
            return Ok("Message sent successfully");
        }
    }
}
