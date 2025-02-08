using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.ReactiveUI;
using CSIA.Backend;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CSIA.ViewModels;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.Models;

namespace CSIA.Views
{
    public partial class MainWindow : Window
    {

        private PopUpDialog popUpDialog = new PopUpDialog();
        public FTPServerWindow FTPWindow = new FTPServerWindow();
        public FTPConnectWindow ConnectWindow;

        public MainWindow()
        {
            string LocalAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CSIA");
            Directory.CreateDirectory(LocalAppData);
            Directory.CreateDirectory(Path.Combine(LocalAppData, "tmp"));
            Directory.CreateDirectory(Path.Combine(LocalAppData, "config"));
            InitializeComponent();
            ConnectWindow = new FTPConnectWindow(this);
            DataContext = new MainWindowViewModel(this, null,null);
            // LoadDrives();
            
            // Button click handlers
            // BackButton.Click += BackButton_Click;
            // ForwardButton.Click += ForwardButton_Click;
            RefreshButton.Click += RefreshButton_Click;
            HostButton.Click += HostButton_Click;
            StopButton.Click += StopButton_Click;
            ConnectButton.Click += ConnectButton_Click;
            CloseButton.Click += CloseButton_Click;
            MinimButton.Click += MinimButton_Click;
            MaximButton.Click += MaximButton_Click;
            TestButton.Click += TestButton_Click;
            UploadButton.Click += UploadButton_Click;
            LocalFolderAddButton.Click += LocalFolderAddButton_Click;
            LocalDeleteButton.Click += LocalDeleteButton_Click;
            DownloadButton.Click += DownloadButton_Click;
            RemoteFolderAddButton.Click += RemoteFolderAddButton_Click;
            RemoteDeleteButton.Click += RemoteDeleteButton_Click;
            
            // ForwardButton.IsEnabled = false;
            // BackButton.IsEnabled = false;

            // TreeView and ListBox selection handlers
            // DirectoryTreeView.SelectionChanged += DirectoryTreeView_SelectionChanged;
            LocalListBox.DoubleTapped += LocalListBox_DoubleTapped;
            RemoteListBox.DoubleTapped += RemoteListBox_DoubleTapped;
        }

        // private void LoadDrives()
        // {
        //     DirectoryTreeView.ItemsSource = Directory.GetLogicalDrives();
        // }
        //
        // private void DirectoryTreeView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        // {
        //     if (DirectoryTreeView.SelectedItem is string path && Directory.Exists(path))
        //     {
        //         LoadDirectory(path);
        //     }
        // }
        //
        // private void LoadDirectory(string path)
        // {
        //     try
        //     {
        //         currentDirectory = path;
        //         breadcrumbPath.Text = currentDirectory;
        //
        //         // Clear current selection
        //         DirectoryTreeView.SelectedItem = null;
        //         RemoteListBox.SelectedItem = null;
        //
        //         // Get directories and files
        //         var directories = Directory.GetDirectories(path);
        //         var files = Directory.GetFiles(path);
        //
        //         DirectoryTreeView.ItemsSource = directories.Any() ? directories : null;
        //         
        //         // Create FileItem list
        //         RemoteListBox.ItemsSource = files.Any() ? files : null;
        //     }
        //     catch (UnauthorizedAccessException)
        //     {
        //         // Show a popup message if access is denied
        //         popUpDialog.ShowAccessDeniedMessage(this,path);
        //     }
        //     catch (Exception ex)
        //     {
        //         // Log or handle other exceptions as needed
        //         Console.WriteLine($"Error: {ex.Message}");
        //     }
        // }

        
        // public static string GetLocalIPv6Address()
        // {
        //     var host = Dns.GetHostEntry(Dns.GetHostName());
        //     foreach (var ip in host.AddressList)
        //     {
        //         if (ip.AddressFamily == AddressFamily.InterNetworkV6)
        //         {
        //             return ip.ToString();
        //         }
        //     }
        //     throw new Exception("No network adapters with an IPv6 address in the system!");
        // }

        // private void OpenButton_Click(object? sender, RoutedEventArgs e)
        // {
        //     if (FileListBox.SelectedItem is string filePath && File.Exists(filePath))
        //     {
        //         // Open file logic, e.g., open with default application
        //         System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        //         {
        //             FileName = filePath,
        //             UseShellExecute = true
        //         });
        //         OpenButton.IsEnabled = false;
        //         FileListBox.SelectedItem = null;
        //     }
        // }

        // private void BackButton_Click(object? sender, RoutedEventArgs e)
        // {
        //     if (!string.IsNullOrEmpty(currentDirectory))
        //     {
        //         var parentDirectory = Directory.GetParent(currentDirectory)?.FullName;
        //
        //         // Check if we're at the root of a drive (e.g., "C:\")
        //         if (parentDirectory == null)
        //         {
        //             // If there's no parent directory, we're at the root of a drive.
        //             // So, we should go back to the list of drives.
        //             LoadDrives();
        //             breadcrumbPath.Text = null;
        //             RemoteListBox.ItemsSource = null;
        //             currentDirectory = null; // Reset current directory to indicate we're at the drive level
        //             BackButton.IsEnabled = false;
        //         }
        //         else
        //         {
        //             // Otherwise, load the parent directory
        //             forwardDirectory = currentDirectory;
        //             ForwardButton.IsEnabled = true;
        //             LoadDirectory(parentDirectory);
        //         }
        //     }
        //     else
        //     {
        //         // If currentDirectory is null, we're already at the drive list level, no further action needed
        //         Console.WriteLine("Already at the drive list.");
        //     }
        // }
        //
        // private void ForwardButton_Click(object? sender, RoutedEventArgs e)
        // {
        //     if (!string.IsNullOrEmpty(forwardDirectory))
        //     { 
        //
        //         // Check if we're at the root of a drive (e.g., "C:\")
        //         if (Directory.Exists(forwardDirectory))
        //         {
        //             LoadDirectory(forwardDirectory);
        //             forwardDirectory = null;
        //             ForwardButton.IsEnabled = false;
        //         }
        //         else
        //         {
        //             
        //         }
        //     }
        //     else
        //     {
        //         // If currentDirectory is null, we're already at the drive list level, no further action needed
        //         Console.WriteLine("Folder does not exist");
        //     }
        // }
        
        private void HostButton_Click(object? sender, RoutedEventArgs e)
        {
            FTPWindow.Show();
            
            // // Stop any existing server before starting a new one
            // cancelSource?.Cancel();
            //
            // var endPoint = new IPEndPoint(IPAddress.Any, 21);
            // var baseDirectory = rootDirectory;
            //
            // // IPv4 implementation of FTP Server
            // try
            // {
            //     var server = new FtpServer(endPoint, baseDirectory);
            //     cancelSource = new CancellationTokenSource();
            //     var runResult = server.RunAsync(cancelSource.Token);
            //
            //     popUpDialog.ShowHostingMessage(GetLocalIPAddress(), endPoint.Port.ToString());
            // }
            // catch (SocketException ex)
            // {
            //     popUpDialog.ShowErrorMessage(this, ex.Message);
            //     Console.WriteLine($"Socket exception: {ex.Message}");
            //     // Display error message or handle it as needed
            // }
        }

        private async void RefreshButton_Click(object? sender, RoutedEventArgs e)
        {
            DataContext = new MainWindowViewModel(this, CurrentLocalPath.Text,FTPClass.Instance.RemotePath);
        }
        
        private async void StopButton_Click(object? sender, RoutedEventArgs e)
        {
            try{
                Console.WriteLine($"Server is running: {FTPWindow.ftpRunning}");
                await FTPWindow.StopFtpServer();
                Console.WriteLine($"Server is running: {FTPWindow.ftpRunning}");
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions as needed
                popUpDialog.ShowErrorMessage(this, ex.Message);
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            ConnectWindow.Show();
        }

        private void RemoteListBox_Tapped(object? sender, EventArgs e)
        {
            // if (FileListBox.SelectedItem is string filePath && File.Exists(filePath))
            // {
            //     OpenButton.IsEnabled = true;
            // }
        }

        private void RemoteListBox_DoubleTapped(object? sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel && RemoteListBox.SelectedItem is MainWindowViewModel.RemoteFileSystemItem selectedItem)
            {
                viewModel.RemoteOpenItem(selectedItem);
            }
        }
        
        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel && RemoteListBox.SelectedItems is AvaloniaList<object> selectedItems)
            {
                var itemsToDownload = selectedItems.ToList();
                
                foreach (var selectedItem in itemsToDownload)
                {
                    if (selectedItem is MainWindowViewModel.RemoteFileSystemItem remoteItem)
                    {
                        try
                        { 
                            FTPClass.Instance.DownloadFile(remoteItem.FullPath, CurrentLocalPath.Text);
                        }
                        catch (Exception ex)
                        {
                            popUpDialog.ShowErrorMessage(this, ex.Message);
                        }
                    }
                }

                // Update the DataContext only once after all deletions
                DataContext = new MainWindowViewModel(this, CurrentLocalPath.Text, FTPClass.Instance.RemotePath);
            }
            else
            {
                Console.WriteLine("Local Fail");
            }
        }
        
        private async void RemoteFolderAddButton_Click(object sender, RoutedEventArgs e)
        {
            string? userInput = await FolderNameDialog.Show(this);
            string pattern = @"[\/:*?""<>|]";
            // Use Regex.IsMatch to check if the pattern is found in the input string
            
            if (userInput != @"null/\cancel")
            {
                bool containsInvalidChar = Regex.IsMatch(userInput, pattern);
                if (containsInvalidChar)
                {
                    MessageBoxManager
                        .GetMessageBoxStandard("Invalid Folder Name", $"Folder name cannot contain {pattern.Replace("[","").Replace("]","")}")
                        .ShowWindowDialogAsync(this);
                }
                else
                {
                    // Do something with the user input
                    // Console.WriteLine(userInput);
                    // Console.WriteLine(CurrentLocalPath.Text);
                    await FTPClass.Instance.CreateDirectory(userInput);
                    DataContext = new MainWindowViewModel(this, CurrentLocalPath.Text, FTPClass.Instance.RemotePath);
                }
            }
        }
        
        private async void RemoteDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel && RemoteListBox.SelectedItems is AvaloniaList<object> selectedItems)
            {
                var itemsToDelete = selectedItems.ToList();
                string message;
                
                if (itemsToDelete.Count > 1)
                {
                    message = $"Are you sure you want to delete these {itemsToDelete.Count} items?";
                }
                else
                {
                    var firstItem = itemsToDelete.FirstOrDefault() as MainWindowViewModel.RemoteFileSystemItem;
                    if (firstItem == null)
                    {
                        Console.WriteLine("Selected item is not a valid FileSystemItem.");
                        return;
                    }
                    message = $"Are you sure you want to delete {firstItem.RemoteName}?";
                }

                ButtonResult deleteItem = await popUpDialog.ShowDeleteQuestionMessage(this, message); //Get user answer if yes or no delete items

                if (deleteItem == ButtonResult.Yes)
                {
                    foreach (var selectedItem in itemsToDelete)
                    {
                        if (selectedItem is MainWindowViewModel.RemoteFileSystemItem remoteItem)
                        {
                            try
                            {
                                FTPClass.Instance.DeleteItem(remoteItem.FullPath);
                            }
                            catch (Exception ex)
                            {
                                popUpDialog.ShowErrorMessage(this, ex.Message);
                            }
                        }
                    }

                    // Update the DataContext only once after all deletions
                    DataContext = new MainWindowViewModel(this, CurrentLocalPath.Text, FTPClass.Instance.RemotePath);
                }
            }
            else
            {
                Console.WriteLine("Remote Fail");
            }
        }
        
        private void LocalListBox_DoubleTapped(object? sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel && LocalListBox.SelectedItem is MainWindowViewModel.LocalFileSystemItem selectedItem)
            {
                viewModel.LocalOpenItem(selectedItem);
            }
        }

        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel && LocalListBox.SelectedItems is AvaloniaList<object> selectedItems)
            {
                var itemsToUpload = selectedItems.ToList();
                
                foreach (var selectedItem in itemsToUpload)
                {
                    if (selectedItem is MainWindowViewModel.LocalFileSystemItem localItem)
                    {
                        try
                        { 
                            FTPClass.Instance.UploadFile(Path.GetFullPath(localItem.FullPath), FTPClass.Instance.RemotePath);
                        }
                        catch (Exception ex)
                        {
                            popUpDialog.ShowErrorMessage(this, ex.Message);
                        }
                    }
                }

                // Update the DataContext only once after all deletions
                DataContext = new MainWindowViewModel(this, CurrentLocalPath.Text, FTPClass.Instance.RemotePath);
            }
            else
            {
                Console.WriteLine("Local Fail");
            }
        }
        
        private async void LocalFolderAddButton_Click(object sender, RoutedEventArgs e)
        {
            string? userInput = await FolderNameDialog.Show(this);
            string pattern = @"[\/:*?""<>|]";
            // Use Regex.IsMatch to check if the pattern is found in the input string
            
            if (userInput != @"null/\cancel")
            {
                bool containsInvalidChar = Regex.IsMatch(userInput, pattern);
                if (containsInvalidChar)
                {
                    MessageBoxManager
                        .GetMessageBoxStandard("Invalid Folder Name", $"Folder name cannot contain {pattern.Replace("[","").Replace("]","")}")
                        .ShowWindowDialogAsync(this);
                }
                else
                {
                    // Do something with the user input
                    // Console.WriteLine(userInput);
                    // Console.WriteLine(CurrentLocalPath.Text);
                    if (!CurrentLocalPath.Text.EndsWith(@"\"))
                        Directory.CreateDirectory(CurrentLocalPath.Text+@"\"+userInput);
                    else
                        Directory.CreateDirectory(CurrentLocalPath.Text+userInput);
                    DataContext = new MainWindowViewModel(this, CurrentLocalPath.Text, FTPClass.Instance.RemotePath);
                }
            }
        }

        private async void LocalDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel && LocalListBox.SelectedItems is AvaloniaList<object> selectedItems)
            {
                var itemsToDelete = selectedItems.ToList();
                string message;
                
                if (itemsToDelete.Count > 1)
                {
                    message = $"Are you sure you want to delete these {itemsToDelete.Count} items?";
                }
                else
                {
                    var firstItem = itemsToDelete.FirstOrDefault() as MainWindowViewModel.LocalFileSystemItem;
                    if (firstItem == null)
                    {
                        Console.WriteLine("Selected item is not a valid FileSystemItem.");
                        return;
                    }
                    message = $"Are you sure you want to delete {firstItem.LocalName}?";
                }

                ButtonResult deleteItem = await popUpDialog.ShowDeleteQuestionMessage(this, message); //Get user answer if yes or no delete items

                if (deleteItem == ButtonResult.Yes)
                {
                    foreach (var selectedItem in itemsToDelete)
                    {
                        if (selectedItem is MainWindowViewModel.LocalFileSystemItem localItem)
                        {
                            try
                            {
                                if (localItem.IsDirectory)
                                    Directory.Delete(localItem.FullPath, true);
                                else
                                    File.Delete(localItem.FullPath);
                            }
                            catch (Exception ex)
                            {
                                popUpDialog.ShowErrorMessage(this, ex.Message);
                            }
                        }
                    }

                    // Update the DataContext only once after all deletions
                    DataContext = new MainWindowViewModel(this, CurrentLocalPath.Text, FTPClass.Instance.RemotePath);
                }
            }
            else
            {
                Console.WriteLine("Local Fail");
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState != WindowState.Maximized)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            DataContext = new MainWindowViewModel(this, null, null);
        }
        
        protected override async void OnClosing(WindowClosingEventArgs e)
        {
            // Check if FTP server is running
            if (FTPWindow.ftpRunning)
            {
                e.Cancel = true;
                try
                {
                    // Await the result of the async function
                    var result = await popUpDialog.ServerRunningFunction(this, FTPWindow);
                    if (result=="Closed")
                    {
                        FTPWindow.StopFtpServer();
                        Environment.Exit(0);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during closing: {ex.Message}");
                }
            }
            else
            {
                // If FTP server is not running, just proceed with the window close
                Environment.Exit(0);
            }
        }
    }
}
