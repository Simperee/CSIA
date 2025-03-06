using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CSIA.Backend;

public class UIColors
{
    public Theme ClientTheme = new Theme();
    public UIColors()
    {
        String ConfigFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CSIA");
        String ConfigFilePath = null;
        Console.WriteLine(Directory.CreateDirectory(ConfigFolder));
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
            var _theme = new Theme()
            {
                // ADDED
                AppBackgroundHex = "#50410010",
                AppForegroundHex = "#FFFFFFFF",
                
                // ADDED
                MenuButton_BackgroundHex = "#753A3A3A",
                MenuButton_TextHex = "#FFFFFFFF",
                MenuButton_BackgroundHoverHex = "#1AFFFFFF",
                MenuButton_TextHoverHex = "#FFFFFFFF",
                
                // ADDED
                CloseButton_BackgroundHex = "#50410010",
                CloseButton_IconHex = "#00410010",
                CloseButton_BackgroundHoverHex = "#50C54E81",
                CloseButton_IconHoverHex = "#50410010",
                
                // ADDED
                MaximButton_BackgroundHex = "#50410010",
                MaximButton_IconHex = "#50410010",
                MaximButton_BackgroundHoverHex = "#50C54E81",
                MaximButton_IconHoverHex = "#50410010",
                
                // ADDED
                MinimButton_BackgroundHex = "#50410010",
                MinimButton_IconHex = "#50410010",
                MinimButton_BackgroundHoverHex = "#50C54E81",
                MinimButton_IconHoverHex = "#50410010",
                
                // TO ADD
                LocalListBox_BackgroundHex = "#35670019",
                LocalListBox_ForegroundHex = "#FFFFFFFF",
                LocalListBox_BorderHex = "#258D0022",
                
                // TO ADD
                RemoteListBox_BackgroundHex = "#35670019",
                RemoteListBox_ForegroundHex = "#FFFFFFFF",
                RemoteListBox_BorderHex = "#258D0022",
                
                // TO ADD
                DevicesList_BackgroundHex = "#35670019",
                DevicesList_ForegroundHex = "#FFFFFFFF",
                DevicesList_BorderHex = "#258D0022",
                
                // ADDED
                ActionButton_BackgroundHex = "#50410010",
                ActionButton_IconHex = "#50C54E81",
                ActionButton_BackgroundHoverHex = "#50670019",
                ActionButton_IconHoverHex = "#50C54E81",
            };
            
            ClientTheme.AppBackgroundHex = _theme.AppBackgroundHex;
            ClientTheme.AppForegroundHex = _theme.AppForegroundHex;
            
            ClientTheme.MenuButton_BackgroundHex = _theme.MenuButton_BackgroundHex;
            ClientTheme.MenuButton_TextHex = _theme.MenuButton_TextHex;
            ClientTheme.MenuButton_BackgroundHoverHex = _theme.MenuButton_BackgroundHoverHex;
            ClientTheme.MenuButton_TextHoverHex = _theme.MenuButton_TextHoverHex;
            
            ClientTheme.CloseButton_BackgroundHex = _theme.CloseButton_BackgroundHex;
            ClientTheme.CloseButton_IconHex = _theme.CloseButton_IconHex;
            ClientTheme.CloseButton_BackgroundHoverHex = _theme.CloseButton_BackgroundHoverHex;
            ClientTheme.CloseButton_IconHoverHex = _theme.CloseButton_IconHoverHex;
            
            ClientTheme.MaximButton_BackgroundHex = _theme.MaximButton_BackgroundHex;
            ClientTheme.MaximButton_IconHex = _theme.MaximButton_IconHex;
            ClientTheme.MaximButton_BackgroundHoverHex = _theme.MaximButton_BackgroundHoverHex;
            ClientTheme.MaximButton_IconHoverHex = _theme.MaximButton_IconHoverHex;
            
            ClientTheme.MinimButton_BackgroundHex = _theme.MinimButton_BackgroundHex;
            ClientTheme.MinimButton_IconHex = _theme.MinimButton_IconHex;
            ClientTheme.MinimButton_BackgroundHoverHex = _theme.MinimButton_BackgroundHoverHex;
            ClientTheme.MinimButton_IconHoverHex = _theme.MinimButton_IconHoverHex;
            
            ClientTheme.LocalListBox_BackgroundHex = _theme.LocalListBox_BackgroundHex;
            ClientTheme.LocalListBox_ForegroundHex = _theme.LocalListBox_ForegroundHex;
            ClientTheme.LocalListBox_BorderHex = _theme.LocalListBox_BorderHex;
            
            ClientTheme.RemoteListBox_BackgroundHex = _theme.RemoteListBox_BackgroundHex;
            ClientTheme.RemoteListBox_ForegroundHex = _theme.RemoteListBox_ForegroundHex;
            ClientTheme.RemoteListBox_BorderHex = _theme.RemoteListBox_BorderHex;
            
            ClientTheme.DevicesList_BackgroundHex = _theme.DevicesList_BackgroundHex;
            ClientTheme.DevicesList_ForegroundHex = _theme.DevicesList_ForegroundHex;
            ClientTheme.DevicesList_BorderHex = _theme.DevicesList_BorderHex;
            
            ClientTheme.ActionButton_BackgroundHex = _theme.ActionButton_BackgroundHex;
            ClientTheme.ActionButton_IconHex = _theme.ActionButton_IconHex;
            ClientTheme.ActionButton_BackgroundHoverHex = _theme.ActionButton_BackgroundHoverHex;
            ClientTheme.ActionButton_IconHoverHex = _theme.ActionButton_IconHoverHex;
            
            using (StreamWriter file = File.CreateText(ConfigFilePath))
            {
                string jsonString = JsonConvert.SerializeObject(_theme, Formatting.Indented);
                file.Write(jsonString);
            }
        }
        else
        {
            JObject ConfigFile = JObject.Parse(File.ReadAllText(ConfigFilePath));
            ClientTheme.AppBackgroundHex = (string)ConfigFile["AppBackgroundHex"];
            ClientTheme.AppForegroundHex = (string)ConfigFile["AppForegroundHex"];
            
            ClientTheme.MenuButton_BackgroundHex = (string)ConfigFile["MenuButton_BackgroundHex"];
            ClientTheme.MenuButton_TextHex = (string)ConfigFile["MenuButton_TextHex"];
            ClientTheme.MenuButton_BackgroundHoverHex = (string)ConfigFile["MenuButton_BackgroundHoverHex"];
            ClientTheme.MenuButton_TextHoverHex = (string)ConfigFile["MenuButton_TextHoverHex"];
            
            ClientTheme.CloseButton_BackgroundHex = (string)ConfigFile["CloseButton_BackgroundHex"];
            ClientTheme.CloseButton_IconHex = (string)ConfigFile["CloseButton_IconHex"];
            ClientTheme.CloseButton_BackgroundHoverHex = (string)ConfigFile["CloseButton_BackgroundHoverHex"];
            ClientTheme.CloseButton_IconHoverHex = (string)ConfigFile["CloseButton_IconHoverHex"];
            
            ClientTheme.MaximButton_BackgroundHex = (string)ConfigFile["MaximButton_BackgroundHex"];
            ClientTheme.MaximButton_IconHex = (string)ConfigFile["MaximButton_IconHex"];
            ClientTheme.MaximButton_BackgroundHoverHex = (string)ConfigFile["MaximButton_BackgroundHoverHex"];
            ClientTheme.MaximButton_IconHoverHex = (string)ConfigFile["MaximButton_IconHoverHex"];
            
            ClientTheme.MinimButton_BackgroundHex = (string)ConfigFile["MinimButton_BackgroundHex"];
            ClientTheme.MinimButton_IconHex = (string)ConfigFile["MinimButton_IconHex"];
            ClientTheme.MinimButton_BackgroundHoverHex = (string)ConfigFile["MinimButton_BackgroundHoverHex"];
            ClientTheme.MinimButton_IconHoverHex = (string)ConfigFile["MinimButton_IconHoverHex"];
            
            ClientTheme.LocalListBox_BackgroundHex = (string)ConfigFile["LocalListBox_BackgroundHex"];
            ClientTheme.LocalListBox_ForegroundHex = (string)ConfigFile["LocalListBox_ForegroundHex"];
            ClientTheme.LocalListBox_BorderHex = (string)ConfigFile["LocalListBox_BorderHex"];
            
            ClientTheme.RemoteListBox_BackgroundHex = (string)ConfigFile["RemoteListBox_BackgroundHex"];
            ClientTheme.RemoteListBox_ForegroundHex = (string)ConfigFile["RemoteListBox_ForegroundHex"];
            ClientTheme.RemoteListBox_BorderHex = (string)ConfigFile["RemoteListBox_BorderHex"];
            
            ClientTheme.DevicesList_BackgroundHex = (string)ConfigFile["DevicesList_BackgroundHex"];
            ClientTheme.DevicesList_ForegroundHex = (string)ConfigFile["DevicesList_ForegroundHex"];
            ClientTheme.DevicesList_BorderHex = (string)ConfigFile["DevicesList_BorderHex"];
            
            ClientTheme.ActionButton_BackgroundHex = (string)ConfigFile["ActionButton_BackgroundHex"];
            ClientTheme.ActionButton_IconHex = (string)ConfigFile["ActionButton_IconHex"];
            ClientTheme.ActionButton_BackgroundHoverHex = (string)ConfigFile["ActionButton_BackgroundHoverHex"];
            ClientTheme.ActionButton_IconHoverHex = (string)ConfigFile["ActionButton_IconHoverHex"];
        }
    }

    public class AppStyle : Styles
    {
        public AppStyle(Theme theme)
        {
            var generalStyle = new Style(x => x.OfType<Window>())
            {
                Setters =
                {
                    new Setter(Window.BackgroundProperty, Brush.Parse(theme.AppBackgroundHex)),
                    new Setter(Window.ForegroundProperty, Brush.Parse(theme.AppForegroundHex)),
                    new Setter(Window.ExtendClientAreaToDecorationsHintProperty, true),
                    new Setter(Window.ExtendClientAreaChromeHintsProperty, ExtendClientAreaChromeHints.NoChrome),
                    new Setter(Window.TransparencyLevelHintProperty, new List<WindowTransparencyLevel> { WindowTransparencyLevel.AcrylicBlur }),
                }
            };
            
            Add(generalStyle);
        }
    }
    
    public class MenuButtonStyle : Styles
    {
        public MenuButtonStyle(Theme theme)
        {
            var generalStyle = new Style(x => x.OfType<Button>().Class("MenuButton"))
            {
                Setters =
                {
                    new Setter(Button.BackgroundProperty, Brush.Parse(theme.MenuButton_BackgroundHex)),
                    new Setter(Button.ForegroundProperty, Brush.Parse(theme.MenuButton_TextHex))
                }
            };

            var hoverStyle = new Style(x => x.OfType<Button>().Class("MenuButton").Class(":pointerover"))
            {
                Setters =
                {
                    new Setter(Button.BackgroundProperty, Brush.Parse(theme.MenuButton_BackgroundHoverHex)),
                    new Setter(Button.ForegroundProperty, Brush.Parse(theme.MenuButton_TextHoverHex)),
                }
            };
            
            Add(generalStyle);
            Add(hoverStyle);
        }
    }
    
    public class CloseButtonStyle : Styles
    {
        public CloseButtonStyle(Theme theme)
        {
            var generalStyle = new Style(x => x.OfType<Button>().Name("CloseButton"))
            {
                Setters =
                {
                    new Setter(Button.BackgroundProperty, Brush.Parse(theme.CloseButton_BackgroundHex)),
                    new Setter(Button.ForegroundProperty, Brush.Parse(theme.CloseButton_IconHex))
                }
            };

            var hoverStyle = new Style(x => x.OfType<Button>().Name("CloseButton").Class(":pointerover"))
            {
                Setters =
                {
                    new Setter(Button.BackgroundProperty, Brush.Parse(theme.CloseButton_BackgroundHoverHex)),
                    new Setter(Button.ForegroundProperty, Brush.Parse(theme.CloseButton_IconHoverHex)),
                }
            };
            
            Add(generalStyle);
            Add(hoverStyle);
        }
    }
    
    public class MaximButtonStyle : Styles
    {
        public MaximButtonStyle(Theme theme)
        {
            var generalStyle = new Style(x => x.OfType<Button>().Name("MaximButton"))
            {
                Setters =
                {
                    new Setter(Button.BackgroundProperty, Brush.Parse(theme.MaximButton_BackgroundHex)),
                    new Setter(Button.ForegroundProperty, Brush.Parse(theme.MaximButton_IconHex))
                }
            };

            var hoverStyle = new Style(x => x.OfType<Button>().Name("MaximButton").Class(":pointerover"))
            {
                Setters =
                {
                    new Setter(Button.BackgroundProperty, Brush.Parse(theme.MaximButton_BackgroundHoverHex)),
                    new Setter(Button.ForegroundProperty, Brush.Parse(theme.MaximButton_IconHoverHex)),
                }
            };
            
            Add(generalStyle);
            Add(hoverStyle);
        }
    }
    
    public class MinimButtonStyle : Styles
    {
        public MinimButtonStyle(Theme theme)
        {
            var generalStyle = new Style(x => x.OfType<Button>().Name("MinimButton"))
            {
                Setters =
                {
                    new Setter(Button.BackgroundProperty, Brush.Parse(theme.MinimButton_BackgroundHex)),
                    new Setter(Button.ForegroundProperty, Brush.Parse(theme.MinimButton_IconHex))
                }
            };

            var hoverStyle = new Style(x => x.OfType<Button>().Name("MinimButton").Class(":pointerover"))
            {
                Setters =
                {
                    new Setter(Button.BackgroundProperty, Brush.Parse(theme.MinimButton_BackgroundHoverHex)),
                    new Setter(Button.ForegroundProperty, Brush.Parse(theme.MinimButton_IconHoverHex)),
                }
            };
            
            Add(generalStyle);
            Add(hoverStyle);
        }
    }
    
    public class LocalListBoxStyle : Styles
    {
        public LocalListBoxStyle(Theme theme)
        {
            var generalListStyle = new Style(x => x.OfType<ListBox>().Name("LocalListBox"))
            {
                Setters =
                {
                    new Setter(ListBox.BackgroundProperty, Brush.Parse(theme.RemoteListBox_BackgroundHex)),
                    new Setter(ListBox.BorderBrushProperty, Brush.Parse(theme.RemoteListBox_BorderHex))
                }
            };

            var generalItemStyle = new Style(x => x.OfType<ListBoxItem>().Name("LocalListBoxItem"))
            {
                Setters =
                {
                    new Setter(ListBoxItem.ForegroundProperty, Brush.Parse("Green")),
                }
            };
            
            Add(generalListStyle);
            Add(generalItemStyle);
        }
    }
    
    public class RemoteListBoxStyle : Styles
    {
        public RemoteListBoxStyle(Theme theme)
        {
            var generalListStyle = new Style(x => x.OfType<ListBox>().Name("RemoteListBox"))
            {
                Setters =
                {
                    new Setter(ListBox.BackgroundProperty, Brush.Parse(theme.RemoteListBox_BackgroundHex)),
                    new Setter(ListBox.BorderBrushProperty, Brush.Parse(theme.RemoteListBox_BorderHex))
                }
            };

            var generalItemStyle = new Style(x => x.OfType<ListBoxItem>().Name("RemoteListBoxItem"))
            {
                Setters =
                {
                    new Setter(ListBoxItem.ForegroundProperty, Brush.Parse(theme.RemoteListBox_ForegroundHex)),
                }
            };
            
            Add(generalListStyle);
            Add(generalItemStyle);
        }
    }

    public class ActionButtonStyle : Styles
    {
        public ActionButtonStyle(Theme theme)
        {
            var generalStyle = new Style(x => x.OfType<Button>().Class("ActionButton"))
            {
                Setters =
                {
                    new Setter(Button.BackgroundProperty, Brush.Parse(theme.ActionButton_BackgroundHex)),
                    new Setter(Button.ForegroundProperty, Brush.Parse(theme.ActionButton_IconHex))
                }
            };
            
            var hoverStyle = new Style(x => x.OfType<Button>().Class("ActionButton").Class(":pointerover"))
            {
                Setters =
                {
                    new Setter(Button.BackgroundProperty, Brush.Parse(theme.ActionButton_BackgroundHoverHex)),
                    new Setter(Button.ForegroundProperty, Brush.Parse(theme.ActionButton_IconHoverHex))
                }
            };
            
            Add(generalStyle);
            Add(hoverStyle);
        }
    }
    
    public class Theme
    {
        public string AppBackgroundHex { get; set; }
        public string AppForegroundHex { get; set; }
        
        public string MenuButton_BackgroundHex { get; set; }
        public string MenuButton_TextHex { get; set; }
        public string MenuButton_BackgroundHoverHex { get; set; }
        public string MenuButton_TextHoverHex { get; set; }
        
        public string CloseButton_BackgroundHex { get; set; }
        public string CloseButton_IconHex { get; set; }
        public string CloseButton_BackgroundHoverHex { get; set; }
        public string CloseButton_IconHoverHex { get; set; }
        
        public string MaximButton_BackgroundHex { get; set; }
        public string MaximButton_IconHex { get; set; }
        public string MaximButton_BackgroundHoverHex { get; set; }
        public string MaximButton_IconHoverHex { get; set; }
        
        public string MinimButton_BackgroundHex { get; set; }
        public string MinimButton_IconHex { get; set; }
        public string MinimButton_BackgroundHoverHex { get; set; }
        public string MinimButton_IconHoverHex { get; set; }
        
        public string LocalListBox_BackgroundHex { get; set; }
        public string LocalListBox_ForegroundHex { get; set; }
        public string LocalListBox_BorderHex { get; set; }
        
        public string RemoteListBox_BackgroundHex { get; set; }
        public string RemoteListBox_ForegroundHex { get; set; }
        public string RemoteListBox_BorderHex { get; set; }
        
        public string DevicesList_BackgroundHex { get; set; }
        public string DevicesList_ForegroundHex { get; set; }
        public string DevicesList_BorderHex { get; set; }
        
        public string ActionButton_BackgroundHex { get; set; }
        public string ActionButton_IconHex { get; set; }
        public string ActionButton_BackgroundHoverHex { get; set; }
        public string ActionButton_IconHoverHex { get; set; }
    }
}