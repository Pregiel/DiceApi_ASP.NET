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
        RollValue Create(RollValue rollValue);
        IEnumerable<RollValue> GetAll();
        IEnumerable<RollValue> GetRoomRolls(Roll roll);
    }
    public class RollValueService : IRollValueService
    {
        private DataContext _context;

        public RollValueService(DataContext context)
        {
            _context = context;
        }

        public RollValue Create(RollValue rollValue)
        {
            _context.RollValues.Add(rollValue);
            _context.SaveChanges();

            return rollValue;
        }

        public IEnumerable<RollValue> GetAll()
        {
            return _context.RollValues
                .Include(x => x.Roll);
        }

        public IEnumerable<RollValue> GetRoomRolls(Roll roll)
        { 
            return _context.RollValues
                .Include(x => x.Roll)
                .Where(x => x.RollId == roll.Id);
        }
    }
}
