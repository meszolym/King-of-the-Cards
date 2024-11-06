using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Models.Records;

namespace KC.Models.Classes;

public class Session(Guid id, Table table)
{
    public Guid Id { get; } = id;
    public Table Table { get; } = table;
    public int CurrentBoxIdx { get; set; } = -1; //invalid index
    public int CurrentHandIdx { get; set; } = -1; //invalid index
}