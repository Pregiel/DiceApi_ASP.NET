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
        Task<Room> Authenticate(int id, string password);
        Task<Room> Create(Room room, string password);
        Task<IEnumerable<Room>> GetAll();
        Task<Room> GetById(int id);
        Task Update(Room roomParam, string password = null);
        Task Delete(int id);
    }
    public class RoomService : IRoomService
    {
        private DataContext _context;

        public RoomService(DataContext context)
        {
            _context = context;
        }

        public async Task<Room> Authenticate(int id, string password)
        {
            var room = await _context.Rooms
                .Include(x => x.RoomUsers)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (room == null)
                throw new ApplicationException(Properties.resultMessages.RoomNotFound);

            if (!PasswordHelpers.VerifyPasswordHash(password, room.PasswordHash, room.PasswordSalt))
                throw new ApplicationException(Properties.resultMessages.CredentialsInvalid);

            return room;
        }

        public async Task<Room> Create(Room room, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ApplicationException(Properties.resultMessages.PasswordNull);

            byte[] passwordHash, passwordSalt;
            PasswordHelpers.CreatePasswordHash(password, out passwordHash, out passwordSalt);

            room.PasswordHash = passwordHash;
            room.PasswordSalt = passwordSalt;

            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            return room;
        }

        public async Task<IEnumerable<Room>> GetAll()
        {
            return await _context.Rooms
                .Include(r => r.RoomUsers)
                .ThenInclude(roomUsers => roomUsers.User)
                .ToListAsync();
        }

        public async Task<Room> GetById(int id)
        {
            return await _context.Rooms
                .Include(x => x.RoomUsers)
                .ThenInclude(roomUsers => roomUsers.User)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task Update(Room roomParam, string password = null)
        {
            var room = await _context.Rooms.FindAsync(roomParam.Id);

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
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var room = _context.Rooms.Find(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
            }
        }
    }
}
