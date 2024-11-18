using System;
using System.Collections.ObjectModel;
using System.IO;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CSIA.Backend;
using CSIA.Views;
using ReactiveUI;

public class MainWindowViewModel : ReactiveObject
{
    private PopUpDialog popUpDialog = new PopUpDialog();
    
    // FileSystemItem class defined inside the MainWindowViewModel
    public class FileSystemItem
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public bool IsDirectory { get; set; }
        public Bitmap Icon { get; set; }

        public FileSystemItem(string fullPath, bool isDirectory, bool parentFolder)
        {
            FullPath = fullPath;
            IsDirectory = isDirectory;

            if (parentFolder)
            {
                if (FullPath.Contains("Drives"))
                {
                    Name = "Go to Drives";
                }
                else
                {
                    Name = "..";   
                }
            }
            else
            {
                Name = Path.GetFileName(fullPath) == string.Empty ? fullPath : Path.GetFileName(fullPath);
            }

            // Determine the appropriate icon for the file or folder
            if (isDirectory)
            {
                Console.WriteLine(FullPath);
                if (fullPath.Contains(":/"))
                {
                    Icon = new Bitmap(AssetLoader.Open(new Uri("avares://CSIA/Assets/Icons/drive_icon.png")));
                }
                else
                {
                    Icon = new Bitmap(AssetLoader.Open(new Uri("avares://CSIA/Assets/Icons/folder_icon.png")));
                }
            }
            else
            {
                var extension = Path.GetExtension(fullPath).ToLower();
                Uri iconUri;

                switch (extension)
                {
                    case ".txt":
                        iconUri = new Uri("avares://CSIA/Assets/Icons/txt_icon.png");
                        break;
                    case ".pdf":
                        iconUri = new Uri("avares://CSIA/Assets/Icons/pdf_icon.png");
                        break;
                    case ".doc":
                    case ".docx":
                        iconUri = new Uri("avares://CSIA/Assets/Icons/doc_icon.png");
                        break;
                    case ".jpg":
                    case ".jpeg":
                        iconUri = new Uri("avares://CSIA/Assets/Icons/jpg_icon.png");
                        break;
                    case ".png":
                        iconUri = new Uri("avares://CSIA/Assets/Icons/png_icon.png");
                        break;
                    case ".mp3":
                        iconUri = new Uri("avares://CSIA/Assets/Icons/mp3_icon.png");
                        break;
                    case ".mp4":
                        iconUri = new Uri("avares://CSIA/Assets/Icons/mp4_icon.png");
                        break;
                    default:
                        iconUri = new Uri("avares://CSIA/Assets/Icons/file_icon.png");
                        break;
                }

                Icon = new Bitmap(AssetLoader.Open(iconUri));
            }
        }
    }

    private string _currentPath;
    
    public string CurrentPath
    {
        get => _currentPath;
        set => this.RaiseAndSetIfChanged(ref _currentPath, value);
    }
    
    private ObservableCollection<FileSystemItem> _items;

    public ObservableCollection<FileSystemItem> Items
    {
        get => _items;
        set => this.RaiseAndSetIfChanged(ref _items, value);
    }

    public MainWindowViewModel()
    {
        // Start with the list of drives
        LoadDrives();
    }

    // Method to load the list of drives
    public void LoadDrives()
    {
        var driveItems = new ObservableCollection<FileSystemItem>();

        try
        {
            // Get all drives
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady) // Check if the drive is ready to avoid exceptions
                {
                    driveItems.Add(new FileSystemItem(drive.RootDirectory.FullName, true, false));
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., access permissions)
            // Console.WriteLine($"Error loading drives: {ex.Message}");
            // popUpDialog.ShowErrorMessage(,ex.Message);
        }

        Items = driveItems;
        CurrentPath = null; // No specific path since we're at the root of drives
    }

    // Method to load items in a given directory
    public void LoadItems(string path)
    {
        var newItems = new ObservableCollection<FileSystemItem>();

        // Check if the current path has a parent directory
        if (!string.IsNullOrEmpty(path))
        {
            var parentDirectory = Directory.GetParent(path);
            if (parentDirectory != null)
            {
                // Add a special "Go to Parent Folder" item at the top of the list
                newItems.Add(new FileSystemItem(parentDirectory.FullName, true, true));
            }
            else
            {
                // Add a "Go to Drives" option to return to the root drives list
                newItems.Add(new FileSystemItem("Drives", true, true));
            }
        }

        try
        {
            // Add directories
            foreach (var directory in Directory.GetDirectories(path))
            {
                newItems.Add(new FileSystemItem(directory, true, false));
            }

            // Add files
            foreach (var file in Directory.GetFiles(path))
            {
                newItems.Add(new FileSystemItem(file, false, false));
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., access permissions)
            Console.WriteLine($"Error loading items: {ex.Message}");
        }

        Items = newItems;
        CurrentPath = path;
    }

    // Override OpenItem to handle "Drives" navigation
    public void OpenItem(FileSystemItem item)
    {
        if (item.Name.Contains("Go to Drives"))
        {
            LoadDrives();
        }
        else if (item.IsDirectory)
        {
            LoadItems(item.FullPath);
        }
        else
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = item.FullPath,
                UseShellExecute = true
            });
        }
    }
}
