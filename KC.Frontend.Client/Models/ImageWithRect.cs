using System;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ReactiveUI;

namespace KC.Frontend.Client.Models;
public class ImageWithRect(Uri imagePath, Rect bounds) : ReactiveObject
{
    public Uri ImagePath { get; } = imagePath;
    public Bitmap Image { get; } = new Bitmap(AssetLoader.Open(imagePath));
    public Rect Bounds { get; } = bounds;
}