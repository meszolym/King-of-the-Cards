using System.Collections.ObjectModel;
using System.Linq;
using DynamicData;
using KC.Shared.Models.Dtos;
using KC.Shared.Models.GameItems;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace KC.Frontend.Client.ViewModels.Components;

 public partial class HandViewModel : ReactiveObject
 {
        public ObservableCollection<CardViewModel> Cards { get; set; } = [];
        
        [Reactive]
        private decimal _betAmount;

        [Reactive]
        private bool _isPartOfSplit;
        
        
        public HandViewModel(HandReadDto sourceDto)
        {
            foreach (var card in sourceDto.Cards) AddCard(card);
            BetAmount = (decimal)sourceDto.Bet;
        }
        public HandViewModel(){}
        
        private const int CardOffsetX = 23; // Horizontal offset for each card
        private const int CardOffsetY = -23; // Negative offset for each card
        
        public void AddCard(Card card)
        {
            var idx = Cards.Count; 
            Cards.Add(new CardViewModel(card,idx*CardOffsetX, idx*CardOffsetY, idx));
        }
    }