using System;
using System.Collections.ObjectModel;
using System.IO;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CSIA.Backend;
using CSIA.Views;
using ReactiveUI;
using SkiaSharp;

public class FTPConnectViewModel : ReactiveObject
{
    private PopUpDialog popUpDialog = new PopUpDialog();
    private readonly Window _owner;
    
    // SavedConItem class defined inside the FTPConnectViewModel
    public class SavedConItem
    {
        public string Name { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public Bitmap Icon { get; set; }

        public SavedConItem(string deviceName, string deviceType)
        {
            DeviceName = deviceName;
            DeviceType = deviceType;
            // Name = Path.GetFileName(fullPath) == string.Empty ? fullPath : Path.GetFileName(fullPath);
            Name = deviceName;

            // Determine if server or desktop icon
            Uri iconUri;
            
            switch (DeviceType)
            {
                case "server":
                    iconUri = new Uri("avares://CSIA/Assets/Icons/server_icon.svg");
                    break;
                case "desktop":
                    iconUri= new Uri("avares://CSIA/Assets/Icons/computer_icon.svg");
                    break;
                default:
                    iconUri = new Uri("avares://CSIA/Assets/Icons/server_icon.svg");
                    break;
            }

            Icon = new Bitmap(AssetLoader.Open(iconUri));
        }
    }
    
    private string _deviceType;

    public string DeviceType
    {
        get => _deviceType;
        set => this.RaiseAndSetIfChanged(ref _deviceType, value);
    }
    
    private string _currentPath;
    
    public string CurrentPath
    {
        get => _currentPath;
        set => this.RaiseAndSetIfChanged(ref _currentPath, value);
    }
    
    private ObservableCollection<SavedConItem> _items;

    public ObservableCollection<SavedConItem> Items
    {
        get => _items;
        set => this.RaiseAndSetIfChanged(ref _items, value);
    }

    public FTPConnectViewModel(Window owner)
    {
        // Start with the list of drives
        _owner = owner;
        LoadSaved();
        LoadDrives();
    }

    public void LoadSaved()
    {
        var savedItems = new ObservableCollection<SavedConItem>();
        try
        {
            
        }
    }
    
    // Method to load the list of drives
    public void LoadDrives()
    {
        var driveItems = new ObservableCollection<SavedConItem>();

        try
        {
            // Get all drives
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady) // Check if the drive is ready to avoid exceptions
                {
                    driveItems.Add(new SavedConItem(drive.RootDirectory.FullName, true, false));
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., access permissions)
            // Console.WriteLine($"Error loading drives: {ex.Message}");
            popUpDialog.ShowErrorMessage(_owner,ex.Message);
        }

        Items = driveItems;
        CurrentPath = null; // No specific path since we're at the root of drives
    }

    // Method to load items in a given directory
    public void LoadItems(string path)
    {
        var newItems = new ObservableCollection<SavedConItem>();
        bool checkRights = false;
        try
        {
            Directory.GetDirectories(path);
            checkRights = true;
        }
        catch (Exception ex)
        {
            popUpDialog.ShowErrorMessage(_owner, ex.Message);
            checkRights = false;
        }

        if (checkRights)
        {
            // Check if the current path has a parent directory
            if (!string.IsNullOrEmpty(path))
            {
                var parentDirectory = Directory.GetParent(path);
                if (parentDirectory != null)
                {
                    // Add a special "Go to Parent Folder" item at the top of the list
                    newItems.Add(new SavedConItem(parentDirectory.FullName, true, true));
                }
                else
                {
                    // Add a "Go to Drives" option to return to the root drives list
                    newItems.Add(new SavedConItem("Drives", true, true));
                }
            }

            try
            {
                // Add directories
                foreach (var directory in Directory.GetDirectories(path))
                {
                    newItems.Add(new SavedConItem(directory, true, false));
                }

                // Add files
                foreach (var file in Directory.GetFiles(path))
                {
                    newItems.Add(new SavedConItem(file, false, false));
                    FileInfo test = new FileInfo(file);
                    Console.WriteLine(test.Name);
                    Console.WriteLine(test.Length/1024);
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error loading items: {ex.Message}");
                popUpDialog.ShowErrorMessage(_owner, ex.Message);
            }

            Items = newItems;
            CurrentPath = path;
        }
    }

    // Override OpenItem to handle "Drives" navigation
    public void OpenItem(SavedConItem item)
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
