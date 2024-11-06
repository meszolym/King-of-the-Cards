using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Models.Structs;


//TODO: Create a builder for this struct
public readonly struct HandValue(int value, bool isBlackJack, bool isPair, bool isSoft)
{
    public int Value { get; } = value;
    public bool IsBlackJack { get; } = isBlackJack;
    public bool IsPair { get; } = isPair;
    public bool IsSoft { get; } = isSoft;

    public override string ToString() => this switch
    {
        { IsBlackJack: true } => "BJ",
        { IsSoft: true, IsPair: true } => "P11", //Pair of Aces
        { IsSoft: true } => $"S{Value}",
        { IsPair: true } => $"P{Value / 2}",
        _ => Value.ToString()
    };

}