using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Models
{
    public record HandValue(int Value, bool IsBlackJack, bool IsPair, bool IsSoft)
    {
        public int Value { get; } = Value;
        public bool IsBlackJack { get; } = IsBlackJack;
        public bool IsPair { get; } = IsPair;
        public bool IsSoft { get; } = IsSoft;

        public override string ToString() => this switch
        {
            { IsBlackJack: true } => "BJ",
            { IsSoft: true, IsPair: true } => "P11", //Pair of Aces
            { IsSoft: true } => $"S{Value}",
            { IsPair: true } => $"P{Value / 2}",
            _ => Value.ToString()
        };

    }
}
