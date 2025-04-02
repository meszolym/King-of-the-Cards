using System;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using KC.Frontend.Client.Extensions;
using KC.Shared.Models.GameItems;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace KC.Frontend.Client.ViewModels;

public partial class CardViewModel : ReactiveObject
{
    [Reactive]
    private Bitmap _imageSource;

    /// <summary>
    /// From left
    /// </summary>
    public double X { get; private init; }

    /// <summary>
    /// From bottom
    /// </summary>
    public double Y { get; private init; }
    public double Z { get; private init; }

    [Reactive]
    private Card _card;

    public CardViewModel(Card card, double x=0, double y=0, double z=0)
    {
        Card = card;
        X = x;
        Y = y;
        Z = z;
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