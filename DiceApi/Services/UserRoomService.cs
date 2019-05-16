using DiceApi.Entities;
using DiceApi.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceApi.Services
{
    public interface IUserRoomService
    {
        Task<UserRoom> Create(User user, Room room, bool owner);
        Task<IEnumerable<UserRoom>> GetAll();
        Task<User> GetOwner(Room room);
        Task<UserRoom> GetByIds(int userId, int roomId);
        Task<IEnumerable<Room>> GetRoomsByUserId(int userId);
        Task<IEnumerable<User>> GetUsersByRoomId(int roomId);
        Task ChangeOwner(User newOwner, Room room);
        Task DeleteUserFromRoom(User user, Room room);
        Task Delete(UserRoom userRoom);
    }
    public class UserRoomService : IUserRoomService
    {
        private DataContext _context;

        public UserRoomService(DataContext context)
        {
            _context = context;
        }

        public async Task<UserRoom> Create(User user, Room room, bool owner)
        {
            UserRoom userRoom = new UserRoom(user, room, owner);

            if (!await _context.UserRooms.AnyAsync(x => x.UserId == user.Id && x.RoomId == room.Id))
            {
                await _context.UserRooms.AddAsync(userRoom);
                await _context.SaveChangesAsync();
            }
            else
                throw new ApplicationException(Properties.resultMessages.UserRoomExists);

            return userRoom;
        }

        public async Task<IEnumerable<UserRoom>> GetAll()
        {
            return await _context.UserRooms
                .Include(x => x.User)
                .Include(x => x.Room)
                .ToListAsync();
        }

        public async Task<User> GetOwner(Room roomParam)
        {
            var room = await _context.Rooms
                .Include(x => x.RoomUsers)
                .SingleOrDefaultAsync(x => x.Id == roomParam.Id);

            if (room == null)
                throw new ApplicationException(Properties.resultMessages.RoomNotFound);

            var owner = await _context.UserRooms
                .SingleOrDefaultAsync(x => x.RoomId == room.Id && x.Owner == true);

            if (owner == null)
                throw new ApplicationException(Properties.resultMessages.UserNotFound);

            return owner.User;
        }

        public async Task<UserRoom> GetByIds(int userId, int roomId)
        {
            return await _context.UserRooms
                .Include(x => x.User)
                .Include(x => x.Room)
                .SingleOrDefaultAsync(x => x.UserId == userId && x.RoomId == roomId);
        }

        public async Task<IEnumerable<Room>> GetRoomsByUserId(int userId)
        {
            return await _context.UserRooms
                .Include(x => x.User)
                .Include(x => x.Room)
                .Where(x => x.UserId == userId)
                .Select(x => x.Room)
                .Include(x => x.RoomUsers)
                .ThenInclude(x => x.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByRoomId(int roomId)
        {
            return await _context.UserRooms
                .Include(x => x.User)
                .Include(x => x.Room)
                .Where(x => x.RoomId == roomId)
                .Select(x => x.User)
                .ToListAsync();
        }

        public async Task ChangeOwner(User newOwner, Room room)
        {
            var userRoom = await _context.UserRooms.FindAsync(newOwner.Id, room.Id);

            if (userRoom == null)
                throw new ApplicationException(Properties.resultMessages.UserRoomNotFound);

            foreach (UserRoom ur in room.RoomUsers)
            {
                if (ur.User.Id == newOwner.Id)
                    ur.Owner = true;
                else
                    ur.Owner = false;
                _context.UserRooms.Update(ur);
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserFromRoom(User user, Room room)
        {
            var userRoom = await _context.UserRooms.FindAsync(user.Id, room.Id);
            if (userRoom != null)
            {
                _context.UserRooms.Remove(userRoom);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Delete(UserRoom userRoom)
        {
            _context.UserRooms.Remove(userRoom);
            await _context.SaveChangesAsync();
        }
    }
}
