using PlanningPoker.Data;

namespace PlanningPoker.Services
{
    public interface IroomService
    {
        Task<Room> GetOrCreateRoomAsync(string roomName, string userName, string connectionId);
        Task JoinRoomAsync(Room room, User user);
        Task<List<User>> GetUsersInRoomAsync(string roomName);
        Task<bool> UserExistsInRoom(string roomName, string userName);
    }
}
