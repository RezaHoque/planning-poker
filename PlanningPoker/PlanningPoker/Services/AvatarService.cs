
using log4net;
using Microsoft.EntityFrameworkCore;
using PlanningPoker.Data;

namespace PlanningPoker.Services
{
    public class AvatarService : IavatarService
    {
        private readonly PokerContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ILog _log;
        public AvatarService(IConfiguration configuration, ILog log)
        {
            _dbContext = new PokerContext();
            _configuration = configuration;
            _log = log;
        }
        public async Task<string> GetAvatar(string userName, string roomId, string iconPack)
        {
            try
            {
                var avatar = await _dbContext.UserAvatars.FirstOrDefaultAsync(x => x.UserName == userName && x.RoomId == roomId);
                if (avatar != null)
                {
                    return avatar.Avatar;
                }

                var userAvaratUrl = GetAvatarApiEndpoint(userName, iconPack);

                var userAvatar = new UserAvatar
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = userName,
                    Avatar = userAvaratUrl,
                    RoomId = roomId
                };
                _dbContext.UserAvatars.Add(userAvatar);
                await _dbContext.SaveChangesAsync();
                return userAvaratUrl;
            }
            catch (Exception ex)
            {
                _log.Error($"Error getting avatar for {userName} in roomid {roomId}", ex);
                return null;

            }

        }

        public Task<User> SaveAvatar(string userName, string roomId, string avatar)
        {
            throw new NotImplementedException();
        }

        private Dictionary<string, string> GetAvatarApiSettings()
        {
            var apiSettings = new Dictionary<string, string>();

            var apiSection = _configuration.GetSection("Api:Avatar");

            foreach (var avatarType in apiSection.GetChildren())
            {
                foreach (var avatarConfig in avatarType.GetChildren())
                {
                    //  var baseUrl = avatarConfig.GetSection("BaseUrl").Value;
                    if (!string.IsNullOrEmpty(avatarConfig.Value))
                    {
                        // apiSettings[$"{avatarType.Key}:{avatarConfig.Value}"] = baseUrl;
                        if (!apiSettings.ContainsKey(avatarType.Key))
                        {
                            apiSettings.Add(avatarType.Key, avatarConfig.Value);
                        }

                    }
                }
            }

            return apiSettings;
        }
        private string GetAvatarApiEndpoint(string userName, string iconPack)
        {
            var apiSettings = GetAvatarApiSettings();
            if (string.IsNullOrEmpty(iconPack))
            {
                var random = new Random();
                var apiIndex = random.Next(0, apiSettings.Count);
                var selectedSettings = apiSettings.ElementAt(apiIndex);

                if (selectedSettings.Key == "Robohash")
                {
                    return $"{selectedSettings.Value}{userName}?set=set3";
                }
                else if (selectedSettings.Key == "DiceBear")
                {
                    return $"{selectedSettings.Value}{userName}";
                }
                else
                {
                    return $"{selectedSettings.Value}{userName}.png";
                }
            }
            else
            {
                var dictVal = apiSettings.FirstOrDefault(x => x.Key.Equals(iconPack, StringComparison.OrdinalIgnoreCase)).Value;

                if (string.Equals(iconPack, "Robohash", StringComparison.OrdinalIgnoreCase))
                {

                    return $"{dictVal}{userName}?set=set3";
                }
                else if (string.Equals(iconPack, "DiceBear", StringComparison.OrdinalIgnoreCase))
                {
                    return $"{dictVal}{userName}";
                }
                else
                {
                    return $"{dictVal}{userName}.png";

                }
            }




        }

    }
}
