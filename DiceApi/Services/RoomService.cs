using DiceApi.Entities;
using DiceApi.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceApi.Services
{
    public interface IRoomService
    {
        Room Authenticate(int id, string password);
        Room Create(Room room, string password);
        IEnumerable<Room> GetAll();
        Room GetById(int id);
        void Update(Room roomParam, string password = null);
        void Delete(int id);
    }
    public class RoomService : IRoomService
    {
        private DataContext _context;

        public RoomService(DataContext context)
        {
            _context = context;
        }

        public Room Authenticate(int id, string password)
        {
            var room = _context.Rooms
                .Include(x => x.RoomUsers)
                .SingleOrDefault(x => x.Id == id);

            if (room == null)
                throw new ApplicationException(Properties.resultMessages.RoomNotFound);

            if (!PasswordHelpers.VerifyPasswordHash(password, room.PasswordHash, room.PasswordSalt))
                throw new ApplicationException(Properties.resultMessages.CredentialsInvalid);

            return room;
        }

        public Room Create(Room room, string password)
        {
            if (string.IsNullOrWhiteSpace(room.Title))
                throw new ApplicationException(Properties.resultMessages.TitleNull);

            if (string.IsNullOrWhiteSpace(password))
                throw new ApplicationException(Properties.resultMessages.PasswordNull);

            byte[] passwordHash, passwordSalt;
            PasswordHelpers.CreatePasswordHash(password, out passwordHash, out passwordSalt);

            room.PasswordHash = passwordHash;
            room.PasswordSalt = passwordSalt;

            _context.Rooms.Add(room);
            _context.SaveChanges();

            return room;
        }

        public IEnumerable<Room> GetAll()
        {
            return _context.Rooms
                .Include(r => r.RoomUsers)
                .ThenInclude(roomUsers => roomUsers.User);
        }

        public Room GetById(int id)
        {
            return _context.Rooms
                .Include(x => x.RoomUsers)
                .ThenInclude(roomUsers => roomUsers.User)
                .SingleOrDefault(x => x.Id == id);
        }

        public void Update(Room roomParam, string password = null)
        {
            var room = _context.Rooms.SingleOrDefault(x => x.Id == roomParam.Id);

            if (room == null)
                throw new ApplicationException(Properties.resultMessages.RoomNotFound);

            room.Title = roomParam.Title;
            room.RoomUsers = roomParam.RoomUsers;

            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                PasswordHelpers.CreatePasswordHash(password, out passwordHash, out passwordSalt);

                room.PasswordHash = passwordHash;
                room.PasswordSalt = passwordSalt;
            }

            _context.Rooms.Update(room);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var room = _context.Rooms.SingleOrDefault(x => x.Id == id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                _context.SaveChanges();
            }
        }
    }
}
