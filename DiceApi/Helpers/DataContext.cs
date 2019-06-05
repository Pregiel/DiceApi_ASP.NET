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
                entity.Property(u => u.Username).IsRequired();
                entity.HasIndex(u => u.Username).IsUnique();
            });

            builder.Entity<Room>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Title).IsRequired();
            });

            builder.Entity<UserRoom>(entity =>
            {
                entity.HasKey(q => new { q.UserId, q.RoomId });
                entity.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRooms)
                    .HasForeignKey(ur => ur.UserId);
                entity.HasOne(ur => ur.Room)
                    .WithMany(r => r.RoomUsers)
                    .HasForeignKey(ur => ur.RoomId);
            });

            builder.Entity<Roll>(entity =>
            {
                entity.HasKey(q => q.Id);
                entity.HasOne(r => r.User)
                    .WithMany(u => u.Rolls)
                    .HasForeignKey(r => r.UserId);
                entity.HasOne(r => r.Room)
                    .WithMany(r => r.Rolls)
                    .HasForeignKey(r => r.RoomId);
                entity.Property(r => r.CreatedTime).HasDefaultValueSql("getutcdate()");
            });

            builder.Entity<RollValue>(entity =>
            {
                entity.HasKey(q => q.Id);
                entity.HasOne(rv => rv.Roll)
                .WithMany(ur => ur.RollValues)
                .HasForeignKey(rv => rv.RollId);
            });
        }
    }
}