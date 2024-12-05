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

        public PokerHub(IavatarService avatarService, IroomService roomService, IuserService userService)
        {
            _avatarService = avatarService;
            _dbContext = new PokerContext();
            _roomService = roomService;
            _userService = userService;
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public async Task JoinRoom(string roomName, string userName)
        {
            // add user to db
            var room = await _roomService.GetOrCreateRoomAsync(roomName, userName, Context.ConnectionId);
            var user = await _userService.GetOrCreateUserAsync(userName, Context.ConnectionId);

            bool isNewUser = false;
            if (!await _roomService.UserExistsInRoom(roomName, userName))
            {
                await _roomService.JoinRoomAsync(room, user);
                isNewUser = true;

            }

            var usersInRoom = await _roomService.GetUsersInRoomAsync(roomName);

            var userListWithAvatars = usersInRoom.Select(user => new
            {
                UserName = user.Name,
                Avatar = user.Avatar
            }).ToList();

            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            //// await Clients.Group(roomName).SendAsync("UserJoined", userName, users);

            //if (isNewUser)
            //{
            //    await Clients.Group(roomName).SendAsync("UserJoined", new User
            //    {
            //        Name = UserAvatars.FirstOrDefault(x => x.Key == userName).Key,
            //        Avatar = UserAvatars.FirstOrDefault(x => x.Key == userName).Value
            //    }, userListWithAvatars);
            //}

            if (isNewUser)
            {
                await Clients.Group(roomName).SendAsync("UserJoined", new User
                {
                    Name = usersInRoom.FirstOrDefault(x => x.Name == userName).Name,
                    Avatar = usersInRoom.FirstOrDefault(x => x.Name == userName).Avatar
                }, userListWithAvatars);
            }
            await Clients.Caller.SendAsync("ReceiveUserList", userListWithAvatars);
            //await Clients.OthersInGroup(roomName).SendAsync("UserJoined", $"{userName} has joined the room.");
        }

        public async Task LeaveRoom(string roomName, string userName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("ReceiveMessage", $"{userName} has left the room.");
        }
        public async Task SubmitVote(string roomName, string userName, string vote)
        {
            await Clients.Group(roomName).SendAsync("ReceiveVote", userName, vote);
        }
        public async Task RevealVotes(string roomName, string[] userVoted, string[] votes)
        {
            List<int> intVotes = new List<int>();
            foreach (string str in votes)
            {
                if (int.TryParse(str, out int val))
                {
                    intVotes.Add(val);
                }
            }
            var avarageVotes = intVotes.Sum() / userVoted.Count();

            await Clients.Group(roomName).SendAsync("ReceiveAvarageVote", avarageVotes);
        }
        public async Task ResetVotes(string roomName)
        {
            //await Clients.Group(roomName).SendAsync("ReceiveMessage", "Votes have been cleared.");
            await Clients.Group(roomName).SendAsync("ClearVotes");
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            foreach (var (roomName, users) in RoomUsers)
            {
                lock (users)
                {
                    if (UserConnections.TryGetValue(Context.ConnectionId, out var userName))
                    {
                        users.Remove(userName);
                        UserConnections.TryRemove(Context.ConnectionId, out _);

                        // Notify the room about the user leaving
                        Clients.Group(roomName).SendAsync("UserLeft", userName);
                    }
                    if (!users.Any())
                    {
                        RoomUsers.TryRemove(roomName, out _);
                    }
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
