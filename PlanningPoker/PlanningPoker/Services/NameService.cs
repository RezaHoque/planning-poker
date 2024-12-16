
using log4net;
using PlanningPoker.Data;
using System.Text;
using System.Text.Json;

namespace PlanningPoker.Services
{
    public class NameService : InameService
    {
        private readonly IConfiguration _configuration;
        private readonly ILog _log;
        private readonly PokerContext _dbContext;
        public NameService(IConfiguration configuration, ILog log)
        {
            _configuration = configuration;
            _log = log;
            _dbContext = new PokerContext();
        }
        public string GenerateName()
        {
            return GenerateRandomRoomName();
        }

        public string GenerateUserName()
        {
            return GenerateRandomName("-");
        }

        public bool IsNameUnique(string name)
        {
            var roomName = _dbContext.Rooms.FirstOrDefault(x => x.Name == name);
            return roomName == null;
        }

        #region Private Methods
        private Dictionary<string, List<string>> LoadCategories()
        {
            Dictionary<string, List<string>> categories = new Dictionary<string, List<string>>();
            var dataDirectory = _configuration["NameGenerator:FileLocation"];

            var jsonFiles = Directory.GetFiles(dataDirectory, "*.json");
            foreach (var file in jsonFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                string jsonContent = File.ReadAllText(file);
                var items = JsonSerializer.Deserialize<List<string>>(jsonContent);

                if (items != null)
                {
                    categories[fileName] = items;
                }
            }
            return categories;
        }
        private string GenerateRandomRoomName()
        {
            var name = $"{GenerateRandomName("-")}-{GenerateRandomString()}";
            Random random = new Random();
            if (!IsNameUnique(name))
            {
                // add a random number to the end of the name
                return $"{name}-{random.Next(1000)}";
            }

            return name;
        }
        private string GenerateRandomString(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var stringBuilder = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                char randomChar = chars[random.Next(chars.Length)];
                stringBuilder.Append(randomChar);
            }

            return stringBuilder.ToString();
        }
        private string GenerateRandomName(string separatot)
        {
            var categories = LoadCategories();
            Random random = new Random();

            if (!categories.ContainsKey("adjectives"))
            {
                _log.Error("The 'adjectives.json' file is required but was not found.");
                return Guid.NewGuid().ToString();
            }

            // Get a random adjective
            var adjectives = categories["adjectives"];
            string randomAdjective = adjectives[random.Next(adjectives.Count)];


            var otherCategories = new List<string>(categories.Keys);
            otherCategories.Remove("adjectives");

            if (otherCategories.Count == 0)
            {
                _log.Error("No other categories found besides 'adjectives'.");
                return Guid.NewGuid().ToString();
            }

            // Get a random string from any other category
            string randomCategory = otherCategories[random.Next(otherCategories.Count)];
            var items = categories[randomCategory];
            string randomItem = items[random.Next(items.Count)];


            var name = $"{randomAdjective}{separatot}{randomItem}";

            return name;
        }
        #endregion
    }
}
