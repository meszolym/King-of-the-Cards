using Avalonia;
using Avalonia.ReactiveUI;
using System;
using KC.Frontend.Client.Services;
using Splat;

namespace KC.Frontend.Client;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp().AfterSetup(RegisterServices)
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
    
    private static void RegisterServices(AppBuilder builder)
    {
        Locator.CurrentMutable.RegisterConstant(new ExternalCommunicatorService());
    }
}