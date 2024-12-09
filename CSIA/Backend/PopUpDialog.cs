using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using CSIA.Views;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace CSIA.Backend;

public class PopUpDialog
{
    private ButtonResult ServRunButtonResult;
    public ButtonResult OkButton;
    public ButtonResult LossButton;
    public ButtonResult SaveButton;
    public ButtonResult ExistButton;
    public string ButtonResult;
    public async Task<ButtonResult> ShowDataLossMessage(Window owner)
    {
        var messageBox = MessageBoxManager.GetMessageBoxStandard(
            "Warning",
            $"Are you sure? All data will be lost.",
            MsBox.Avalonia.Enums.ButtonEnum.YesNo,
            MsBox.Avalonia.Enums.Icon.Warning
        );

        LossButton = await messageBox.ShowWindowDialogAsync(owner); // Show the popup
        return LossButton;
    }
    
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

    public async Task<ButtonResult> ShowHostingMessage(Window owner, string ip, string port)
    {
        var messageBox = MessageBoxManager.GetMessageBoxStandard(
            "FTP Server Online",
            $"This computer is now hosting on IP: {ip} | Port number: {port}",
            MsBox.Avalonia.Enums.ButtonEnum.Ok,
            MsBox.Avalonia.Enums.Icon.Info
        );
        OkButton = await messageBox.ShowWindowDialogAsync(owner); // Show the popup
        return OkButton;
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
    public async void ShowAuthFailMessage(Window owner)
    {
        var messageBox = MessageBoxManager.GetMessageBoxStandard(
            "Authentication Failed",
            $"Incorrect username or password. Could not authenticate your session",
            MsBox.Avalonia.Enums.ButtonEnum.Ok,
            MsBox.Avalonia.Enums.Icon.Forbidden
        );
        await messageBox.ShowWindowDialogAsync(owner); // Show the popup
    }

    public async void ShowPingFailMessage(Window owner, string host, int port)
    {
        var messageBox = MessageBoxManager.GetMessageBoxStandard(
            "Connection Failed",
            $"Could not connect to host {host}:{port}. Host is not active",
            MsBox.Avalonia.Enums.ButtonEnum.Ok,
            MsBox.Avalonia.Enums.Icon.Info
        );
        await messageBox.ShowWindowDialogAsync(owner); // Show the popup
    }
    
    public async Task<ButtonResult> ShowSaveDevMessage(Window owner, string host)
    {
        var messageBox = MessageBoxManager.GetMessageBoxStandard(
            "Save Information",
            $"Would you like to save FTP Server: {host}?",
            MsBox.Avalonia.Enums.ButtonEnum.YesNo,
            MsBox.Avalonia.Enums.Icon.Database
        );
        SaveButton = await messageBox.ShowWindowDialogAsync(owner); // Show the popup
        return SaveButton;
    }
    
    public async Task<ButtonResult> ShowExistsMessage(Window owner, string item)
    {
        var messageBox = MessageBoxManager.GetMessageBoxStandard(
            "Already exists",
            $"{item} already exists. Would you like to replace?",
            MsBox.Avalonia.Enums.ButtonEnum.YesNo,
            MsBox.Avalonia.Enums.Icon.Warning
        );
        ExistButton = await messageBox.ShowWindowDialogAsync(owner); // Show the popup
        return ExistButton;
    }

    public async Task ShowServerRunningMessage(Window owner)
    {
        var messageBox = MessageBoxManager.GetMessageBoxStandard(
            "FTP Server Running",
            $"This computer is already hosting an FTP server.",
            MsBox.Avalonia.Enums.ButtonEnum.OkAbort,
            MsBox.Avalonia.Enums.Icon.Error
        );
        ServRunButtonResult = await messageBox.ShowWindowDialogAsync(owner);
    }

    public async Task<string> ServerRunningFunction(Window owner, FTPServerWindow FTPWindow)
    {
        await ShowServerRunningMessage(owner);
        if (ServRunButtonResult != null)
        {
            if (ServRunButtonResult.ToString() == "Abort")
            {
                FTPWindow.StopFtpServer();
                ButtonResult = "Aborted";

            }

            return ButtonResult;
        }
        return ButtonResult;
    }
}