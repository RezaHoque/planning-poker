using DotNetEnv.Extensions;
using log4net;
using System.Text.Json;

namespace PlanningPoker.Services
{
    public class GifService : IgifService
    {
        private readonly IConfiguration _configuration;
        private readonly ILog _log;
        public GifService(IConfiguration configuration, ILog log)
        {
            _configuration = configuration;
            _log = log;
        }
        public async Task<string> GetGif(string number)
        {
            var dict = DotNetEnv.Env.NoEnvVars().Load().ToDotEnvDictionary();

            var baseUrl = dict["GIPHY_API_BASE_URL"];
            var query = $"{dict["GIPHY_API_QUERY"]}";

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

            var client = new HttpClient();
            var response = await client.GetAsync(url);
            var random = new Random();
            var gifIndex = random.Next(0, 5);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);
                var gifUrl = json.RootElement
                     .GetProperty("data")[gifIndex]
                     .GetProperty("images")
                     .GetProperty("original")
                     .GetProperty("url")
                     .GetString();
                return gifUrl;
            }
            return string.Empty;
        }
    }
}
