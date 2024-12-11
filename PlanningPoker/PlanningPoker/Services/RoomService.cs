
using log4net;
using Microsoft.EntityFrameworkCore;
using PlanningPoker.Data;

namespace PlanningPoker.Services
{
    public class RoomService : IroomService
    {
        private readonly PokerContext _dbContext;
        private readonly IuserService _userService;
        private readonly IavatarService _avatarService;
        private readonly ILog _log;
        public RoomService(IuserService userService, IavatarService avatarService, ILog log)
        {
            _dbContext = new PokerContext();
            _userService = userService;
            _avatarService = avatarService;
            _log = log;
        }
        public async Task<Room> GetOrCreateRoomAsync(string roomName, string userName)
        {

            if (!string.IsNullOrEmpty(roomName) && !string.IsNullOrEmpty(userName))
            {
                var user = await _userService.GetOrCreateUserAsync(userName, roomName);

                var existingRoom = await _dbContext.Rooms.FirstOrDefaultAsync(x => x.Name == roomName);
                if (existingRoom == null)
                {
                    var newRoom = new Room
                    {
                        Name = roomName,
                        OwnerId = user.Id,
                        CreateDate = DateTime.Now,
                        Id = Guid.NewGuid().ToString()
                    };
                    _dbContext.Rooms.Add(newRoom);
                    await _dbContext.SaveChangesAsync();
                    _log.Info($"New Room {roomName} created by {user.Name}");
                    return newRoom;
                }
                return existingRoom;
            }
            _log.Info("Room name or user name is empty");
            return null;

        }

        public async Task<List<User>> GetUsersInRoomAsync(string roomName)
        {
            var usersInRoom = _dbContext.UserRooms.Where(x => x.Room.Name == roomName).Select(x => x.User).ToList();
            foreach (var user in usersInRoom)
            {
                user.Avatar = await _avatarService.GetAvatar(user.Name, roomName);
                _dbContext.Users.Update(user);
            }
            return usersInRoom;
        }

        public async Task<List<UserRoom>> GetUsersInRoomByConnectionAsync(string connectionId)
        {
            var usersInRoom = await _dbContext.UserRooms.Include(x => x.User).Include(x => x.Room).Where(x => x.ConnectionId == connectionId).ToListAsync();
            return usersInRoom;
        }

        public async Task JoinRoomAsync(Room room, User user, string connectionId)
        {
            var userRoom = await _dbContext.UserRooms.Include(x => x.User).Include(x => x.Room).FirstOrDefaultAsync(x => x.UserId == user.Id && x.RoomId == room.Name && x.ConnectionId == connectionId);
            if (userRoom == null)
            {
                var newUserRoom = new UserRoom
                {
                    UserId = user.Id,
                    RoomId = room.Id,
                    Id = Guid.NewGuid().ToString(),
                    ConnectionId = connectionId
                };
                _dbContext.UserRooms.Add(newUserRoom);
                await _dbContext.SaveChangesAsync();
            }

        }

        public async Task LeaveRoomAsync(string roomName, string userName, string connectionId)
        {
            await deleteUserRoom(userName, roomName, connectionId);
            await deleteAvatar(userName, roomName);
        }

        public async Task LeaveRoomAsync(string userName, string connectionId)
        {
            await deleteUserRoom(userName, connectionId);
            var room = await _dbContext.UserRooms.Include(x => x.User).Include(x => x.Room).FirstOrDefaultAsync(x => x.ConnectionId == connectionId);
            if (room != null)
            {
                await deleteAvatar(userName, room.Room.Name);
            }
        }

        public async Task<bool> UserExistsInRoom(string roomName, string userName)
        {
            var user = await _dbContext.UserRooms.FirstOrDefaultAsync(x => x.User.Name == userName && x.Room.Name == roomName);
            return user != null;

        }

        #region private methods
        private async Task deleteAvatar(string userName, string roomName)
        {
            var avatar = _dbContext.UserAvatars.FirstOrDefault(x => x.UserName == userName && x.RoomId == roomName);
            if (avatar != null)
            {
                _dbContext.UserAvatars.Remove(avatar);
                await _dbContext.SaveChangesAsync();
            }
        }
        private async Task deleteUserRoom(string userName, string roomName, string connectionId)
        {
            var userRooms = _dbContext.UserRooms.Where(x => x.User.Name == userName && x.Room.Name == roomName && x.ConnectionId == connectionId).ToList();
            if (userRooms.Count > 0)
            {
                _dbContext.UserRooms.RemoveRange(userRooms);
                await _dbContext.SaveChangesAsync();
            }
        }
        private async Task deleteUserRoom(string userName, string connectionId)
        {
            var userRooms = _dbContext.UserRooms.Where(x => x.User.Name == userName && x.ConnectionId == connectionId).ToList();
            if (userRooms.Count > 0)
            {
                _dbContext.UserRooms.RemoveRange(userRooms);
                await _dbContext.SaveChangesAsync();
            }
        }
        #endregion
    }
}
