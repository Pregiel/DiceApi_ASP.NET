﻿using DiceApi.Entities;
using DiceApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceApi.Services
{
    public interface IRoomService
    {
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

        public Room Create(Room room, string password)
        {
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
            return _context.Rooms;
        }

        public Room GetById(int id)
        {
            return _context.Rooms.Find(id);
        }

        public void Update(Room roomParam, string password = null)
        {
            var room = _context.Rooms.Find(roomParam.Id);

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
            var room = _context.Rooms.Find(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                _context.SaveChanges();
            }
        }
    }
}