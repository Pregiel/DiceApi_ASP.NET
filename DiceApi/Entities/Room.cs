﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceApi.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public virtual ICollection<UserRoom> RoomUsers { get; set; }
    }
}