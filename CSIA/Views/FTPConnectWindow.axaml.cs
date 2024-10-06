using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Rendering.SceneGraph;
using CSIA.Backend;
using FluentFTP;

namespace CSIA.Views
{
    public partial class FTPConnectWindow : Window
    {
        PopUpDialog popUpDialog = new PopUpDialog();
        
        private TextBox? connectUnameControl;
        private TextBox? connectUpassControl;
        private TextBox? connectIPControl;
        private NumericUpDown? customConControl;
        private CheckBox? customConEnable;
        private Button? focusLossButton;
        private FtpClient? client;
        public FTPConnectWindow()
        {
            InitializeComponent();

            customConControl = this.FindControl<NumericUpDown>("connect_portval");
            customConEnable = this.FindControl<CheckBox>("customconn_enable");
            connectUnameControl = this.FindControl<TextBox>("connect_uname");
            connectUpassControl = this.FindControl<TextBox>("connect_upass");
            connectIPControl = this.FindControl<TextBox>("connect_ip");
            focusLossButton = this.FindControl<Button>("focusloss_button");
            ConnectButton = this.FindControl<Button>("ConnectButton");
            if (ConnectButton != null)
            {
                ConnectButton.Click += ConnectButton_Click;
            }
            CancelButton = this.FindControl<Button>("CancelButton");
            if (CancelButton != null)
            {
                CancelButton.Click += CancelButton_Click;
            }
            
            connectUnameControl.GotFocus += UnameFocus;
            connectUpassControl.GotFocus += UpassFocus;
            connectIPControl.GotFocus += IPFocus;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void ConnectButton_Click(object? sender, RoutedEventArgs e)
        {
            Console.WriteLine("sex");
            if (customConEnable.IsChecked == true)
            {
                int port = Convert.ToInt32(customConControl.Text);
                Console.WriteLine(port);
                client = new FtpClient(connectIPControl.Text, connectUnameControl.Text, connectUpassControl.Text, port);
                client.Connect();
                Hide();
            }
            else
            {
                client = new FtpClient(connectIPControl.Text, connectUnameControl.Text, connectUpassControl.Text);
                client.Connect();
                Hide();
            }
        }
        
        private void UnameFocus(object? sender, RoutedEventArgs e)
        {
            if (connectUnameControl.BorderBrush != Brush.Parse("#99ffffff"))
            {
                connectUnameControl.BorderBrush = Brush.Parse("#99ffffff");
            }
        }

        private void UpassFocus(object? sender, RoutedEventArgs e)
        {
            if (connectUpassControl.BorderBrush != Brush.Parse("#99ffffff"))
            {
                connectUpassControl.BorderBrush = Brush.Parse("#99ffffff");
            }
        }
        
        private void IPFocus(object? sender, RoutedEventArgs e)
        {
            if (connectIPControl.BorderBrush != Brush.Parse("#99ffffff"))
            {
                connectIPControl.BorderBrush = Brush.Parse("#99ffffff");
            }
        }
        
        private async void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            if (connectUnameControl.Text != null || connectUpassControl.Text != null || connectIPControl.Text != null || customConControl.IsEnabled){
                try
                {
                    var result = await popUpDialog.ShowDataLossMessage(this);
                    if (result.ToString() == "Yes")
                    {
                        Close();
                    }
                }
                catch (Exception ex)
                {
                    popUpDialog.ShowErrorMessage(this, ex.Message);
                }
            }
            else
            {
                Close();
            }
        }
        protected override void OnClosing(WindowClosingEventArgs e)
        {
            connectUnameControl.Text = null;
            connectUpassControl.Text = null;
            connectIPControl.Text = null;
            customConControl.Value = 23;
            customConEnable.IsChecked = false;
            connectUnameControl.BorderBrush = Brush.Parse("#99ffffff");
            connectUpassControl.BorderBrush = Brush.Parse("#99ffffff");
            connectIPControl.BorderBrush = Brush.Parse("#99ffffff");
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
}