using log4net;
using Microsoft.AspNetCore.SignalR;
using PlanningPoker.Data;
using PlanningPoker.Services;
using System.Collections.Concurrent;

namespace PlanningPoker.Hubs
{
    public class PokerHub : Hub
    {
        private static readonly ConcurrentDictionary<string, List<string>> RoomUsers = new();
        private static readonly ConcurrentDictionary<string, string> UserConnections = new();
        private static readonly ConcurrentDictionary<string, string> UserAvatars = new();
        private static readonly ConcurrentDictionary<string, string> UserVotes = new();
        private readonly IavatarService _avatarService;
        private readonly PokerContext _dbContext;
        private readonly IroomService _roomService;
        private readonly IuserService _userService;
        private readonly ILog _logger;
        private readonly IgifService _gifService;



        public PokerHub(IavatarService avatarService, IroomService roomService, IuserService userService, ILog logger, IgifService gifService)
        {
            _avatarService = avatarService;
            _dbContext = new PokerContext();
            _roomService = roomService;
            _userService = userService;
            _logger = logger;
            _gifService = gifService;
        }
        //public async Task SendMessage(string user, string message)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", user, message);
        //}
        public async Task JoinRoom(string roomName, string userName, string iconPack, bool isModerator)
        {
            // add user to db
            var room = await _roomService.GetOrCreateRoomAsync(roomName, userName, iconPack);
            var user = await _userService.GetOrCreateUserAsync(userName, roomName, iconPack, isModerator);

            bool isNewUser = false;
            if (!await _roomService.UserExistsInRoom(roomName, userName))
            {
                await _roomService.JoinRoomAsync(room, user, Context.ConnectionId);
                isNewUser = true;

            }

            var usersInRoom = await _roomService.GetUsersInRoomAsync(roomName);

            var userListWithAvatars = usersInRoom.Select(user => new
            {
                UserName = user.Name,
                Avatar = user.Avatar
            }).ToList();

            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);

            if (isNewUser)
            {
                await Clients.Group(roomName).SendAsync("UserJoined", new User
                {
                    Name = usersInRoom.FirstOrDefault(x => x.Name == userName).Name,
                    Avatar = usersInRoom.FirstOrDefault(x => x.Name == userName).Avatar
                }, userListWithAvatars);
            }
            await Clients.Caller.SendAsync("ReceiveUserList", userListWithAvatars);

            _logger.Info($"{userName} has joined the room {roomName}.");
            //await Clients.OthersInGroup(roomName).SendAsync("UserJoined", $"{userName} has joined the room.");
        }

        public async Task LeaveRoom(string roomName, string userName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            await _roomService.LeaveRoomAsync(roomName, userName, Context.ConnectionId);
            await Clients.Group(roomName).SendAsync("Leaveroom", userName);

        }
        public async Task SubmitVote(string roomName, string userName, string vote)
        {
            await _roomService.AddRoomVotes(roomName, userName, vote);
            await Clients.Group(roomName).SendAsync("ReceiveVote", userName, vote);
        }
        public async Task RevealVotes(string roomName)
        {
            var avarageVotes = _roomService.CalculateAvarageRoomVotes(roomName);
            var avarageFibonacciVotes = _roomService.CalculateAvarageFibRoomVotes(roomName);

            var allUservotes = await _roomService.GetRoomVotesWithUsersAsync(roomName);
            var votesWithUsers = allUservotes.Select(vote => new
            {
                vote = vote.Vote,
                userName = vote.UserName
            }).ToList();

            var numberForGif = avarageFibonacciVotes > 0 ? avarageFibonacciVotes : 0;
            var gifUrl = await _gifService.GetGif(numberForGif.ToString());

            await Clients.Group(roomName).SendAsync("ReceiveAvarageVote", avarageVotes, avarageFibonacciVotes, votesWithUsers, gifUrl);
        }
        public async Task ResetVotes(string roomName)
        {
            //await Clients.Group(roomName).SendAsync("ReceiveMessage", "Votes have been cleared.");
            var votes = await _roomService.GetRoomVotesWithUsersAsync(roomName);
            var votesWithUsers = votes.Select(vote => new
            {
                vote = vote.Vote,
                userName = vote.UserName
            }).ToList();

            await _roomService.RemoveRoomVotes(roomName);
            await Clients.Group(roomName).SendAsync("ClearVotes", votesWithUsers);
        }
        public async Task<object> GetRoomVotesWithUsers(string roomName)
        {
            var votes = await _roomService.GetRoomVotesWithUsersAsync(roomName);
            var votesWithUsers = votes.Select(vote => new
            {
                vote = vote.Vote,
                userName = vote.UserName
            }).ToList();

            return votesWithUsers;
            //await Clients.Caller.SendAsync("ReceiveRoomVotes", votesWithUsers);
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = await _userService.GetUserByConnectionId(Context.ConnectionId);
            var usersInroom = await _roomService.GetUsersInRoomByConnectionAsync(Context.ConnectionId);
            var roomName = usersInroom.FirstOrDefault().Room.Name;
            if (user != null)
            {
                if (usersInroom.Any())
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
                    await Clients.Group(roomName).SendAsync("Leaveroom", user.Name);
                }
                await _roomService.LeaveRoomAsync(user.Name, Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendGifToAll(string roomName, string vote)
        {
            var gifUrl = await _gifService.GetGif(vote);
            await Clients.Group(roomName).SendAsync("ReceiveGif", gifUrl);
        }
    }
}
