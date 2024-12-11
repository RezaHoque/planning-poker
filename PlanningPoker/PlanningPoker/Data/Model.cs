﻿using Microsoft.EntityFrameworkCore;

namespace PlanningPoker.Data
{
    public class PokerContext : DbContext
    {
        public string DbPath { get; }
        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<UserRoom> UserRooms { get; set; }
        public DbSet<UserAvatar> UserAvatars { get; set; }
        public PokerContext()
        {
            var folder = Path.Combine(AppContext.BaseDirectory, "Database");
            Directory.CreateDirectory(folder);
            //Environment.SpecialFolder.LocalApplicationData;
            //var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(folder, "planningPoker.db");
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source={DbPath}");

    }

    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public DateTime JoinDate { get; set; }
        public string RoomName { get; set; }

    }
    public class Room
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string OwnerId { get; set; }
        public DateTime CreateDate { get; set; }

    }
    public class UserRoom
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string RoomId { get; set; }
        public Room Room { get; set; }
        public string ConnectionId { get; set; }
    }
    public class UserAvatar
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public string RoomId { get; set; }
    }

}
