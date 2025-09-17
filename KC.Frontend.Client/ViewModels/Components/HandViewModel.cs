using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DynamicData;
using KC.Shared.Models.Dtos;
using KC.Shared.Models.GameItems;
using KC.Shared.Models.Misc;
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
        
        [Reactive]
        private string _outcomeText = string.Empty;
        
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

        public async Task ShowOutcome(Outcome outcome)
        {
            switch (outcome)
            {
                case Outcome.Win:
                    OutcomeText = "You win!";
                    break;
                case Outcome.Lose:
                    OutcomeText = "You lose!";
                    break;
                case Outcome.Push:
                    OutcomeText = "Push!";
                    break;
                case Outcome.BjWin:
                    OutcomeText = "BJ - You win!";
                    break;
                case Outcome.BjLose:
                    OutcomeText = "BJ - You lose!";
                    break;
                case Outcome.BjPush:
                    OutcomeText = "BJ - Push!";
                    break;
                case Outcome.None:
                default:
                    OutcomeText = string.Empty;
                    break;
            }
            await Task.Delay(Constants.ShowOutcomeDelayMs);
            OutcomeText = string.Empty;
        }
    }