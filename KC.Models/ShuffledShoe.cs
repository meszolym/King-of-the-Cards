using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Models
{
    public record ShuffledShoe(Guid TableId, int ShuffleCardIndex, ImmutableQueue<Card> Cards)
    {
        public void Deconstruct(out Guid TableId, out int ShuffleCardIndex, out ImmutableQueue<Card> Cards)
        {
            TableId = this.TableId;
            ShuffleCardIndex = this.ShuffleCardIndex;
            Cards = this.Cards;
        }
    }
}
