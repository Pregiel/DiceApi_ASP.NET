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

        private User _user;
        public virtual User User
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
                UserId = value.Id;
            }
        }
        private Room _room;
        public virtual Room Room
        {
            get
            {
                return _room;
            }
            set
            {
                _room = value;
                RoomId = value.Id;
            }
        }

        public bool Owner { get; set; }
    }
}
