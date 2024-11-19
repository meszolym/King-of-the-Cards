using System.Collections.Immutable;
using KC.Logic.SessionLogic.TableLogic.BettingBoxLogic;
using KC.Models.Classes;
using KC.Models.Records;
using LanguageExt;

namespace KC.Logic.SessionLogic.TableLogic
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
