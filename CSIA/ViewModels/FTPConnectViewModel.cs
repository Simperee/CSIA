using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CSIA;
using CSIA.Backend;
using CSIA.Views;
using ReactiveUI;
using Realms;
using Realms.Exceptions;
using Realms.Logging;

public class FTPConnectViewModel : ReactiveObject
{
    private PopUpDialog popUpDialog = new PopUpDialog();
    private readonly Window _owner;
    
    //Secure saved device database. I am not risking security for this :)
    private readonly Realm _realm;
    
    //Initialize bindings to set values once device selected.
    private string _savedip;
    
    public string SavedIp
    {
        get => _savedip;
        set => this.RaiseAndSetIfChanged(ref _savedip, value);
    }
    
    private int _savedport;
    
    public int SavedPort
    {
        get => _savedport;
        set => this.RaiseAndSetIfChanged(ref _savedport, value);
    }
    
    private string _saveduser;
    
    public string SavedUser
    {
        get => _saveduser;
        set => this.RaiseAndSetIfChanged(ref _saveduser, value);
    }

    private string _savedpass;
    
    public string SavedPass
    {
        get => _savedpass;
        set => this.RaiseAndSetIfChanged(ref _savedpass, value);
    }
    
    // SavedConItem class defined inside the FTPConnectViewModel
    public class SavedConItem
    {
        public string Name { get; set; }
        public string DeviceName { get; set; }
        public Bitmap Icon { get; set; }

        public SavedConItem(string deviceName, string deviceIP, int devicePort, string deviceUser, string devicePass)
        {
            DeviceName = deviceName;
            // Name = Path.GetFileName(fullPath) == string.Empty ? fullPath : Path.GetFileName(fullPath);
            Name = DeviceName;
            
            Uri iconUri = new Uri("avares://CSIA/Assets/Icons/server_icon.png");

            Icon = new Bitmap(AssetLoader.Open(iconUri));
        }
    }
    
    private ObservableCollection<SavedConItem> _items;

    public ObservableCollection<SavedConItem> Items
    {
        get => _items;
        set => this.RaiseAndSetIfChanged(ref _items, value);
    }

    public FTPConnectViewModel(Window owner)
    {
        Console.WriteLine("FTPConnectViewModel constructor called.");
        _owner = owner;

        var config = new RealmConfiguration(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CSIA", "config", "devices.realm"))
        {
            IsReadOnly = false,
            SchemaVersion = 2, // Update this version whenever the schema changes
            MigrationCallback = (migration, oldSchemaVersion) =>
            {
                Console.WriteLine($"Migrating Realm from version {oldSchemaVersion}...");
                if (oldSchemaVersion < 2)
                {
                    // Migration logic for version 2
                }
                Console.WriteLine("Migration completed.");
            }
        };

        try
        {
            _realm = Realm.GetInstance(config);
            Console.WriteLine("Realm initialized successfully.");
        }
        catch (RealmFileAccessErrorException ex)
        {
            Console.WriteLine($@"Error creating or opening the realm file: {ex.Message}");
        }

        LoadSaved();
    }
    

    public void LoadSaved()
    {
        Console.WriteLine("Running LoadSaved");
        var savedItems = new ObservableCollection<SavedConItem>();
        try
        {
            Console.WriteLine("Reading database");
            var devices = _realm.All<Device>();
            Console.WriteLine($"Device count: {devices.Count()}");
            foreach (var device in devices)
            {
                Console.WriteLine($"Device Name: {device.Name}, IP: {device.IP}, Port: {device.Port}");
            }
            foreach (var device in devices)
            {
                savedItems.Add(new SavedConItem(device.Name, device.IP, device.Port, device.Username, device.Password));
                Console.WriteLine($"Name: {device.Name}. IP: {device.IP}.");
            }

            Items = savedItems;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task SaveDevice(Window _owner, string name, string ip, int port, string username, string password)
    {
        var result = await popUpDialog.ShowSaveDevMessage(_owner, name);
        if (result.ToString() == "Yes")
        {
            //Check if already exists in database
            try
            {
                // Add a new device to the database
                _realm.Write(() =>
                {
                    _realm.Add(new Device
                    {
                        Name = name,
                        IP = ip,
                        Port = port,
                        Username = username,
                        Password = password,
                    });
                });
            }
            catch (Exception ex)
            {
                var existresult = await popUpDialog.ShowExistsMessage(_owner, name);
                if (existresult.ToString() == "Yes")
                {
                    _realm.Write(() =>
                    {
                        // Find the device by primary key (Name)
                        var deviceToRemove = _realm.Find<Device>(name);

                        if (deviceToRemove != null)
                        {
                            // Remove the device from the Realm database
                            _realm.Remove(deviceToRemove);
                        }
                        else
                        {
                            Console.WriteLine($"Device {name} not found.");
                        }
                    });
                    _realm.Write(() =>
                    {
                        _realm.Add(new Device
                        {
                            Name = name,
                            IP = ip,
                            Port = port,
                            Username = username,
                            Password = password,
                        });
                    });
                }
            }
            LoadSaved();
        }
        // Update the Devices collection
    }

    public void LoadSelectedDevice(string name)
    {
        _realm.Write(() =>
        {
            // Find the device by primary key (Name)
            var selectedDevice = _realm.Find<Device>(name);

            if (selectedDevice != null)
            {
                // Get info from selected item in DB.
                SavedIp = selectedDevice.IP;
                SavedPort = selectedDevice.Port;
                SavedUser = selectedDevice.Username;
                SavedPass = selectedDevice.Password;
            }
            else
            {
                Console.WriteLine($"Device {name} not found.");
            }
        });
    }
    
}
public partial class Device : RealmObject
{
    [PrimaryKey]
    public string Name { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string IP { get; set; }
    public int Port { get; set; }
}