using PlanningPoker.Data;

namespace PlanningPoker.Services
{
    public interface IavatarService
    {
        Task<string> GetAvatar(string userName, string roomId, string iconPack);
        Task<User> SaveAvatar(string userName, string roomId, string avatar);


    }
}
