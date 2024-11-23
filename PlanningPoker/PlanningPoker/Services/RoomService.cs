
using PlanningPoker.Data;

namespace PlanningPoker.Services
{
    public class RoomService : IroomService
    {
        private readonly PokerContext _dbContext;
        public RoomService()
        {
            _dbContext = new PokerContext();
        }
        public Task<string> CreateRoomAsync(string roomName, string userName)
        {
            throw new NotImplementedException();
        }

        public Task<string> JoinRoomAsync(string roomName, string userName)
        {
            throw new NotImplementedException();
        }
    }
}
