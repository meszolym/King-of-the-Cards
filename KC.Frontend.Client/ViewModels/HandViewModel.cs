using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using DynamicData;
using KC.Frontend.Client.Extensions;
using KC.Frontend.Client.Models;
using KC.Shared.Models.GameItems;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace KC.Frontend.Client.ViewModels;

public partial class HandViewModel : ReactiveObject
{
    [Reactive]
    private double _bet;

    private readonly SourceList<Card> _cardsInHand = new SourceList<Card>();
    
    [Reactive]
    private List<ImageWithRect> _cardImageWithRects = [];
    
    public void AddCard(Card card) => _cardsInHand.Add(card);
    public void RemoveLastCard() => _cardsInHand.RemoveAt(_cardsInHand.Count - 1);
    
    public HandViewModel()
    {
        _cardsInHand.Connect()
            .OnItemAdded(c =>
            {
                Rect lastImageRect;
                lastImageRect = CardImageWithRects.Count > 0 ? CardImageWithRects.Last().Bounds : new Rect(-10, -10, 140, 190);
                var newBounds = new Rect(lastImageRect.X + 50, lastImageRect.Y - 50, lastImageRect.Width, lastImageRect.Height);
                var newItem = new ImageWithRect(c.ImagePath(), newBounds);
                CardImageWithRects.Add(newItem);
                
            })
            .OnItemRemoved(_ =>
            {
                // Remove the last item from the list (this is only used with splits)
                CardImageWithRects.RemoveAt(CardImageWithRects.Count - 1);
            })
            .Subscribe();
        
        _cardsInHand.Add(Card.WithSuitAndFace(Card.CardSuit.Spades, Card.CardFace.Ace));
        _cardsInHand.Add(Card.WithSuitAndFace(Card.CardSuit.Spades, Card.CardFace.Two));
        
    }
}