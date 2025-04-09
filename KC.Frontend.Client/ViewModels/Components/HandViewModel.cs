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
        private double _betAmount;
        
        private double _height = 648; //gombócból is sok :)
        
        public HandViewModel(HandReadDto sourceDto)
        {
            foreach (var card in sourceDto.Cards) AddCard(card);
            BetAmount = sourceDto.Bet;
        }
        public HandViewModel(){}
        
        private const int CardOffsetX = 23; // Horizontal offset for each card
        private const int CardOffsetY = -23; // Negative offset for each card
        
        public void AddCard(Card card)
        {
            var idx = Cards.Count; 
            Cards.Add(new CardViewModel(card,idx*CardOffsetX, _height*0.65+ idx*CardOffsetY, idx));
        }
    }