using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;

namespace CSIA.Backend
{
    public class FileIconManager
    {
        private readonly Dictionary<string, Bitmap> _iconCache = new Dictionary<string, Bitmap>();
        private readonly string _iconDirectory;

        public FileIconManager(string iconDirectory)
        {
            _iconDirectory = iconDirectory;
            LoadIcons();
        }

        private void LoadIcons()
        {
            // Load icons from resources or files
            // You'll need to add these icon files to your project resources
            try
            {
                _iconCache["image"] = new Bitmap($"{_iconDirectory}/image-icon.png");
                _iconCache["document"] = new Bitmap($"{_iconDirectory}/document-icon.png");
                _iconCache["pdf"] = new Bitmap($"{_iconDirectory}/pdf-icon.png");
                _iconCache["video"] = new Bitmap($"{_iconDirectory}/video-icon.png");
                _iconCache["audio"] = new Bitmap($"{_iconDirectory}/audio-icon.png");
                _iconCache["archive"] = new Bitmap($"{_iconDirectory}/archive-icon.png");
                _iconCache["default"] = new Bitmap($"{_iconDirectory}/default-icon.png");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading icons: {ex.Message}");
            }
        }

        public Bitmap GetIconForFile(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            
            return extension switch
            {
                ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" => _iconCache["image"],
                ".doc" or ".docx" or ".txt" or ".rtf" => _iconCache["document"],
                ".pdf" => _iconCache["pdf"],
                ".mp4" or ".avi" or ".mov" or ".wmv" => _iconCache["video"],
                ".mp3" or ".wav" or ".ogg" or ".m4a" => _iconCache["audio"],
                ".zip" or ".rar" or ".7z" => _iconCache["archive"],
                _ => _iconCache["default"]
            };
        }
    }
}