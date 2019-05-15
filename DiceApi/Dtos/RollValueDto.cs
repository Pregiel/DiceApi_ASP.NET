using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DiceApi.Dtos
{
    public class RollValueDto
    {
        public int MaxValue { get; set; }
        public int Value { get; set; }
    }
}
