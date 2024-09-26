using System;
using System.Net;
using System.Net.Sockets;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FubarDev.FtpServer;
using FubarDev.FtpServer.FileSystem.DotNet;
using FubarDev.FtpServer.AccountManagement;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using System.Threading.Tasks;
using CSIA.Backend;
using FubarDev.FtpServer.ConnectionChecks;

namespace CSIA.Views
{
    public partial class FTPServerWindow : Window
    {
        PopUpDialog popUpDialog = new PopUpDialog();
        
        private IFtpServerHost _ftpServerHost;
        public bool ftpRunning;
        private TextBox? hostUnameControl;
        private TextBox? hostUpassControl;
        private NumericUpDown? customConControl;
        private CheckBox? customConEnable;
        private Button? focusLossButton;

        public FTPServerWindow()
        {
            InitializeComponent();

            customConControl = this.FindControl<NumericUpDown>("port_value");
            customConEnable = this.FindControl<CheckBox>("customconn_enable");
            hostUnameControl = this.FindControl<TextBox>("hosting_uname");
            hostUpassControl = this.FindControl<TextBox>("hosting_upass");
            focusLossButton = this.FindControl<Button>("focusloss_button");
            HostingButton = this.FindControl<Button>("HostingButton");
            if (HostingButton != null)
            {
                HostingButton.Click += HostingButton_Click;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private void HostingButton_Click(object? sender, RoutedEventArgs e)
        {
            if (!ftpRunning)
            {
                try
                {
                    var services = new ServiceCollection();
                    services.Configure<FtpServerOptions>(opt => opt.ServerAddress = "0.0.0.0");

                    string username = hostUnameControl?.Text ?? "defaultUser";
                    string password = hostUpassControl?.Text ?? "defaultPass";

                    services.AddFtpServer(builder =>
                    {
                        builder.UseDotNetFileSystem(); 
                        builder.Services.AddSingleton<IMembershipProvider>(new CustomMembershipProvider(username, password));
                    });

                    services.Configure<DotNetFileSystemOptions>(opt =>
                    {
                        opt.RootPath = @"C:\";  
                    });

                    int portNumber;
                    
                    if (customConControl.IsEnabled)
                    {
                        decimal? portValue = customConControl?.Value;
                        portNumber = (int)portValue;
                    }
                    else
                    {
                        decimal? portValue = 21;
                        portNumber = (int)portValue;
                    }

                    services.Configure<FtpServerOptions>(opt => opt.Port = portNumber);
                    var serviceProvider = services.BuildServiceProvider();
                    _ftpServerHost = serviceProvider.GetRequiredService<IFtpServerHost>();

                    StartFtpServer(portNumber);
                }
                catch (Exception ex)
                {
                    popUpDialog.ShowErrorMessage(this, ex.Message);
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                popUpDialog.ServerRunningFunction(this, this);
            }
        }

        private async void StartFtpServer(int port)
        {
            try
            {
                await _ftpServerHost.StartAsync();
                Console.WriteLine($"FTP server started on port {port}.");
                ftpRunning = true;
                var result = await popUpDialog.ShowHostingMessage(this, GetLocalIPAddress(), port.ToString());
                if (result.ToString() == "Ok" || result.ToString() == "None")
                {
                    Hide();
                }
            }
            catch (Exception ex)
            {
                popUpDialog.ShowErrorMessage(this, ex.Message);
                Console.WriteLine($"Failed to start FTP server: {ex.Message}");
                ftpRunning = false;
            }
        }

        public async Task StopFtpServer()
        {
            if (_ftpServerHost != null)
            {
                await _ftpServerHost.StopAsync();
                Console.WriteLine("FTP server stopped.");
                ftpRunning = false;
            }
            else
            {
                Console.WriteLine("fail");
            }
        }
        
        protected override void OnClosing(WindowClosingEventArgs e)
        {
            hostUnameControl.Text = null;
            hostUpassControl.Text = null;
            customConControl.Value = 23;
            customConEnable.IsChecked = false;
            ClearFocus();
            e.Cancel = true;
            Hide();
        }

        private void ClearFocus()
        {
            if (!focusLossButton.IsFocused)
            {
                
                focusLossButton.Focus();
            }
        }

    }

    // Custom authentication logic
    public class CustomMembershipProvider : IMembershipProvider
    {
        private readonly string _username;
        private readonly string _password;

        public CustomMembershipProvider(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public Task<MemberValidationResult> ValidateUserAsync(string username, string password)
        {
            if (username == _username && password == _password)
            {
                var user = new FtpUser(username);
                return Task.FromResult(new MemberValidationResult(MemberValidationStatus.AuthenticatedUser, user));
            }
            return Task.FromResult(new MemberValidationResult(MemberValidationStatus.InvalidLogin));
        }
    }

    // User class implementing IFtpUser
    public class FtpUser : IFtpUser
    {
        public string Name { get; }

        public FtpUser(string name)
        {
            Name = name;
        }

        public bool IsInGroup(string groupName)
        {
            // Return false if group management is not needed
            return false;
        }
    }
}
