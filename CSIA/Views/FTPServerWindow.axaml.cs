using System;
using System.Net;
using System.Net.Sockets;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using DouglasDwyer.ExtensibleFtp;
using DouglasDwyer.ExtensibleFtp.Anonymous;
using MsBox.Avalonia;

namespace CSIA.Views
{
    public partial class FTPServerWindow : Window
    {
        // private string? _portvalue;
        public FTPServerWindow()
        {
            InitializeComponent();
            
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
                $"This computer is now hosting on IP: {ip} | port number: {port}",
                MsBox.Avalonia.Enums.ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Error
            
            );

            await messageBox.ShowWindowDialogAsync(this); // Show the popup
        }

        private void HostingButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                ExtensibleFtpServer server = new ExtensibleFtpServer(new AnonymousAuthenticator("C:/"));

                ExtensibleFtpServer testserver =
                    new ExtensibleFtpServer(new FTPAuthenticator("testuser","testpass"));
                
                var portValueControl = this.FindControl<NumericUpDown>("port_value");
                if (portValueControl != null)
                {
                    decimal? portValue = portValueControl.Value;
                    int portNumber = (int)portValue;
                    Console.WriteLine(portNumber);
                    
                    testserver.Start(portNumber);  // Use the port number obtained from NumericUpDown
                    ShowHostingMessage(GetLocalIPAddress(), portNumber.ToString());
                }
                else // No port value specified so running on default FTP port 21
                {
                    Console.WriteLine("port_value is null.");
                    testserver.Start();
                    ShowHostingMessage(GetLocalIPAddress(), "21");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}