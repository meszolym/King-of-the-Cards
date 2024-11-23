using System.Collections.Immutable;
using KC.App.Logic.SessionLogic.TableLogic.BettingBoxLogic;
using KC.App.Models.Classes;
using KC.App.Models.Records;
using LanguageExt;

namespace KC.App.Logic.SessionLogic.TableLogic
{
    public static class TableExtensions
    {
        public static Option<BettingBox> GetBettingBox(this Table table, int boxIdx) =>
            table.Boxes.ElementAtOrDefault(boxIdx);

        public static Seq<BettingBox> BoxesInPlay(this Table table) => new(table.Boxes.Where(box => box.Hands[0].Bet > 0).OrderBy(b => b.Idx));

        public static Unit Reset(this Table table)
        {
            table.Boxes.ForEach(b => b.ClearHands());
            return Unit.Default;
        }
    }
}
