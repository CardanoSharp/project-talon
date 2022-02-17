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
        public ICommand ViewSettingsCommand { get; }
        public Interaction<ImportWalletViewModel, ImportWalletViewModel?> ImportWalletDialog { get; }
        public Interaction<AddWalletViewModel, GenerateWalletViewModel?> GenerateWalletDialog { get; }
        public Interaction<ConnectionsViewModel, ViewConnectionsViewModel?> ViewConnectionsDialog { get; }
        public Interaction<SettingsViewModel, ManageSettingsViewModel?> ViewSettingsDialog { get; }
        
        public MainWindowViewModel()
        {
            ImportWalletDialog = new Interaction<ImportWalletViewModel, ImportWalletViewModel?>();
            GenerateWalletDialog = new Interaction<AddWalletViewModel, GenerateWalletViewModel?>();
            ViewConnectionsDialog = new Interaction<ConnectionsViewModel, ViewConnectionsViewModel?>();
            ViewSettingsDialog = new Interaction<SettingsViewModel, ManageSettingsViewModel?>();
            
            ImportWalletCommand = ReactiveCommand.CreateFromTask(async () =>
            {

                var result = await ImportWalletDialog.Handle(new ImportWalletViewModel());

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
            
            ViewSettingsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var vm = new SettingsViewModel();

                var result = await ViewSettingsDialog.Handle(vm);

                if (result != null)
                {
                    //do something
                }
            });
        }
    }
}
