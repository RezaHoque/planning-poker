namespace PlanningPoker.Services
{
    public interface InameService
    {
        string GenerateName();
        bool IsNameUnique(string name);
    }
}
