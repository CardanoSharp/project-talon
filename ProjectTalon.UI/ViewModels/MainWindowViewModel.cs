using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;
using JetBrains.Annotations;
using Avalonia.ReactiveUI;
using Avalonia.Controls;
using Avalonia;
using ReactiveUI;
using ProjectTalon.Core.Data;

namespace ProjectTalon.UI.ViewModels
{
    public interface IMainWindowViewModel
    {
        
    }
    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        
        public ICommand ImportWalletCommand { get; }
        public Interaction<AddWalletViewModel, ImportWalletViewModel?> ImportWalletDialog { get; }
        
        public MainWindowViewModel()
        {
            ImportWalletDialog = new Interaction<AddWalletViewModel, ImportWalletViewModel?>();
            
            ImportWalletCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var vm = new AddWalletViewModel();

                var result = await ImportWalletDialog.Handle(vm);

                if (result != null)
                {
                    //do something
                }
            });
        }
    }
}
