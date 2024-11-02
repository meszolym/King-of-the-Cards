using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Models
{
    public record HandValue(int value, bool isBlackJack, bool isPair, bool isSoft)
    {
        public int Value { get; set; } = value;
        public bool IsBlackJack { get; set; } = isBlackJack;
        public bool IsPair { get; set; } = isPair;
        public bool IsSoft { get; set; } = isSoft;

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
