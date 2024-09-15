using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using CSIA.Views;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace CSIA.Backend;

public class PopUpDialog
{
    private ButtonResult ServRunButtonResult;
    
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

    public async Task ShowServerRunningMessage(Window owner)
    {
        var messageBox = MessageBoxManager.GetMessageBoxStandard(
            "FTP Server Already Online",
            $"This computer is already hosting an FTP server. Could not start a new one.",
            MsBox.Avalonia.Enums.ButtonEnum.OkAbort,
            MsBox.Avalonia.Enums.Icon.Error
        );
        ServRunButtonResult = await messageBox.ShowWindowDialogAsync(owner);
    }

    public async Task<string> ServerRunningFunction(Window owner, FTPServerWindow FTPWindow)
    {
        string ButtonResult = null;
        await ShowServerRunningMessage(owner);

        if (ServRunButtonResult.ToString() == "Abort")
        {
            FTPWindow.StopFtpServer();
            ButtonResult = "Aborted";
            
        }
        return ButtonResult;
    }
}