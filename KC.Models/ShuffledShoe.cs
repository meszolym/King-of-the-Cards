using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Models
{
    public class ShuffledShoe(int shuffleCardIndex, Queue<Card> cards)
    {
        public int ShuffleCardIndex { get; set; } = shuffleCardIndex;
        public Queue<Card> Cards { get; set; } = cards;
    }
}
