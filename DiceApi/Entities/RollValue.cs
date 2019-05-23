using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DiceApi.Entities
{
    public class RollValue
    {
        public int Id { get; set; }
        public int MaxValue { get; set; }
        public int Value { get; set; }
        public int RollId { get; set; }

        private Roll _roll;
        public virtual Roll Roll
        {
            get
            {
                return _roll;
            }
            set
            {
                _roll = value;
                RollId = value.Id;
            }
        }
    }
}
