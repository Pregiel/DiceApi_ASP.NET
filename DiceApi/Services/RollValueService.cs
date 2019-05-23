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
        IEnumerable<RollValue> GetRollValues(Roll roll);
    }
    public class RollValueService : Service, IRollValueService
    {
        public RollValueService() : base() { }
        public RollValueService(DataContext context) : base(context) { }

        public RollValue Create(RollValue rollValue)
        {
            if (_context.Rolls.SingleOrDefault(x => x.Id == rollValue.RollId) == null)
                throw new ApplicationException(Properties.resultMessages.RollNotFound);

            _context.RollValues.Add(rollValue);
            _context.SaveChanges();

            return rollValue;
        }

        public IEnumerable<RollValue> GetAll()
        {
            return _context.RollValues
                .Include(x => x.Roll);
        }

        public IEnumerable<RollValue> GetRollValues(Roll roll)
        {
            return _context.RollValues
                .Include(x => x.Roll)
                .Where(x => x.RollId == roll.Id);
        }
    }
}
