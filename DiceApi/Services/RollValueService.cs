using DiceApi.Entities;
using DiceApi.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceApi.Services
{
    public interface IRollValueService
    {
        Task<RollValue> Create(RollValue rollValue);
        Task<IEnumerable<RollValue>> GetAll();
        Task<IEnumerable<RollValue>> GetRoomRolls(Roll roll);
    }
    public class RollValueService : IRollValueService
    {
        private DataContext _context;

        public RollValueService(DataContext context)
        {
            _context = context;
        }

        public async Task<RollValue> Create(RollValue rollValue)
        {
            await _context.RollValues.AddAsync(rollValue);
            await _context.SaveChangesAsync();

            return rollValue;
        }

        public async Task<IEnumerable<RollValue>> GetAll()
        {
            return await _context.RollValues
                .Include(x => x.Roll)
                .ToListAsync();
        }

        public async Task<IEnumerable<RollValue>> GetRoomRolls(Roll roll)
        { 
            return await _context.RollValues
                .Include(x => x.Roll)
                .Where(x => x.RollId == roll.Id)
                .ToListAsync();
        }
    }
}
