using Microsoft.EntityFrameworkCore;
using PlanningPoker.Data;

namespace PlanningPoker.Services
{
    public class UserService : IuserService
    {
        private readonly PokerContext _dbContext;
        private readonly IavatarService _avatarService;
        public UserService(IavatarService avatarService)
        {
            _dbContext = new PokerContext();
            _avatarService = avatarService;
        }
        public async Task<User> GetOrCreateUserAsync(string userName, string connectionId)
        {
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(connectionId))
            {
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Name == userName);
                if (existingUser == null)
                {
                    var newUser = new User
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = userName,
                        JoinDate = DateTime.Now,
                        ConnectionId = connectionId,
                        Avatar = string.Empty

                    };
                    _dbContext.Users.Add(newUser);
                    await _dbContext.SaveChangesAsync();
                    return newUser;
                }
                return existingUser;
            }
            return null;
        }
    }
}
