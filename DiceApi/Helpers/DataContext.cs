using DiceApi.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceApi.Helpers
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<UserRoom> UserRooms { get; set; }

        public DbSet<Roll> Rolls { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().HasKey(q => q.Id);

            builder.Entity<Room>().HasKey(q => q.Id);

            builder.Entity<UserRoom>().HasKey(q => new {q.UserId, q.RoomId});
            builder.Entity<UserRoom>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRooms)
                .HasForeignKey(ur => ur.UserId);
            builder.Entity<UserRoom>()
                .HasOne(ur => ur.Room)
                .WithMany(r => r.RoomUsers)
                .HasForeignKey(ur => ur.RoomId);

            builder.Entity<Roll>().HasKey(q => q.Id);
            builder.Entity<Roll>()
                .HasOne(r => r.User)
                .WithMany(r => r.Rolls)
                .HasForeignKey(r => r.UserId);
            builder.Entity<Roll>()
                .HasOne(r => r.Room)
                .WithMany(r => r.Rolls)
                .HasForeignKey(r => r.RoomId);
            builder.Entity<Roll>().Property(r => r.CreatedTime).HasDefaultValueSql("getutcdate()");
        }
    }
}