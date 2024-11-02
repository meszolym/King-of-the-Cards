﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Models
{
    public class ShuffledShoe(Guid TableId, int ShuffleCardIndex, ImmutableQueue<Card> Cards)
    {
        public Guid TableId { get; } = TableId;
        public int ShuffleCardIndex { get; } = ShuffleCardIndex;
        public ImmutableQueue<Card> Cards { get; } = Cards;
    }
}
