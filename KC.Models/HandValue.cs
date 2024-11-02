using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Models
{
    public record HandValue(int Value, bool IsBlackJack, bool IsPair, bool IsSoft)
    {
        public override string ToString() => this switch
        {
            { IsBlackJack: true } => "BJ",
            { IsSoft: true, IsPair: true } => "P11", //Pair of Aces
            { IsSoft: true } => $"S{Value}",
            { IsPair: true } => $"P{Value / 2}",
            _ => Value.ToString()
        };

        public void Deconstruct(out int Value, out bool IsBlackJack, out bool IsPair, out bool IsSoft)
        {
            Value = this.Value;
            IsBlackJack = this.IsBlackJack;
            IsPair = this.IsPair;
            IsSoft = this.IsSoft;
        }
    }
}
