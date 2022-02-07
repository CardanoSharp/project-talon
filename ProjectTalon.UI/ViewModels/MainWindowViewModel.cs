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
using ProjectTalon.UI.Views;

namespace ProjectTalon.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand GenerateWalletCommand { get; }
        public ICommand ImportWalletCommand { get; }
        public ICommand ViewConnectionsCommand { get; }
        public Interaction<AddWalletViewModel, ImportWalletViewModel?> ImportWalletDialog { get; }
        public Interaction<AddWalletViewModel, GenerateWalletViewModel?> GenerateWalletDialog { get; }
        public Interaction<ConnectionsViewModel, ViewConnectionsViewModel?> ViewConnectionsDialog { get; }
        
        public MainWindowViewModel()
        {
            ImportWalletDialog = new Interaction<AddWalletViewModel, ImportWalletViewModel?>();
            GenerateWalletDialog = new Interaction<AddWalletViewModel, GenerateWalletViewModel?>();
            ViewConnectionsDialog = new Interaction<ConnectionsViewModel, ViewConnectionsViewModel?>();
            
            ImportWalletCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var vm = new AddWalletViewModel();

                var result = await ImportWalletDialog.Handle(vm);

                if (result != null)
                {
                    //do something
                }
            });
            
            GenerateWalletCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var vm = new AddWalletViewModel();

                var result = await GenerateWalletDialog.Handle(vm);

                if (result != null)
                {
                    //do something
                }
            });
            
            ViewConnectionsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var vm = new ConnectionsViewModel();

                var result = await ViewConnectionsDialog.Handle(vm);

                if (result != null)
                {
                    //do something
                }
            });
        }
    }
}
