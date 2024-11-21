using KC.App.Logic.SessionLogic.TableLogic.BettingBoxLogic.HandLogic;
using KC.App.Models.Classes;
using LanguageExt;

namespace KC.App.Logic.SessionLogic.TableLogic.BettingBoxLogic
{
    public static class BettingBoxService
    {
        public static BettingBox CreateEmptyBettingBox() => new BettingBox([HandService.CreateEmptyHand()], Option<Player>.None);
    }
}
