using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.IO;
using System.Linq;
using Avalonia.ReactiveUI;
using MsBox.Avalonia;
using CSIA.ViewModels;

namespace CSIA.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        private string? _currentDirectory;

        public MainWindow()
        {
            InitializeComponent();
            LoadDrives();
            
            // Button click handlers
            OpenButton.Click += OpenButton_Click;
            BackButton.Click += BackButton_Click;

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
                _currentDirectory = path;

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
                ShowAccessDeniedMessage(path);
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions as needed
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async void ShowAccessDeniedMessage(string path)
        {
            var messageBox = MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard(
                "Access Denied",
                $"You do not have permission to access the directory: {path}",
                MsBox.Avalonia.Enums.ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Warning
            );

            await messageBox.ShowWindowDialogAsync(this); // Show the popup
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
            if (!string.IsNullOrEmpty(_currentDirectory))
            {
                var parentDirectory = Directory.GetParent(_currentDirectory)?.FullName;

                // Check if we're at the root of a drive (e.g., "C:\")
                if (parentDirectory == null)
                {
                    // If there's no parent directory, we're at the root of a drive.
                    // So, we should go back to the list of drives.
                    LoadDrives();
                    _currentDirectory = null; // Reset current directory to indicate we're at the drive level
                }
                else
                {
                    // Otherwise, load the parent directory
                    LoadDirectory(parentDirectory);
                }
            }
            else
            {
                // If _currentDirectory is null, we're already at the drive list level, no further action needed
                Console.WriteLine("Already at the drive list.");
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
    }
}
