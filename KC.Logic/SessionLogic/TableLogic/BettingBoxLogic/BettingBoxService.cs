using KC.Logic.SessionLogic.TableLogic.BettingBoxLogic.HandLogic;
using KC.Models.Classes;
using LanguageExt;

namespace KC.Logic.SessionLogic.TableLogic.BettingBoxLogic
{
    public static class BettingBoxService
    {
        public static BettingBox CreateEmptyBettingBox() => new BettingBox([HandService.CreateEmptyHand()], Option<Player>.None);
    }
}
