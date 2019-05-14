using DiceApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceApi.Dtos
{
    public class RoomDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public UserInfoDto Owner { get; set; }
        public ICollection<UserInfoDto> Users { get; set; }
    }
}
