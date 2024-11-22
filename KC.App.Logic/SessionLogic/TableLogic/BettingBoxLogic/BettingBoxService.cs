using KC.App.Logic.SessionLogic.TableLogic.BettingBoxLogic.HandLogic;
using KC.App.Models.Classes;
using LanguageExt;

namespace KC.App.Logic.SessionLogic.TableLogic.BettingBoxLogic
{
    public static class BettingBoxService
    {
        public static BettingBox CreateEmptyBettingBox(int idx) => new BettingBox(idx,[HandService.CreateEmptyHand()], Option<Player>.None);
    }
}
