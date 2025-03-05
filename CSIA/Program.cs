using Avalonia;
using Avalonia.ReactiveUI;
using System;
using System.IO;
using CSIA.Views;

namespace CSIA;

sealed class Program
{
    
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        Environment.SetEnvironmentVariable("SK_METAL", "1");

        // Start the Avalonia application
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseSkia() // Ensure Skia is used as the rendering engine
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
