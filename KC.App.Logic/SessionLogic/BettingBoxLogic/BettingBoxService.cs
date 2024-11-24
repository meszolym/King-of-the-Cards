using KC.App.Logic.SessionLogic.HandLogic;
using KC.App.Models.Classes;

namespace KC.App.Logic.SessionLogic.BettingBoxLogic
{
    internal static class BettingBoxService
    {
        internal static BettingBox CreateEmptyBettingBox(int idx) => new(idx, [HandService.CreateEmptyPlayerHand()], null);
    }
}
