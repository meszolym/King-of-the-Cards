using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using KC.Frontend.Client.Services;
using KC.Shared.Models.GameItems;
using ReactiveUI;

namespace KC.Frontend.Client.ViewModels.Components;

public partial class DealerViewModel : ReactiveObject
{
    public ObservableCollection<CardViewModel> Cards { get; private set; } = [];

    public DealerViewModel(IEnumerable<Card> tableDealerVisibleCards)
    {
        Cards.AddRange(tableDealerVisibleCards.Select(c => new CardViewModel(c)));
        
        ExternalCommunicatorService.SignalREvents.HandsUpdated.ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(s => UpdateDealer(s.Table.DealerVisibleCards));
    }

    public void AddCard(Card card) => Cards.Add(new CardViewModel(card));

    private void UpdateDealer(IEnumerable<Card> tableDealerVisibleCards)
    {
        Cards.Clear();
        tableDealerVisibleCards.ToList().ForEach(c => Cards.Add(new CardViewModel(c)));
    }
}