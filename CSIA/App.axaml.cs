using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CSIA.ViewModels;
using CSIA.Views;

namespace CSIA;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        // {
        //     desktop.MainWindow = new MainWindow
        //     {
        //         DataContext = new MainWindowViewModel(),
        //     };
        // }
        //
        // base.OnFrameworkInitializationCompleted();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow();
            mainWindow.DataContext = new MainWindowViewModel(mainWindow, null, null); // Pass MainWindow to the ViewModel
            desktop.MainWindow = mainWindow; // Set the MainWindow
        }

        base.OnFrameworkInitializationCompleted();
    }
}