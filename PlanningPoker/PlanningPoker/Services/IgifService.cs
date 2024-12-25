namespace PlanningPoker.Services
{
    public interface IgifService
    {
        Task<string> GetGif(string keyWord);
    }
}
