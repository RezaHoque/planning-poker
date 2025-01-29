using log4net;
using Microsoft.AspNetCore.Mvc;
using PlanningPoker.Services;

namespace PlanningPoker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GifController : ControllerBase
    {
        private readonly IgifService _gifService;
        private readonly ILog _logger;

        public GifController(IgifService gifService, ILog logger)
        {
            _gifService = gifService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetGif(string number)
        {
            try
            {
                var keyWord = number switch
                {
                    "1" => "Easy peasy",
                    "2" => "Sounds good",
                    "3" => "you are amazing",
                    "5" => "Great job",
                    "8" => "are you serious",
                    "13" => "Too much",
                    "21" => "angry",
                    _ => "happy"
                };
                var gifUrl = await _gifService.GetGif(keyWord);
                return Ok(gifUrl);
            }
            catch (Exception ex)
            {
                _logger.Error("Error getting gif", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
