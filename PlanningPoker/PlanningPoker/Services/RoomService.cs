
using Microsoft.EntityFrameworkCore;
using PlanningPoker.Data;

namespace PlanningPoker.Services
{
    public class RoomService : IroomService
    {
        private readonly PokerContext _dbContext;
        private readonly IuserService _userService;
        private readonly IavatarService _avatarService;
        public RoomService(IuserService userService, IavatarService avatarService)
        {
            _dbContext = new PokerContext();
            _userService = userService;
            _avatarService = avatarService;
        }
        public async Task<Room> GetOrCreateRoomAsync(string roomName, string userName, string connectionId)
        {

            if (!string.IsNullOrEmpty(roomName) && !string.IsNullOrEmpty(userName))
            {
                var user = await _userService.GetOrCreateUserAsync(userName, connectionId);

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
                    return newRoom;
                }
                return existingRoom;
            }
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

        public async Task JoinRoomAsync(Room room, User user)
        {
            var userRoom = await _dbContext.UserRooms.FirstOrDefaultAsync(x => x.UserId == user.Id && x.RoomId == room.Name);
            if (userRoom == null)
            {
                var newUserRoom = new UserRoom
                {
                    UserId = user.Id,
                    RoomId = room.Id,
                    Id = Guid.NewGuid().ToString()
                };
                _dbContext.UserRooms.Add(newUserRoom);
                await _dbContext.SaveChangesAsync();
            }

        }

        public async Task<bool> UserExistsInRoom(string roomName, string userName)
        {
            var user = await _dbContext.UserRooms.FirstOrDefaultAsync(x => x.User.Name == userName && x.Room.Name == roomName);
            return user != null;

        }
    }
}
