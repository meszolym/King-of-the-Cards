using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Models
{
    public record ShoeProperties(int NumberOfDecks, double MaxShoePenetration, int ShuffleCardRadius)
    {
        public int NumberOfDecks { get; } = NumberOfDecks;

        [Range(0, 1)]
        public double MaxShoePenetration { get; } = MaxShoePenetration;
        public int ShuffleCardRadius { get; } = ShuffleCardRadius;
    }
}
