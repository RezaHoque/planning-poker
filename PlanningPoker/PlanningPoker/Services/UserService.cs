using log4net;
using Microsoft.EntityFrameworkCore;
using PlanningPoker.Data;

namespace PlanningPoker.Services
{
    public class UserService : IuserService
    {
        private readonly PokerContext _dbContext;
        private readonly IavatarService _avatarService;
        private readonly InameService _nameService;
        private readonly ILog _log;
        public UserService(IavatarService avatarService, ILog log, InameService nameService)
        {
            _dbContext = new PokerContext();
            _avatarService = avatarService;
            _log = log;
            _nameService = nameService;
        }

        public async Task<string> CheckAndGetUserName(string userName, string roomName)
        {
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Name == userName && x.RoomName == roomName);
            if (existingUser != null)
            {
                var newUserName = $"{userName} {_nameService.GenerateUserName()}";
                return newUserName;
            }
            return userName;
        }

        public async Task<User> GetOrCreateUserAsync(string userName, string roomName, string iconPack, bool isModerator)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Name == userName && x.RoomName == roomName);
                if (existingUser == null)
                {
                    var newUser = new User
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = userName,
                        JoinDate = DateTime.Now,
                        Avatar = string.Empty,
                        RoomName = roomName,
                        IconPack = iconPack,
                        IsModerator = isModerator

                    };
                    _dbContext.Users.Add(newUser);
                    await _dbContext.SaveChangesAsync();
                    return newUser;
                }
                existingUser.IconPack = iconPack;
                await _dbContext.SaveChangesAsync();
                return existingUser;
            }
            return null;
        }

        public async Task<User> GetUserByConnectionId(string connectionId)
        {
            var user = await _dbContext.UserRooms.Include(x => x.User).FirstOrDefaultAsync(x => x.ConnectionId == connectionId);
            if (user != null)
            {
                return user.User;
            }
            return null;

        }
    }
}
