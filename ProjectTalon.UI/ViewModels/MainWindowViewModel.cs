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
using ProjectTalon.Core.Services;
using ProjectTalon.UI.Views;
using Splat;

namespace ProjectTalon.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand CreateWalletCommand { get; }
        public ICommand ImportWalletCommand { get; }
        public ICommand ViewConnectionsCommand { get; }
        public ICommand ViewSettingsCommand { get; }
        public Interaction<ImportWalletViewModel, ImportWalletWizardViewModel?> ImportWalletDialog { get; }
        public Interaction<CreateWalletViewModel, CreateWalletViewModel?> CreateWalletDialog { get; }
        public Interaction<ConnectionsViewModel, ViewConnectionsViewModel?> ViewConnectionsDialog { get; }
        public Interaction<SettingsViewModel, ManageSettingsViewModel?> ViewSettingsDialog { get; }
        
        public MainWindowViewModel()
        {
            ImportWalletDialog = new Interaction<ImportWalletViewModel, ImportWalletWizardViewModel?>();
            CreateWalletDialog = new Interaction<CreateWalletViewModel, CreateWalletViewModel?>();
            ViewConnectionsDialog = new Interaction<ConnectionsViewModel, ViewConnectionsViewModel?>();
            ViewSettingsDialog = new Interaction<SettingsViewModel, ManageSettingsViewModel?>();
            
            ImportWalletCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var vm = new ImportWalletViewModel();

                var result = await ImportWalletDialog.Handle(vm);

                if (result != null)
                {
                    //do something
                }
            });
            
            CreateWalletCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var vm = new CreateWalletViewModel(Locator.Current.GetService<IWalletService>());

                var result = await CreateWalletDialog.Handle(vm);

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
