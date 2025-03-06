using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.App.Backend.Models.Structs;

namespace KC.App.Backend.Models.Classes.Hand
{
    public class PlayerHand(List<Card> cards, double bet, bool splittable) : Hand(cards)
    {
        public double Bet { get; set; } = bet;
        public bool Splittable { get; set; } = splittable;
        public override IEnumerable<Card> CardsVisible
        {
            get => Cards;
        }
    }
}
