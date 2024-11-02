using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;

namespace KC.Models
{
    public class Table(ShoeProperties ShoeProperties)
    {
        public Guid TableId { get; } = Guid.NewGuid();
        public ImmutableList<BettingBox> Boxes { get; } = [];
        public ImmutableList<Hand> Hands => Boxes.SelectMany(b => b.Hands).ToImmutableList();
        public ShoeProperties ShoeProperties { get; } = ShoeProperties;
        public Option<ShuffledShoe> Shoe { get; }
        public int CurrentHandInTurn { get; } = -1;
    }
}
