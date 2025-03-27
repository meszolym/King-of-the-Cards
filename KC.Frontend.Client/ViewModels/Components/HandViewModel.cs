using System.Collections.ObjectModel;
using KC.Shared.Models.GameItems;
using ReactiveUI;

namespace KC.Frontend.Client.ViewModels.Components;

 public partial class HandViewModel : ReactiveObject
    {
        public ObservableCollection<CardViewModel> Cards { get; set; }
        
        private decimal _betAmount;
        public decimal BetAmount
        {
            get => _betAmount;
            set => this.RaiseAndSetIfChanged(ref _betAmount, value);
        }
        
        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => this.RaiseAndSetIfChanged(ref _isActive, value);
        }
        
        private double _height = 648; //gombócból is sok :)
        
        public HandViewModel()
        {
            Cards = new ObservableCollection<CardViewModel>();
            BetAmount = 0;
            IsActive = false;
            AddCard(Card.WithSuitAndFace(Card.CardSuit.Clubs, Card.CardFace.Ace));
            AddCard(Card.WithSuitAndFace(Card.CardSuit.Diamonds, Card.CardFace.Seven));
            AddCard(Card.WithSuitAndFace(Card.CardSuit.Hearts, Card.CardFace.Two));
        }
        
        private const int CardOffsetX = 23; // Horizontal offset for each card
        private const int CardOffsetY = -23; // Negative offset for each card
        
        public void AddCard(Card card)
        {
            var idx = Cards.Count; 
            Cards.Add(new CardViewModel(card,idx*CardOffsetX, _height*0.65+ idx*CardOffsetY, idx));
        }
    }