using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Models.Interfaces;
using LanguageExt;

namespace KC.Models
{
    public record Table(Guid Id, ShoeProperties ShoeProperties, ImmutableList<BettingBox> Boxes, int CurrentHandInTurn)
        : IIdentityBearer<Guid>
    {
        //public ImmutableList<Hand> Hands => Boxes.SelectMany(b => b.Hands).ToImmutableList();

        public void Deconstruct(out Guid Id, out ShoeProperties ShoeProperties, out ImmutableList<BettingBox> Boxes)
        {
            Id = this.Id;
            ShoeProperties = this.ShoeProperties;
            Boxes = this.Boxes;
        }
    }
}
