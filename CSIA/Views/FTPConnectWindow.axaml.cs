using System;
using System.Net;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using CSIA.Backend;

namespace CSIA.Views
{
    public partial class FTPConnectWindow : Window
    {
        PopUpDialog popUpDialog = new PopUpDialog();
        
        private MainWindow _mainWindow;
        
        private TextBox? connectUnameControl;
        private TextBox? connectUpassControl;
        private TextBox? connectIPControl;
        private NumericUpDown? customConControl;
        private CheckBox? customConEnable;
        private Button? focusLossButton;
        public FTPConnectWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            DataContext = new FTPConnectViewModel(this);

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
            SaveDeviceButton = this.FindControl<Button>("SaveDeviceButton");
            if (SaveDeviceButton != null)
            {
                SaveDeviceButton.Click += SaveDeviceButton_Click;
            }
            
            connectUnameControl.GotFocus += UnameFocus;
            connectUpassControl.GotFocus += UpassFocus;
            connectIPControl.GotFocus += IPFocus;
            
            SavedDevListBox = this.FindControl<ListBox>("SavedDevListBox");
            if (SaveDeviceButton != null)
            {
                SavedDevListBox.DoubleTapped += SavedDevListBox_DoubleTapped;
            }
            
            _mainWindow = mainWindow;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void ConnectButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (customConEnable.IsChecked == true)
                {
                    int port = Convert.ToInt32(customConControl.Text);
                    if (FTPClass.PingHost(connectIPControl.Text, port))
                    {
                        if (!FTPClass.Instance.Connect(connectIPControl.Text, port, connectUnameControl.Text,
                                connectUpassControl.Text))
                        {
                            popUpDialog.ShowAuthFailMessage(this);
                        }
                        else
                        {
                            _mainWindow.DataContext =
                                new MainWindowViewModel(_mainWindow, _mainWindow.CurrentLocalPath.Text, FTPClass.Instance.RemotePath);
                            if (DataContext is FTPConnectViewModel viewModel)
                            {
                                await viewModel.SaveDevice(
                                    this,
                                    $"{connectUnameControl.Text}@{connectIPControl.Text}:{port}",
                                    connectIPControl.Text,
                                    port,
                                    connectUnameControl.Text,
                                    connectUpassControl.Text
                                );
                            }

                            Close();
                        }
                    }
                    else
                    {
                        popUpDialog.ShowPingFailMessage(this, connectIPControl.Text, port);
                    }

                }
                else
                {
                    if (FTPClass.PingHost(connectIPControl.Text, 21))
                    {
                        if (!FTPClass.Instance.Connect(connectIPControl.Text, 21, connectUnameControl.Text,
                                connectUpassControl.Text))
                        {
                            popUpDialog.ShowAuthFailMessage(this);
                        }
                        else
                        {
                            _mainWindow.DataContext =
                                new MainWindowViewModel(_mainWindow, _mainWindow.CurrentLocalPath.Text, FTPClass.Instance.RemotePath);
                            if (DataContext is FTPConnectViewModel viewModel)
                            {
                                await viewModel.SaveDevice(
                                    this,
                                    $"{connectUnameControl.Text}@{connectIPControl.Text}:21",
                                    connectIPControl.Text,
                                    21,
                                    connectUnameControl.Text,
                                    connectUpassControl.Text
                                );
                            }

                            Close();
                        }
                    }
                    else
                    {
                        popUpDialog.ShowPingFailMessage(this, connectIPControl.Text, 21);
                    }
                }
            }
            catch (Exception ex)
            {
                popUpDialog.ShowErrorMessage(this, ex.Message);
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
        
        private void SavedDevListBox_DoubleTapped(object? sender, RoutedEventArgs e)
        {
            if (DataContext is FTPConnectViewModel viewModel && SavedDevListBox.SelectedItem is FTPConnectViewModel.SavedConItem selectedItem)
            {
                viewModel.LoadSelectedDevice(selectedItem.Name);
            }
        }

        private async void SaveDeviceButton_Click(object? sender, RoutedEventArgs e)
        {
            
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