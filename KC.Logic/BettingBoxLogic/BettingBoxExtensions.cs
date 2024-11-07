using KC.Models.Classes;
using LanguageExt;

namespace KC.Logic.BettingBoxLogic;

public static class BettingBoxExtensions
{
    public static Option<Hand> FindHand(this BettingBox box, int Idx) => box.Hands.ElementAtOrDefault(Idx);
}
