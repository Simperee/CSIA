using System;
using System.Net;
using System.Net.Sockets;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FubarDev.FtpServer;
using FubarDev.FtpServer.FileSystem.DotNet;
using FubarDev.FtpServer.Authentication;
using FubarDev.FtpServer.AccountManagement;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using System.IO;
using System.Threading.Tasks;

namespace CSIA.Views
{
    public partial class FTPServerWindow : Window
    {
        private IFtpServerHost _ftpServerHost;
        private TextBox? hostUnameControl;
        private TextBox? hostUpassControl;
        private NumericUpDown? customConControl;

        public FTPServerWindow()
        {
            InitializeComponent();

            customConControl = this.FindControl<NumericUpDown>("port_value");
            hostUnameControl = this.FindControl<TextBox>("hosting_uname");
            hostUpassControl = this.FindControl<TextBox>("hosting_upass");
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

        private async void ShowHostingMessage(string ip, string port)
        {
            var messageBox = MessageBoxManager.GetMessageBoxStandard(
                "FTP Server Online",
                $"This computer is now hosting on IP: {ip} | Port number: {port}",
                MsBox.Avalonia.Enums.ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Info
            );
            await messageBox.ShowWindowDialogAsync(this); // Show the popup
        }

        private void HostingButton_Click(object? sender, RoutedEventArgs e)
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
                ShowHostingMessage(GetLocalIPAddress(), portNumber.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async void StartFtpServer(int port)
        {
            try
            {
                await _ftpServerHost.StartAsync();
                Console.WriteLine($"FTP server started on port {port}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start FTP server: {ex.Message}");
            }
        }

        private async Task StopFtpServer()
        {
            if (_ftpServerHost != null)
            {
                await _ftpServerHost.StopAsync();
                Console.WriteLine("FTP server stopped.");
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
