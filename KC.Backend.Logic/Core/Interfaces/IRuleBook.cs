using System.Collections.Generic;
using KC.Backend.Models.GameItems;
using KC.Shared.Models.GameItems;

namespace KC.Backend.Logic.Core.Interfaces;

public interface IRuleBook
{
    List<Move> GetPossibleActionsOnHand(Hand hand);
    HandValue GetValue(Hand hand);
    string GetHandValueString(Hand hand);
    bool CanBeSplit(Hand hand);
    bool CanDouble(Hand hand);
    bool DealerShouldHit(Hand hand);
    bool DealerCheckBlackJack(Hand hand);
    int BlackjackPayoutMultiplier { get; }
    int StandardPayoutMultiplier { get; }
    int BjVsBjPayoutMultiplier { get; }
}