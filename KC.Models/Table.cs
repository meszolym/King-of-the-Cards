using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;

namespace KC.Models
{
    public class Table(ShoeProperties shoeProperties)
    {
        public Guid TableId { get; set; } = Guid.NewGuid();
        public ImmutableList<BettingBox> Boxes { get; set; } = [];
        public ImmutableList<Hand> Hands => Boxes.SelectMany(b => b.Hands).ToImmutableList();
        public ShoeProperties ShoeProperties { get; set; } = shoeProperties;
        public Option<ShuffledShoe> Shoe { get; set; }
        public int CurrentHandInTurn { get; set; } = -1;
    }
}
