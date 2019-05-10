using DiceApi.Entities;
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
    public class RollService : IRollService
    {
        private DataContext _context;

        public RollService(DataContext context)
        {
            _context = context;
        }

        public Roll Create(Roll roll)
        {
            _context.Rolls.Add(roll);
            _context.SaveChanges();

            return roll;
        }

        public IEnumerable<Roll> GetAll()
        {
            return _context.Rolls
                .Include(x => x.User)
                .Include(x => x.Room);
        }

        public IEnumerable<Roll> GetRoomRolls(Room room)
        { 
            return _context.Rolls
                .Include(x => x.User)
                .Include(x => x.Room)
                .Where(x => x.RoomId == room.Id);
        }
    }
}
