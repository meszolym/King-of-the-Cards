using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Models.Classes;
using KC.Models.Records;

namespace KC.Logic.TableLogic;

public static class TableExtensions
{
    public static BettingBox FindBox(this Table table, int boxId) => table.Boxes.FirstOrDefault(b => b.Id == boxId);
}