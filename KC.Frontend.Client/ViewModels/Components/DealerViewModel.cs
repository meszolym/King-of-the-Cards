using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicData;
using KC.Shared.Models.GameItems;
using ReactiveUI;

namespace KC.Frontend.Client.ViewModels.Components;

public partial class DealerViewModel : ReactiveObject
{
    public ObservableCollection<CardViewModel> Cards { get; private set; } = [];

    public DealerViewModel(IEnumerable<Card> tableDealerVisibleCards) => Cards.AddRange(tableDealerVisibleCards.Select(c => new CardViewModel(c)));
    
    public void AddCard(Card card) => Cards.Add(new CardViewModel(card));
}