
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
            try
            {
                await DeleteUserRoom(userName, roomName, connectionId);
                await DeleteAvatar(userName, roomName);
                await DeleteUser(userName, roomName);
            }
            catch (Exception ex)
            {
                _log.Error($"Error leaving room {roomName} by {userName}. connectiod id: {connectionId}", ex);
            }

        }

        public async Task LeaveRoomAsync(string userName, string connectionId)
        {
            try
            {
                var room = await _dbContext.UserRooms.Include(x => x.User).Include(x => x.Room).FirstOrDefaultAsync(x => x.ConnectionId == connectionId);
                if (room != null)
                {
                    await DeleteUserRoom(userName, connectionId);
                    await DeleteAvatar(userName, room.Room.Name);
                    await DeleteUser(userName, room.Room.Name);
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Error leaving room by {userName}. connectiod id: {connectionId}", ex);
            }

        }

        public async Task<bool> UserExistsInRoom(string roomName, string userName)
        {
            var user = await _dbContext.UserRooms.FirstOrDefaultAsync(x => x.User.Name == userName && x.Room.Name == roomName);
            return user != null;

        }

        #region private methods
        private async Task DeleteAvatar(string userName, string roomName)
        {
            try
            {
                var avatar = _dbContext.UserAvatars.FirstOrDefault(x => x.UserName == userName && x.RoomId == roomName);
                if (avatar != null)
                {
                    _dbContext.UserAvatars.Remove(avatar);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Error deleting avatar by {userName} and room name {roomName}", ex);
            }

        }
        private async Task DeleteUserRoom(string userName, string roomName, string connectionId)
        {
            try
            {
                var userRooms = _dbContext.UserRooms.Where(x => x.User.Name == userName && x.Room.Name == roomName && x.ConnectionId == connectionId).ToList();
                if (userRooms.Count > 0)
                {
                    _dbContext.UserRooms.RemoveRange(userRooms);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Error deleting user room by {userName} and room name {roomName} and connection id {connectionId}", ex);
            }

        }
        private async Task DeleteUserRoom(string userName, string connectionId)
        {
            try
            {
                var userRooms = _dbContext.UserRooms.Where(x => x.User.Name == userName && x.ConnectionId == connectionId).ToList();
                if (userRooms.Count > 0)
                {
                    _dbContext.UserRooms.RemoveRange(userRooms);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Error deleting user room by {userName} and connection id {connectionId}", ex);
            }

        }

        private async Task DeleteUser(string userName, string roomName)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Name == userName && x.RoomName == roomName);
                if (user != null)
                {
                    _dbContext.Users.Remove(user);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Error deleting user by {userName} and room name {roomName}", ex);
            }

        }
        #endregion
    }
}
