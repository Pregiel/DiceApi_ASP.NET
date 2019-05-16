using DiceApi.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DiceApi.Entities
{
    public class Roll
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }

        public virtual User User { get; set; }
        public virtual Room Room { get; set; }

        public virtual ICollection<RollValue> RollValues { get; set; }

        public DateTime CreatedTime { get; set; }

        public Roll() { }

        public Roll(User user, Room room, ICollection<RollValue> rollValues)
        {
            UserId = user.Id;
            RoomId = room.Id;
            User = user;
            Room = room;
            RollValues = rollValues;
        }
    }
}
