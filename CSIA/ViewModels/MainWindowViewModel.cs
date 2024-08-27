using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;
using ReactiveUI;

namespace CSIA.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            ShowDialog = new Interaction<LogInViewModel, TestViewModel?>();

            OpenFileExplorer = ReactiveCommand.CreateFromTask(async () =>
            {
                var store = new LogInViewModel();

                var result = await ShowDialog.Handle(store);
            });
        }

        public ICommand OpenFileExplorer { get; }

        public Interaction<LogInViewModel, TestViewModel?> ShowDialog { get; }
    }
}