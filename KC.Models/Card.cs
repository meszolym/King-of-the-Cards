﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Models
{
    public record Card(CardSuit Suit, CardFace Face)
    {
        public void Deconstruct(out CardSuit Suit, out CardFace Face)
        {
            Suit = this.Suit;
            Face = this.Face;
        }
    }

    public enum CardFace
    {
        None = 0,
        Ace = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13
    }

    public enum CardSuit
    {
        None,
        Spades,
        Hearts,
        Diamonds,
        Clubs
    }
}
