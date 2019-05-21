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
        UserRoom Create(User user, Room room, bool owner);
        IEnumerable<UserRoom> GetAll();
        User GetOwner(Room room);
        UserRoom GetByIds(int userId, int roomId);
        IEnumerable<Room> GetRoomsByUserId(int userId);
        IEnumerable<User> GetUsersByRoomId(int roomId);
        void ChangeOwner(User newOwner, Room room);
        void DeleteUserFromRoom(User user, Room room);
        void Delete(UserRoom userRoom);
    }
    public class UserRoomService : IUserRoomService
    {
        private DataContext _context;

        public UserRoomService(DataContext context)
        {
            _context = context;
        }

        public UserRoom Create(User user, Room room, bool owner)
        {
            UserRoom userRoom = new UserRoom(user, room, owner);

            if (!_context.UserRooms.Any(x => x.UserId == user.Id && x.RoomId == room.Id))
            {
                _context.UserRooms.Add(userRoom);
                _context.SaveChanges();
            }
            else
                throw new ApplicationException(Properties.resultMessages.UserRoomExists);

            return userRoom;
        }

        public IEnumerable<UserRoom> GetAll()
        {
            return _context.UserRooms
                .Include(x => x.User)
                .Include(x => x.Room);
        }

        public User GetOwner(Room roomParam)
        {
            var room = _context.Rooms
                .Include(x => x.RoomUsers)
                .SingleOrDefault(x => x.Id == roomParam.Id);

            if (room == null)
                throw new ApplicationException(Properties.resultMessages.RoomNotFound);

            var owner = _context.UserRooms
                .SingleOrDefault(x => x.RoomId == room.Id && x.Owner == true);

            if (owner == null)
                throw new ApplicationException(Properties.resultMessages.UserNotFound);

            return owner.User;
        }

        public UserRoom GetByIds(int userId, int roomId)
        {
            return _context.UserRooms
                .Include(x => x.User)
                .Include(x => x.Room)
                .SingleOrDefault(x => x.UserId == userId && x.RoomId == roomId);
        }

        public IEnumerable<Room> GetRoomsByUserId(int userId)
        {
            return _context.UserRooms
                .Include(x => x.User)
                .Include(x => x.Room)
                .Where(x => x.UserId == userId)
                .Select(x => x.Room)
                .Include(x => x.RoomUsers)
                .ThenInclude(x => x.User);
        }

        public IEnumerable<User> GetUsersByRoomId(int roomId)
        {
            return _context.UserRooms
                .Include(x => x.User)
                .Include(x => x.Room)
                .Where(x => x.RoomId == roomId)
                .Select(x => x.User);
        }

        public void ChangeOwner(User newOwner, Room room)
        {
            var userRoom = _context.UserRooms.Find(newOwner.Id, room.Id);

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
            _context.SaveChanges();
        }

        public void DeleteUserFromRoom(User user, Room room)
        {
            var userRoom = _context.UserRooms.Find(user.Id, room.Id);
            if (userRoom != null)
            {
                _context.UserRooms.Remove(userRoom);
                _context.SaveChanges();
            }
        }

        public void Delete(UserRoom userRoom)
        {
            _context.UserRooms.Remove(userRoom);
            _context.SaveChanges();
        }
    }
}
