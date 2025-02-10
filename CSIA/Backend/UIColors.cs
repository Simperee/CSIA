using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace CSIA.Backend;

public class UIColors
{
    public UIColors()
    {
        String ConfigFilePath = null;
        try
        {
            ConfigFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CSIA", "UI.json");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        if (!File.Exists(ConfigFilePath))
        {
            List<Theme> _theme = new List<Theme>();
            _theme.Add(new Theme()
            {
                BackgroundHex = 50410010,
                SSN = 2,
                Message = "A Message"
            });
            
            using (StreamWriter file = File.CreateText(ConfigFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, _theme);
            }
        }
        else
        {
            var ConfigFile = JsonConvert.DeserializeObject(File.ReadAllText(ConfigFilePath));
        }
        
    }
    public class Theme
    {
        public int BackgroundHex { get; set; }
        public int SSN { get; set; }
        public string Message { get; set;}
    }
}