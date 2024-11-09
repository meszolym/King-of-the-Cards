using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Models.Interfaces;
using KC.Models.Records;

namespace KC.Models.Classes;

public class Session(Guid id, Table table) : IIdentityBearer<Guid>
{
    public Guid Id { get; } = id;
    public Table Table { get; } = table;
    //public IObservable<long> Timer { get; set; }
    //public IDisposable TimerSubscription { get; set; }
    public bool CanPlaceBets { get; set; } = true;
    public int CurrentBoxIdx { get; set; } = -1; //invalid index
    public int CurrentHandIdx { get; set; } = -1; //invalid index
}