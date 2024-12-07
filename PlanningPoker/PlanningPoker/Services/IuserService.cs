using PlanningPoker.Data;

namespace PlanningPoker.Services
{
    public interface IuserService
    {
        Task<User> GetOrCreateUserAsync(string userName);
        Task<User> GetUserByConnectionId(string connectionId);

    }
}
