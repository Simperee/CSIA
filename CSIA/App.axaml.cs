using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using CSIA.Backend;
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
        
        var myTheme = new UIColors().ClientTheme;
        Current.Styles.Add(new UIColors.AppStyle(myTheme));
        Current.Styles.Add(new UIColors.MenuButtonStyle(myTheme));
        Current.Styles.Add(new UIColors.CloseButtonStyle(myTheme));
        Current.Styles.Add(new UIColors.MaximButtonStyle(myTheme));
        Current.Styles.Add(new UIColors.MinimButtonStyle(myTheme));
        Current.Styles.Add(new UIColors.LocalListBoxStyle(myTheme));
        Current.Styles.Add(new UIColors.RemoteListBoxStyle(myTheme));
        Current.Styles.Add(new UIColors.ActionButtonStyle(myTheme));
        
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow();
            mainWindow.DataContext = new MainWindowViewModel(mainWindow, null, null); // Pass MainWindow to the ViewModel
            desktop.MainWindow = mainWindow; // Set the MainWindow
        }

        base.OnFrameworkInitializationCompleted();
    }
}