namespace PlanningPoker.Services
{
    public interface IavatarService
    {
        Task<string> GetAvatar(string userName, string roomId);

    }
}
