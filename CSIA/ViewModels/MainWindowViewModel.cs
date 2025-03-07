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
                RemoteName = Path.GetFileName(fullPath) == string.Empty ? fullPath : Path.GetFileName(fullPath); //cool regex thats really sick
            }

            //determine appropriate icon for file or folder
            if (isDirectory)
            {
                RemoteIcon = new Bitmap(AssetLoader.Open(new Uri("avares://CSIA/Assets/Icons/folder_icon.png"))); //folder icon
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
                }; //define image to use for item type

                RemoteIcon = new Bitmap(AssetLoader.Open(iconUri)); //apply image defined above
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

            //determine appropriate icon for the file or folder
            if (isDirectory)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(fullPath, @"^[A-Z]:\\$")) //cool regex thats really sick
                {
                    LocalIcon = new Bitmap(AssetLoader.Open(new Uri("avares://CSIA/Assets/Icons/drive_icon.png"))); //disk drive icon
                }
                else
                {
                    LocalIcon = new Bitmap(AssetLoader.Open(new Uri("avares://CSIA/Assets/Icons/folder_icon.png"))); //folder icon
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
                }; //define image to use for item type

                LocalIcon = new Bitmap(AssetLoader.Open(iconUri)); //apply image defined above
            }
        }
    }

    private string _currentLocalPath;
    
    public string CurrentLocalPath
    {
        get => _currentLocalPath;
        set => this.RaiseAndSetIfChanged(ref _currentLocalPath, value);
    }
    
    private ObservableCollection<LocalFileSystemItem> _localitems;

    public ObservableCollection<LocalFileSystemItem> LocalItems
    {
        get => _localitems;
        set => this.RaiseAndSetIfChanged(ref _localitems, value);
    }
    
    private string _currentRemotePath;
    
    public string CurrentRemotePath
    {
        get => _currentRemotePath;
        set => this.RaiseAndSetIfChanged(ref _currentRemotePath, value);
    }
    
    private ObservableCollection<RemoteFileSystemItem> _remoteitems;

    public ObservableCollection<RemoteFileSystemItem> RemoteItems
    {
        get => _remoteitems;
        set => this.RaiseAndSetIfChanged(ref _remoteitems, value);
    }

    public MainWindowViewModel(Window owner, string localReload ,string remoteReload)
    {
        // Start with the list of drives
        _owner = owner;
        if (localReload != null)
            LoadLocalItems(localReload);
        else
            LoadDrives();
        Console.WriteLine($"Connected: {FTPClass.Instance.IsOpen()}");
        if (FTPClass.Instance.IsOpen() && FTPClass.PingHost(FTPClass.Instance._ftpSessionOptions.HostName, FTPClass.Instance._ftpSessionOptions.PortNumber))
        {
            if (remoteReload != null)
                LoadRemoteItems(remoteReload);
            else
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

    //load list of drives
    public void LoadDrives()
    {
        var driveItems = new ObservableCollection<LocalFileSystemItem>();

        try
        {
            //get all drives
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady) //check if drive is ready so no exceptions
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
        CurrentLocalPath = null; //no specific path since root of drives
    }

    // Method to load items in a given directory
    public void LoadLocalItems(string path)
    {
        var newItems = new ObservableCollection<LocalFileSystemItem>();
        bool checkRights = false;
        //check if can access directory
        try
        {
            Directory.GetDirectories(path);
            checkRights = true;
        }
        catch (Exception ex)
        {
            popUpDialog.ShowErrorMessage(_owner, ex.Message);
        }

        if (checkRights)
        {
            //check if current path has parent directory
            if (!string.IsNullOrEmpty(path))
            {
                var parentDirectory = Directory.GetParent(path);
                if (parentDirectory != null)
                {
                    //add a special "Go to Parent Folder" item at top of list
                    newItems.Add(new LocalFileSystemItem(parentDirectory.FullName, true, true));
                }
                else
                {
                    //add a "Go to Drives" option to return to root drives list
                    newItems.Add(new LocalFileSystemItem("Drives", true, true));
                }
            }

            try
            {
                //add directories
                foreach (var directory in Directory.GetDirectories(path))
                {
                    newItems.Add(new LocalFileSystemItem(directory, true, false));
                }

                //add files
                foreach (var file in Directory.GetFiles(path))
                {
                    newItems.Add(new LocalFileSystemItem(file, false, false));
                    FileInfo test = new FileInfo(file);
                    // Console.WriteLine(test.Name);
                    // Console.WriteLine(test.Length/1024);
                }
            }
            catch (Exception ex)
            {
                popUpDialog.ShowErrorMessage(_owner, ex.Message);
            }

            LocalItems = newItems;
            CurrentLocalPath = path;
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
            RemoteDirectoryInfo directory = FTPClass.Instance.DirectoryItems(path);
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
        CurrentRemotePath = FTPClass.Instance.RemotePath;
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