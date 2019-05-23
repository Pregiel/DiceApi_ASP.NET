using DiceApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceApi.Services
{
    public abstract class Service
    {
        protected DataContext _context;

        public DataContext Context
        {
            get => _context;
            set => _context = value;
        }

        public Service() { }

        public Service(DataContext context)
        {
            _context = context;
        }
    }
}
