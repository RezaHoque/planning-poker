namespace PlanningPoker.Services
{
    public interface IroomService
    {
        Task<string> CreateRoomAsync(string roomName, string userName);
        Task<string> JoinRoomAsync(string roomName, string userName);
    }
}
