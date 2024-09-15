using Avalonia.Controls;
using CSIA.Views;
using MsBox.Avalonia;

namespace CSIA.Backend;

public class PopUpDialog
{
    private FTPServerWindow FTPWindow = new FTPServerWindow();
    
    public async void ShowAccessDeniedMessage(Window owner, string path)
    {
        var messageBox = MessageBoxManager.GetMessageBoxStandard(
            "Access Denied",
            $"You do not have permission to access the directory: {path}",
            MsBox.Avalonia.Enums.ButtonEnum.Ok,
            MsBox.Avalonia.Enums.Icon.Error
        );

        await messageBox.ShowWindowDialogAsync(owner); // Show the popup
    }

    public async void ShowHostingMessage(Window owner, string ip, string port)
    {
        var messageBox = MessageBoxManager.GetMessageBoxStandard(
            "FTP Server Online",
            $"This computer is now hosting on IP: {ip} | Port number: {port}",
            MsBox.Avalonia.Enums.ButtonEnum.Ok,
            MsBox.Avalonia.Enums.Icon.Info
        );
        await messageBox.ShowWindowDialogAsync(owner); // Show the popup
    }
    
    public async void ShowErrorMessage(Window owner, string ErrMessage)
    {
        var messageBox = MessageBoxManager.GetMessageBoxStandard(
            "Error",
            $"{ErrMessage}",
            MsBox.Avalonia.Enums.ButtonEnum.Ok,
            MsBox.Avalonia.Enums.Icon.Error
        );
        await messageBox.ShowWindowDialogAsync(owner); // Show the popup
    }

    public async void ShowServerRunningMessage(Window owner)
    {
        var messageBox = MessageBoxManager.GetMessageBoxStandard(
            "FTP Server Already Online",
            $"This computer is already hosting an FTP server. Could not start a new one.",
            MsBox.Avalonia.Enums.ButtonEnum.OkAbort,
            MsBox.Avalonia.Enums.Icon.Error
        );
        await messageBox.ShowWindowDialogAsync(owner); // Show the popup
    }

    public async void ServerRunningFunction(Window owner)
    {
        var sex = ShowServerRunningMessage(owner);
        
    }
}