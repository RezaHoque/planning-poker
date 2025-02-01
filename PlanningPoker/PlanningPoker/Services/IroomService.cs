using PlanningPoker.Data;

namespace PlanningPoker.Services
{
    public interface IroomService
    {
        Task<Room> GetOrCreateRoomAsync(string roomName, string userName, string iconPack);
        Task JoinRoomAsync(Room room, User user, string connectionId);
        Task<List<User>> GetUsersInRoomAsync(string roomName);
        Task<bool> UserExistsInRoom(string roomName, string userName);
        Task LeaveRoomAsync(string roomName, string userName, string connectionId);
        Task LeaveRoomAsync(string userName, string connectionId);
        Task<List<UserRoom>> GetUsersInRoomByConnectionAsync(string connectionId);
        Task AddRoomVotes(string roomName, string userName, string vote);
        Task RemoveRoomVotes(string roomName);
        int? CalculateAvarageRoomVotes(string roomName);
        int? CalculateAvarageFibRoomVotes(string roomName);
        Task<List<RoomVote>> GetRoomVotesWithUsersAsync(string roomName);
    }
}
