using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using Avalonia.Media.Imaging;
using CSIA.Backend;
using CSIA.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private string _currentPath;

    public string CurrentPath
    {
        get => _currentPath;
        set
        {
            if (_currentPath != value)
            {
                _currentPath = value;
                OnPropertyChanged(nameof(CurrentPath));
                LoadFiles(value); // Load files when CurrentPath changes
            }
        }
    }

    public ObservableCollection<FileListItem> FileListItems { get; } = new ObservableCollection<FileListItem>();

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void LoadFiles(string path)
    {
        FileListItems.Clear();
        
        // Ensure the path is valid and accessible
        if (Directory.Exists(path))
        {
            // Load directories and files
            foreach (var dirPath in Directory.GetDirectories(path))
            {
                FileListItems.Add(new FileListItem(new DirectoryInfo(dirPath).Name, GetFolderIcon()));
            }

            foreach (var filePath in Directory.GetFiles(path))
            {
                FileListItems.Add(new FileListItem(Path.GetFileName(filePath), GetFileIcon(filePath)));
            }
        }
    }

    private Bitmap GetFolderIcon()
    {
        // Replace this with your logic to get a folder icon
        return new Bitmap("path/to/default-folder-icon.png");
    }

    private Bitmap GetFileIcon(string filePath)
    {
        // Replace this with your logic to get a file icon based on file type
        return new Bitmap("path/to/default-file-icon.png");
    }
}