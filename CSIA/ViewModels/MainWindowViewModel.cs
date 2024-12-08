using System;
using System.Collections.ObjectModel;
using System.IO;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Input;
using CSIA.Backend;
using CSIA.Views;
using DynamicData;
using ReactiveUI;
using WinSCP;

public class MainWindowViewModel : ReactiveObject
{
    private PopUpDialog popUpDialog = new PopUpDialog();
    private readonly Window _owner;

    public class RemoteFileSystemItem
    {
        public string RemoteName { get; set; }
        public string FullPath { get; set; }
        public bool IsDirectory { get; set; }
        public Bitmap RemoteIcon { get; set; }

        public RemoteFileSystemItem(string fullPath, bool isDirectory, bool parentFolder)
        {
            FullPath = fullPath;
            IsDirectory = isDirectory;

            if (parentFolder)
            {
                if (FullPath.Contains("Drives"))
                {
                    RemoteName = "Go to Drives";
                }
                else
                {
                    RemoteName = "..";
                }
            }
            else
            {
                RemoteName = Path.GetFileName(fullPath) == string.Empty ? fullPath : Path.GetFileName(fullPath);
            }

            // Determine the appropriate icon for the file or folder
            if (isDirectory)
            {
                RemoteIcon = new Bitmap(AssetLoader.Open(new Uri("avares://CSIA/Assets/Icons/folder_icon.png")));
            }
            else
            {
                var extension = Path.GetExtension(fullPath).ToLower();
                Uri iconUri = extension switch
                {
                    ".txt" => new Uri("avares://CSIA/Assets/Icons/txt_icon.png"),
                    ".pdf" => new Uri("avares://CSIA/Assets/Icons/pdf_icon.png"),
                    ".doc" or ".docx" => new Uri("avares://CSIA/Assets/Icons/doc_icon.png"),
                    ".jpg" or ".jpeg" => new Uri("avares://CSIA/Assets/Icons/jpg_icon.png"),
                    ".png" => new Uri("avares://CSIA/Assets/Icons/png_icon.png"),
                    ".mp3" => new Uri("avares://CSIA/Assets/Icons/mp3_icon.png"),
                    ".mp4" => new Uri("avares://CSIA/Assets/Icons/mp4_icon.png"),
                    _ => new Uri("avares://CSIA/Assets/Icons/file_icon.png")
                };

                RemoteIcon = new Bitmap(AssetLoader.Open(iconUri));
            }
        }
    }

    public class LocalFileSystemItem
    {
        public string LocalName { get; set; }
        public string FullPath { get; set; }
        public bool IsDirectory { get; set; }
        public Bitmap LocalIcon { get; set; }

        public LocalFileSystemItem(string fullPath, bool isDirectory, bool parentFolder)
        {
            FullPath = fullPath;
            IsDirectory = isDirectory;

            if (parentFolder)
            {
                if (FullPath.Contains("Drives"))
                {
                    LocalName = "Go to Drives";
                }
                else
                {
                    LocalName = "..";   
                }
            }
            else
            {
                LocalName = Path.GetFileName(fullPath) == string.Empty ? fullPath : Path.GetFileName(fullPath);
            }

            // Determine the appropriate icon for the file or folder
            if (isDirectory)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(fullPath, @"^[A-Z]:\\$"))
                {
                    LocalIcon = new Bitmap(AssetLoader.Open(new Uri("avares://CSIA/Assets/Icons/drive_icon.png")));
                }
                else
                {
                    LocalIcon = new Bitmap(AssetLoader.Open(new Uri("avares://CSIA/Assets/Icons/folder_icon.png")));
                }
            }
            else
            {
                var extension = Path.GetExtension(fullPath).ToLower();
                Uri iconUri = extension switch
                {
                    ".txt" => new Uri("avares://CSIA/Assets/Icons/txt_icon.png"),
                    ".pdf" => new Uri("avares://CSIA/Assets/Icons/pdf_icon.png"),
                    ".doc" or ".docx" => new Uri("avares://CSIA/Assets/Icons/doc_icon.png"),
                    ".jpg" or ".jpeg" => new Uri("avares://CSIA/Assets/Icons/jpg_icon.png"),
                    ".png" => new Uri("avares://CSIA/Assets/Icons/png_icon.png"),
                    ".mp3" => new Uri("avares://CSIA/Assets/Icons/mp3_icon.png"),
                    ".mp4" => new Uri("avares://CSIA/Assets/Icons/mp4_icon.png"),
                    _ => new Uri("avares://CSIA/Assets/Icons/file_icon.png")
                };

                LocalIcon = new Bitmap(AssetLoader.Open(iconUri));
            }
        }
    }

    private string _currentPath;
    
    public string CurrentPath
    {
        get => _currentPath;
        set => this.RaiseAndSetIfChanged(ref _currentPath, value);
    }
    
    private ObservableCollection<LocalFileSystemItem> _localitems;

    public ObservableCollection<LocalFileSystemItem> LocalItems
    {
        get => _localitems;
        set => this.RaiseAndSetIfChanged(ref _localitems, value);
    }
    
    private ObservableCollection<RemoteFileSystemItem> _remoteitems;

    public ObservableCollection<RemoteFileSystemItem> RemoteItems
    {
        get => _remoteitems;
        set => this.RaiseAndSetIfChanged(ref _remoteitems, value);
    }

    public MainWindowViewModel(Window owner)
    {
        // Start with the list of drives
        _owner = owner;
        LoadDrives();
        Console.WriteLine($"Connected: {FTPClass.Instance.IsOpen()}");
        if (FTPClass.Instance.IsOpen() && FTPClass.PingHost(FTPClass.Instance._ftpSessionOptions.HostName, FTPClass.Instance._ftpSessionOptions.PortNumber))
        {
            LoadRemoteItems(FTPClass.Instance.FTPSession.HomePath);
        }
        else
        {
            try
            {
                RemoteItems.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    // Method to load the list of drives
    public void LoadDrives()
    {
        var driveItems = new ObservableCollection<LocalFileSystemItem>();

        try
        {
            // Get all drives
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady) // Check if the drive is ready to avoid exceptions
                {
                    driveItems.Add(new LocalFileSystemItem(drive.RootDirectory.FullName, true, false));
                }
            }
        }
        catch (Exception ex)
        {
            popUpDialog.ShowErrorMessage(_owner,ex.Message);
        }

        LocalItems = driveItems;
        CurrentPath = null; // No specific path since we're at the root of drives
    }

    // Method to load items in a given directory
    public void LoadLocalItems(string path)
    {
        var newItems = new ObservableCollection<LocalFileSystemItem>();
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
                    newItems.Add(new LocalFileSystemItem(parentDirectory.FullName, true, true));
                }
                else
                {
                    // Add a "Go to Drives" option to return to the root drives list
                    newItems.Add(new LocalFileSystemItem("Drives", true, true));
                }
            }

            try
            {
                // Add directories
                foreach (var directory in Directory.GetDirectories(path))
                {
                    newItems.Add(new LocalFileSystemItem(directory, true, false));
                }

                // Add files
                foreach (var file in Directory.GetFiles(path))
                {
                    newItems.Add(new LocalFileSystemItem(file, false, false));
                    FileInfo test = new FileInfo(file);
                    Console.WriteLine(test.Name);
                    Console.WriteLine(test.Length/1024);
                }
            }
            catch (Exception ex)
            {
                popUpDialog.ShowErrorMessage(_owner, ex.Message);
            }

            LocalItems = newItems;
            CurrentPath = path;
        }
    }
    
    public void LoadRemoteItems(string path)
    {
        var newItems = new ObservableCollection<RemoteFileSystemItem>();
        // Check if the current path has a parent directory
        if (!string.IsNullOrEmpty(path))
        {
            var parentDirectory = Directory.GetParent(path);
            if (parentDirectory != null)
            {
                if (parentDirectory.FullName.StartsWith("/"))
                {
                    newItems.Add(new RemoteFileSystemItem(parentDirectory.FullName, true, true));
                }
            }
        }

        try
        {
            RemoteDirectoryInfo directory = FTPClass.Instance.FTPSession.ListDirectory(path);
            foreach (RemoteFileInfo fileInfo in directory.Files)
            {
                if (fileInfo.FullName.StartsWith("/"))
                {
                    newItems.Add(new RemoteFileSystemItem(fileInfo.FullName,fileInfo.IsDirectory,fileInfo.IsParentDirectory));
                }
            }
        }
        catch (Exception ex)
        {
            popUpDialog.ShowErrorMessage(_owner, ex.Message);
        }

        RemoteItems = newItems;
        CurrentPath = path;
    }

    // Override OpenItem to handle "Drives" navigation
    public void LocalOpenItem(LocalFileSystemItem item)
    {
        if (item.LocalName.Contains("Go to Drives"))
        {
            LoadDrives();
        }
        else if (item.IsDirectory)
        {
            LoadLocalItems(item.FullPath);
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

    public void RemoteOpenItem(RemoteFileSystemItem item)
    {
        Console.WriteLine(item.FullPath);
        if (item.IsDirectory)
        {
            LoadRemoteItems(item.FullPath);
        }
    }
}