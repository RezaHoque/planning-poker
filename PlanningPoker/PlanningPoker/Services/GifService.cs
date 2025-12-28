using log4net;
using System.Text.Json;

namespace PlanningPoker.Services
{
    public class GifService : IgifService
    {
        private readonly ILog _log;

        public GifService(ILog log)
        {
            _log = log;
        }

        public async Task<string> GetGif(string number)
        {
            try
            {
                var baseUrl = Environment.GetEnvironmentVariable("GIPHY_API_BASE_URL");
                var query = Environment.GetEnvironmentVariable("GIPHY_API_QUERY");

                if (string.IsNullOrEmpty(baseUrl) || string.IsNullOrEmpty(query))
                {
                    _log.Warn("GIPHY API configuration not found. GIF feature will be disabled.");
                    return string.Empty;
                }

                var keyWord = number switch
                {
                    "1" => "piece of cake",
                    "2" => "Sounds good",
                    "3" => "you are amazing",
                    "5" => "Hi 5",
                    "8" => "are you serious",
                    "13" => "Way too much",
                    "21" => "angry",
                    _ => "happy"
                };

                var url = $"{baseUrl}&q={keyWord}&{query}";

                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(5);
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var json = JsonDocument.Parse(content);
                    var random = new Random();
                    var gifIndex = random.Next(0, 5);

                    var gifUrl = json.RootElement
                        .GetProperty("data")[gifIndex]
                        .GetProperty("images")
                        .GetProperty("original")
                        .GetProperty("url")
                        .GetString();

                    return gifUrl ?? string.Empty;
                }

                _log.Warn($"GIPHY API request failed with status: {response.StatusCode}");
                return string.Empty;
            }
            catch (Exception ex)
            {
                _log.Error("Error fetching GIF from GIPHY API", ex);
                return string.Empty;
            }
        }
    }
}
