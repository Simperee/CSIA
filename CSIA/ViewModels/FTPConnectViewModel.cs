using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CSIA.Backend;
using ReactiveUI;
using Realms;

public class FTPConnectViewModel : ReactiveObject
{
    private PopUpDialog popUpDialog = new PopUpDialog();
    private readonly Window _owner;
    
    //Secure saved device database. I am not risking security for this :)
    private readonly Realm _realm;
    
    // SavedConItem class defined inside the FTPConnectViewModel
    public class SavedConItem
    {
        public string Name { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public Bitmap Icon { get; set; }

        public SavedConItem(string deviceName, string deviceIP, string deviceType)
        {
            DeviceName = deviceName;
            DeviceType = deviceType;
            // Name = Path.GetFileName(fullPath) == string.Empty ? fullPath : Path.GetFileName(fullPath);
            Name = DeviceName;

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
        var config = new RealmConfiguration("devices.realm")
        {
            IsReadOnly = false
        };
        _realm = Realm.GetInstance(config);
        LoadSaved();
    }

    public void LoadSaved()
    {
        var savedItems = new ObservableCollection<SavedConItem>();
        try
        {
            var devices = _realm.All<Device>();
            foreach (var device in devices)
            {
                savedItems.Add(new SavedConItem(device.Name, device.IP, device.Type));
                Console.WriteLine($"Name: {device.Name}. IP: {device.IP}. Type: {device.Type}");
            }
        }
        catch (Exception ex)
        {
            popUpDialog.ShowErrorMessage(_owner, ex.Message);
        }
    }

    public void SaveDevice(string name)
    {
        // Add a new device to the database
        _realm.Write(() =>
        {
            _realm.Add(new Device { Name = name });
        });

        // Update the Devices collection
    }
    
}
public class Device : RealmObject
{
    public string Name { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string IP { get; set; }
    public int port { get; set; }
    public string Type { get; set; }
}