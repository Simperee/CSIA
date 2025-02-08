using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CSIA.Views
{
    public partial class FolderNameDialog : Window
    {

        public FolderNameDialog()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            InputTextBox = this.FindControl<TextBox>("InputTextBox");
            AddButton = this.FindControl<Button>("AddButton");
            CancelButton = this.FindControl<Button>("CancelButton");

            AddButton.Click += (sender, e) => Close(true);
            CancelButton.Click += (sender, e) => Close(false);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public static async Task<string?> Show(Window owner)
        {
            var dialog = new FolderNameDialog()
            {
                Owner = owner
            };

            bool? result = await dialog.ShowDialog<bool?>(owner);

            return result == true ? dialog.InputTextBox.Text : @"null/\cancel";
        }
    }
}