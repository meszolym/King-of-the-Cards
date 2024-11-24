using KC.App.Logic.SessionLogic.TableLogic.BettingBoxLogic.HandLogic;
using KC.App.Models.Classes;

namespace KC.App.Logic.SessionLogic.TableLogic.BettingBoxLogic
{
    public static class BettingBoxService
    {
        public static BettingBox CreateEmptyBettingBox(int idx) => new BettingBox(idx,[HandService.CreateEmptyPlayerHand()], null);
    }
}
