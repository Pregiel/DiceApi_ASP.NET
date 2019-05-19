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
        public string Username { get; set; }
        public int RoomId { get; set; }
        public ICollection<RollValueDto> RollValues { get; set; }
        public string CreatedTime { get; set; }
        public int Modifier { get; set; }
    }
}
