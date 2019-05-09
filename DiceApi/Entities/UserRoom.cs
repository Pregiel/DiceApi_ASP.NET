using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DiceApi.Entities
{
    public class UserRoom
    {
        public int UserId { get; set; }
        public int RoomId { get; set; }

        public virtual User User { get; set; }
        public virtual Room Room { get; set; }

        public bool Owner { get; set; }

        public UserRoom() { }
        public UserRoom(User user, Room room, bool owner)
        {
            UserId = user.Id;
            RoomId = room.Id;
            User = user;
            Room = room;
            Owner = owner;
        }
    }
}
