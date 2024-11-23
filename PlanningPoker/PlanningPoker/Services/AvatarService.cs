
using Microsoft.EntityFrameworkCore;
using PlanningPoker.Data;

namespace PlanningPoker.Services
{
    public class AvatarService : IavatarService
    {
        private readonly PokerContext _dbContext;
        public AvatarService()
        {
            _dbContext = new PokerContext();
        }
        public async Task<string> GetAvatar(string userName, string roomId)
        {
            var avatar = await _dbContext.UserAvatars.FirstOrDefaultAsync(x => x.UserName == userName && x.RoomId == roomId);
            if (avatar != null)
            {
                return avatar.Avatar;
            }

            var avatarApiEndpoint = "https://api.multiavatar.com/";
            var userAvaratUrl = $"{avatarApiEndpoint}{userName}.png";
            // var httpClient = new HttpClient();
            // var response = httpClient.GetAsync($"{avatarApiEndpoint}{userName}").Result;
            //var content = response.Content.ReadAsStringAsync().Result;

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

    }
}
