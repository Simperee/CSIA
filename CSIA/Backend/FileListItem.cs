using Avalonia.Media.Imaging;
using System.IO;

namespace CSIA.Backend;

public class FileListItem
{
    public string FilePath { get; set; }
    public string FileName { get; set; }
    public Bitmap Icon { get; set; }

    public FileListItem(string filePath, Bitmap icon)
    {
        FilePath = filePath;
        FileName = Path.GetFileName(filePath);
        Icon = icon;
    }
}