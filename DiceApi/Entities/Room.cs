using System.Collections.Generic;

namespace DiceApi.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public virtual ICollection<UserRoom> RoomUsers { get; set; }
        public virtual ICollection<Roll> Rolls { get; set; }
    }
}
