﻿using DiceApi.Entities;
using DiceApi.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceApi.Services
{
    public interface IRollService
    {
        Roll Create(Roll roll);
        IEnumerable<Roll> GetAll();
        IEnumerable<Roll> GetRoomRolls(Room room);
    }
    public class RollService : Service, IRollService
    {
        public RollService() : base() { }
        public RollService(DataContext context) : base(context) { }

        public Roll Create(Roll roll)
        {
            if (_context.Users.SingleOrDefault(x => x.Id == roll.UserId) == null)
                throw new ApplicationException(Properties.resultMessages.UserNotFound);

            if (_context.Rooms.SingleOrDefault(x => x.Id == roll.RoomId) == null)
                throw new ApplicationException(Properties.resultMessages.RoomNotFound);

            _context.Rolls.Add(roll);
            _context.SaveChanges();

            return roll;
        }

        public IEnumerable<Roll> GetAll()
        {
            return _context.Rolls
                .Include(x => x.User)
                .Include(x => x.Room)
                .Include(x => x.RollValues);
        }

        public IEnumerable<Roll> GetRoomRolls(Room room)
        {
            return _context.Rolls
                .Include(x => x.User)
                .Include(x => x.Room)
                .Include(x => x.RollValues)
                .Where(x => x.RoomId == room.Id);
        }
    }
}
