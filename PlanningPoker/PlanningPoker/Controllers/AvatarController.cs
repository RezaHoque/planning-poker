using Microsoft.AspNetCore.Mvc;

namespace PlanningPoker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvatarController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get(string userName)
        {
            var avatarApiEndpoint = "https://api.multiavatar.com/";

            var httpClient = new HttpClient();
            var response = httpClient.GetAsync($"{avatarApiEndpoint}{userName}").Result;
            var content = response.Content.ReadAsStringAsync().Result;

            return Ok(content);
        }
    }
}
