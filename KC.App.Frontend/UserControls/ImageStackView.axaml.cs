using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using System.Collections.Generic;

namespace KC.App.Frontend;

public partial class ImageStackView : UserControl
{
    public ImageStackView()
    {
        InitializeComponent();
    }

    public void FillImageStack(List<string> imagePaths, double imageWidth = 100, double imageHeight = 150, double overlapX = 20, double overlapY = 20)
    {
        double offsetX = 0;
        double offsetY = 0;

        StackCanvas.Children.Clear();

        for (int i = 0; i < imagePaths.Count; i++)
        {
            var image = new Image
            {
                Source = new Bitmap(imagePaths[i]),
                Width = imageWidth,
                Height = imageHeight
            };

            Canvas.SetLeft(image, offsetX);
            Canvas.SetTop(image, offsetY);

            StackCanvas.Children.Add(image);

            offsetX += overlapX;
            offsetY += overlapY;
        }
    }
}