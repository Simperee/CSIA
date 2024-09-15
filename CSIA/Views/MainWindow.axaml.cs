using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Avalonia.ReactiveUI;
using CSIA.Backend;

namespace CSIA.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        private string? currentDirectory;

        private PopUpDialog popUpDialog = new PopUpDialog();
        private FTPServerWindow FTPWindow = new FTPServerWindow();

        public MainWindow()
        {
            InitializeComponent();
            LoadDrives();
            
            // Button click handlers
            OpenButton.Click += OpenButton_Click;
            BackButton.Click += BackButton_Click;
            HostButton.Click += HostButton_Click;
            StopButton.Click += StopButton_Click;

            // TreeView and ListBox selection handlers
            DirectoryTreeView.SelectionChanged += DirectoryTreeView_SelectionChanged;
            FileListBox.DoubleTapped += FileListBox_DoubleTapped;
        }

        private void LoadDrives()
        {
            DirectoryTreeView.ItemsSource = Directory.GetLogicalDrives();
        }

        private void DirectoryTreeView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (DirectoryTreeView.SelectedItem is string path && Directory.Exists(path))
            {
                LoadDirectory(path);
            }
        }

        private void LoadDirectory(string path)
        {
            try
            {
                currentDirectory = path;

                // Clear current selection
                DirectoryTreeView.SelectedItem = null;
                FileListBox.SelectedItem = null;

                // Get directories and files
                var directories = Directory.GetDirectories(path);
                var files = Directory.GetFiles(path);

                // Assign to ItemsSource only if there are items
                DirectoryTreeView.ItemsSource = directories.Any() ? directories : null;
                FileListBox.ItemsSource = files.Any() ? files : null;
            }
            catch (UnauthorizedAccessException)
            {
                // Show a popup message if access is denied
                popUpDialog.ShowAccessDeniedMessage(this, path);
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions as needed
                popUpDialog.ShowErrorMessage(this, ex.Message);
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        
        public static string GetLocalIPv6Address()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv6 address in the system!");
        }

        private void OpenButton_Click(object? sender, RoutedEventArgs e)
        {
            if (FileListBox.SelectedItem is string filePath && File.Exists(filePath))
            {
                // Open file logic, e.g., open with default application
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
        }

        private void BackButton_Click(object? sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentDirectory))
            {
                var parentDirectory = Directory.GetParent(currentDirectory)?.FullName;

                // Check if we're at the root of a drive (e.g., "C:\")
                if (parentDirectory == null)
                {
                    // If there's no parent directory, we're at the root of a drive.
                    // So, we should go back to the list of drives.
                    LoadDrives();
                    currentDirectory = null; // Reset current directory to indicate we're at the drive level
                }
                else
                {
                    // Otherwise, load the parent directory
                    LoadDirectory(parentDirectory);
                }
            }
            else
            {
                // If currentDirectory is null, we're already at the drive list level, no further action needed
                Console.WriteLine("Already at the drive list.");
            }
        }
        
        // private CancellationTokenSource? cancelSource;
        
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

        private void FileListBox_DoubleTapped(object? sender, RoutedEventArgs e)
        {
            if (FileListBox.SelectedItem is string filePath && File.Exists(filePath))
            {
                // Open file on double click
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
        }
        
        protected override async void OnClosing(WindowClosingEventArgs e)
        {
            if (FTPWindow.ftpRunning)
            {
                
                
                
                e.Cancel = true;
        
                Task.Run(() => MyShowDialog());
            }
        
            base.OnClosing(e);
        }
    }
}
