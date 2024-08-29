using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.IO;
using System.Linq;
using Avalonia.ReactiveUI;
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
            DataContext = new MainWindowViewModel();
            
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
            catch (ArgumentOutOfRangeException ex)
            {
                // Handle or log the exception as needed
                Console.WriteLine($"Error: {ex.Message}");
            }
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

                if (!string.IsNullOrEmpty(parentDirectory) && Directory.Exists(parentDirectory))
                {
                    LoadDirectory(parentDirectory);
                }
                else
                {
                    // If no parent directory, reload the drives (i.e., go back to the root view)
                    LoadDrives();
                    _currentDirectory = null;
                }
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
