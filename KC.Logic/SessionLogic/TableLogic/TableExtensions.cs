using System.Collections.Immutable;
using KC.App.Logic.SessionLogic.TableLogic.BettingBoxLogic;
using KC.App.Models.Classes;
using KC.App.Models.Records;
using LanguageExt;

namespace KC.App.Logic.SessionLogic.TableLogic
{
    public static class TableExtensions
    {
        public static Fin<BettingBox> GetBettingBox(this Table table, int boxIdx) =>
            Try.lift(() => table.Boxes.ElementAt(boxIdx)).Run();

        public static Seq<BettingBox> BoxesInPlay(this Table table) => new(table.Boxes.Where(box => box.Hands[0].Bet > 0));

        public static Fin<Unit> Reset(this Table table)
        {
            table.Boxes.ForEach(b => b.ClearHands());
            return Unit.Default;
        }
    }
}
