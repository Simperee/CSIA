using Avalonia.ReactiveUI;
using CSIA.ViewModels;
using ReactiveUI;
using System.Threading.Tasks;

namespace CSIA.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WhenActivated(action =>
                action(ViewModel!.ShowDialog.RegisterHandler(DoShowAsync)));
        }

        private async Task DoShowAsync(InteractionContext<LogInViewModel, 
            TestViewModel?> interaction)
        {
            var dialog = new LogInWindow();
            dialog.DataContext = interaction.Input;

            var result = await dialog.ShowDialog<TestViewModel?>(this);
            interaction.SetOutput(result);
        }
    }
}