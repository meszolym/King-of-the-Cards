using System.Collections.ObjectModel;
using KC.Shared.Models.GameItems;
using ReactiveUI;

namespace KC.Frontend.Client.ViewModels.Components;

public partial class DealerViewModel : ReactiveObject
{
    public ObservableCollection<CardViewModel> Cards { get; set; } = [];

    public DealerViewModel()
    {
        AddCard(Card.WithSuitAndFace(Card.CardSuit.None, Card.CardFace.None));
        AddCard(Card.WithSuitAndFace(Card.CardSuit.Hearts, Card.CardFace.Two));
    }
    
    public void AddCard(Card card) => Cards.Add(new CardViewModel(card));
}