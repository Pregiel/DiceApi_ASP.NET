using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DiceApi.Dtos
{
    public class RollDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public int Value { get; set; }
        public int MaxValue { get; set; }
        public DateTime CreatedTime { get; set; }

    }
}
