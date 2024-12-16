using PlanningPoker.Data;

namespace PlanningPoker.Services
{
    public interface IuserService
    {
        Task<User> GetOrCreateUserAsync(string userName, string roomName);
        Task<User> GetUserByConnectionId(string connectionId);
        Task<string> CheckAndGetUserName(string userName, string roomName);

    }
}
