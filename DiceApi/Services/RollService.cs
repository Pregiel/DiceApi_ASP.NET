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
        Task<Roll> CreateAsync(Roll roll);
        Task<IEnumerable<Roll>> GetAll();
        Task<IEnumerable<Roll>> GetRoomRolls(Room room);
    }
    public class RollService : IRollService
    {
        private DataContext _context;

        public RollService(DataContext context)
        {
            _context = context;
        }

        public async Task<Roll> CreateAsync(Roll roll)
        {
            await _context.Rolls.AddAsync(roll);
            await _context.SaveChangesAsync();

            return roll;
        }

        public async Task<IEnumerable<Roll>> GetAll()
        {
            return await _context.Rolls
                .Include(x => x.User)
                .Include(x => x.Room)
                .Include(x => x.RollValues)
                .ToListAsync();
        }

        public async Task<IEnumerable<Roll>> GetRoomRolls(Room room)
        { 
            return await _context.Rolls
                .Include(x => x.User)
                .Include(x => x.Room)
                .Include(x => x.RollValues)
                .Where(x => x.RoomId == room.Id)
                .ToListAsync();
        }
    }
}
