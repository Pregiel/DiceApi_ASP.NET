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

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<UserRoom> UserRooms { get; set; }
        public virtual DbSet<Roll> Rolls { get; set; }
        public virtual DbSet<RollValue> RollValues { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DataContext() { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
            });

            builder.Entity<Room>().HasKey(q => q.Id);

            builder.Entity<UserRoom>().HasKey(q => new { q.UserId, q.RoomId });
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
                .WithMany(u => u.Rolls)
                .HasForeignKey(r => r.UserId);
            builder.Entity<Roll>()
                .HasOne(r => r.Room)
                .WithMany(r => r.Rolls)
                .HasForeignKey(r => r.RoomId);
            builder.Entity<Roll>().Property(r => r.CreatedTime).HasDefaultValueSql("getutcdate()");

            builder.Entity<RollValue>().HasKey(q => q.Id);
            builder.Entity<RollValue>()
                .HasOne(rv => rv.Roll)
                .WithMany(ur => ur.RollValues)
                .HasForeignKey(rv => rv.RollId);
        }
    }
}