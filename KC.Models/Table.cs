using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;

namespace KC.Models
{
    public class Table(Guid TableId, ShoeProperties ShoeProperties, ImmutableList<BettingBox> Boxes)
    {
        public Guid TableId { get; } = TableId;
        public ImmutableList<BettingBox> Boxes { get; } = Boxes;
        //public ImmutableList<Hand> Hands => Boxes.SelectMany(b => b.Hands).ToImmutableList();
        public ShoeProperties ShoeProperties { get; } = ShoeProperties;
        public int CurrentHandInTurn { get; } = -1;
    }
}
