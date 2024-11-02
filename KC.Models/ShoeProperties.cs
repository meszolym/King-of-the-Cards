using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Models
{
    public record ShoeProperties(int NumberOfDecks, [property: Range(0, 1)] double MaxShoePenetration, int ShuffleCardRadius)
    {
        public void Deconstruct(out int NumberOfDecks, out double MaxShoePenetration, out int ShuffleCardRadius)
        {
            NumberOfDecks = this.NumberOfDecks;
            MaxShoePenetration = this.MaxShoePenetration;
            ShuffleCardRadius = this.ShuffleCardRadius;
        }
    }
}
