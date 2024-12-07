using PlanningPoker.Data;

namespace PlanningPoker.Services
{
    public interface IroomService
    {
        Task<Room> GetOrCreateRoomAsync(string roomName, string userName);
        Task JoinRoomAsync(Room room, User user, string connectionId);
        Task<List<User>> GetUsersInRoomAsync(string roomName);
        Task<bool> UserExistsInRoom(string roomName, string userName);
        Task LeaveRoomAsync(string roomName, string userName, string connectionId);
        Task LeaveRoomAsync(string userName, string connectionId);
        Task<List<UserRoom>> GetUsersInRoomByConnectionAsync(string connectionId);
    }
}
