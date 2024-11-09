using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Models.Classes;
using KC.Models.Records;
using LanguageExt;

namespace KC.Logic.SessionLogic.TableLogic
{
    public static class TableExtensions
    {
        public static Fin<BettingBox> GetBettingBox(this Table table, int boxIdx) =>
            Try.lift(() => table.Boxes.ElementAt(boxIdx)).Run();
    }
}
