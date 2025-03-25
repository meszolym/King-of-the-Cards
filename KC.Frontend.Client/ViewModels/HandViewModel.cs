using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DynamicData;
using KC.Frontend.Client.Extensions;
using KC.Frontend.Client.Models;
using KC.Shared.Models.GameItems;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace KC.Frontend.Client.ViewModels;

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
            Cards.Add(new CardViewModel(card,idx*CardOffsetX, _height*0.75+ idx*CardOffsetY, idx));
        }
        

        // private void UpdateCardPositions()
        // {
        //     if (_cardsItemsControl?.Items == null) return;
        //
        //     var itemCount = _cardsItemsControl.Items.Cast<object>().Count();
        //
        //     // Reverse the loop to set proper Z-index (first card on top)
        //     for (var i = 0; i < itemCount; i++)
        //     {
        //         var container = _cardsItemsControl.ContainerFromIndex(i); //as ContentPresenter;
        //         if (container != null)
        //         {
        //             // Position cards starting from bottom left
        //             Canvas.SetLeft(container, i * CardOffsetX);
        //             Canvas.SetBottom(container, i * CardOffsetY); // Use bottom instead of top
        //     
        //             // First card (index 0) should have highest Z-index
        //             container.ZIndex = i; //itemCount - i;
        //         }
        //     }
        // }
    }