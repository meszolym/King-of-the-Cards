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

 public class HandViewModel : ReactiveObject
    {
        private ObservableCollection<CardModel> _cards;
        public ObservableCollection<CardModel> Cards
        {
            get => _cards;
            set => this.RaiseAndSetIfChanged(ref _cards, value);
        }
        
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

        public HandViewModel()
        {
            Cards = new ObservableCollection<CardModel>();
            BetAmount = 0;
            IsActive = false;
            Cards.Add(new CardModel(Card.WithSuitAndFace(Card.CardSuit.Clubs, Card.CardFace.Ace)));
            Cards.Add(new CardModel(Card.WithSuitAndFace(Card.CardSuit.Diamonds, Card.CardFace.Seven)));
            Cards.Add(new CardModel(Card.WithSuitAndFace(Card.CardSuit.Hearts, Card.CardFace.Two)));
        }
        
        public void AddCard(CardModel card)
        {
            Cards.Add(card);
        }
    }

 public partial class CardModel : ReactiveObject
 {
     [Reactive]
     private Bitmap _imageSource;

     [Reactive] private Card _card;
     public CardModel(Card card)
     {
         Card = card;
         LoadImage();
     }
        
     private void LoadImage()
     {
         try
         {
             Uri imagePath = Card.ImagePath();
             ImageSource = new Bitmap(AssetLoader.Open(imagePath));
         }
         catch (Exception ex)
         {
             Console.WriteLine($"Error loading card image: {ex.Message}");
         }
     }
 }