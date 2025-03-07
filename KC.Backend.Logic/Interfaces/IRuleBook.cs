using KC.Backend.Models;

namespace KC.Backend.Logic.Interfaces;

public interface IRuleBook
{
    List<Move> GetPossibleActionsOnHand(Hand hand);
    HandValue GetValue(Hand hand);
    string GetHandValueString(Hand hand);
    bool CanBeSplit(Hand hand);
    bool CanDouble(Hand hand);
    bool DealerShouldHit(Hand hand);
    int BlackjackPayout { get; }
    int StandardPayout { get; }
    int BjVsBjPayout { get; }
}