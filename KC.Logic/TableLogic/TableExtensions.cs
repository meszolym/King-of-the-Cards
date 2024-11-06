using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Models.Classes;
using KC.Models.Records;
using LanguageExt;

namespace KC.Logic.TableLogic;

public static class TableExtensions
{
    public static Option<BettingBox> FindBox(this Table table, int boxId) => table.Boxes.FirstOrDefault(b => b.Id == boxId);
}