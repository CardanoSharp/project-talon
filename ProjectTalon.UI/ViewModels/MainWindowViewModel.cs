using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Linq;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Threading;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using DynamicData;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;
using ProjectTalon.Core.Common;
using ReactiveUI;
using ProjectTalon.Core.Data;
using ProjectTalon.Core.Data.Models;
using ProjectTalon.Core.Services;
using Splat;
using AddressService = CardanoSharp.Wallet.AddressService;

namespace ProjectTalon.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IWalletDatabase _walletDatabase;
        private readonly IWalletKeyDatabase _walletKeyDatabase;
        private readonly ISettingsDatabase _settingsDatabase;
        
        public bool AuthAppWindowIsOpen { get; set; }
        public bool AuthTransactionWindowIsOpen { get; set; }
        
        public ICommand CreateWalletCommand { get; }
        public ICommand ImportWalletCommand { get; }
        public ICommand ViewConnectionsCommand { get; }
        public ICommand ViewSettingsCommand { get; }
        public ICommand AuthorizeAppCommand { get; }
        public ICommand AuthorizeTransactionCommand { get; }
        public Interaction<ImportWalletViewModel, ImportWalletWizardViewModel?> ImportWalletDialog { get; }
        public Interaction<CreateWalletViewModel, CreateWalletViewModel?> CreateWalletDialog { get; }
        public Interaction<ConnectionsViewModel, ViewConnectionsViewModel?> ViewConnectionsDialog { get; }
        public Interaction<SettingsViewModel, ManageSettingsViewModel?> ViewSettingsDialog { get; }
        public Interaction<AuthorizeAppViewModel, AuthorizeAppViewModel?> AuthorizeAppDialog { get; }
        public Interaction<AuthorizeTransactionViewModel, AuthorizeTransactionViewModel?> AuthorizeTransactionDialog { get; }

        private int? _walletId;
        public int? WalletId
        {
            get => _walletId;
            set
            {
                this.RaiseAndSetIfChanged(ref _walletId, value);
                HasWallet = _walletId.HasValue;
            }
        }

        private bool _hasWallet;
        public bool HasWallet
        {
            get => _hasWallet;
            set => this.RaiseAndSetIfChanged(ref _hasWallet, value);
        }

        private string _address;
        public string Address
        {
            get => _address;
            set
            {
                this.RaiseAndSetIfChanged(ref _address, value);

                var addressSections = _address.Length / 4;
                Address1 = _address.Substring(0, addressSections);
                Address2 = _address.Substring(addressSections, addressSections);
                Address3 = _address.Substring(addressSections * 2, addressSections);
                Address4 = _address.Substring(addressSections * 3, addressSections);
            }
        }

        private string _address1;
        public string Address1
        {
            get => _address1;
            set => this.RaiseAndSetIfChanged(ref _address1, value);
        }

        private string _address2;
        public string Address2
        {
            get => _address2;
            set => this.RaiseAndSetIfChanged(ref _address2, value);
        }

        private string _address3;
        public string Address3
        {
            get => _address3;
            set => this.RaiseAndSetIfChanged(ref _address3, value);
        }

        private string _address4;
        public string Address4
        {
            get => _address4;
            set => this.RaiseAndSetIfChanged(ref _address4, value);
        }
        
        public MainWindowViewModel()
        {
            _walletDatabase = Locator.Current.GetService<IWalletDatabase>();
            _walletKeyDatabase = Locator.Current.GetService<IWalletKeyDatabase>();
            _settingsDatabase = Locator.Current.GetService<ISettingsDatabase>();

            this.WhenAnyValue(x => x.WalletId)
                .Subscribe( x => Task.Run(SetupWalletDashboard));
            
            ImportWalletDialog = new Interaction<ImportWalletViewModel, ImportWalletWizardViewModel?>();
            CreateWalletDialog = new Interaction<CreateWalletViewModel, CreateWalletViewModel?>();
            ViewConnectionsDialog = new Interaction<ConnectionsViewModel, ViewConnectionsViewModel?>();
            ViewSettingsDialog = new Interaction<SettingsViewModel, ManageSettingsViewModel?>();
            AuthorizeAppDialog = new Interaction<AuthorizeAppViewModel, AuthorizeAppViewModel?>();
            AuthorizeTransactionDialog = new Interaction<AuthorizeTransactionViewModel, AuthorizeTransactionViewModel?>();
            
            ImportWalletCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var vm = new ImportWalletViewModel();

                await ImportWalletDialog.Handle(vm);
                await WalletExists();
            });
            
            CreateWalletCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var vm = new CreateWalletViewModel(Locator.Current.GetService<Core.Services.IWalletService>());

                await CreateWalletDialog.Handle(vm);
                await WalletExists();
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

                await ViewSettingsDialog.Handle(vm);
            });
            
            AuthorizeAppCommand = ReactiveCommand.CreateFromTask(async (AppConnect connection) =>
            {
                var vm = new AuthorizeAppViewModel()
                {
                    Connection = connection
                };

                var result = await AuthorizeAppDialog.Handle(vm);

                if (result != null)
                {
                    //do something
                }
            });
            
            AuthorizeTransactionCommand = ReactiveCommand.CreateFromTask(async (TransactionRequest request) =>
            {
                var vm = new AuthorizeTransactionViewModel(Locator.Current.GetService<ITransactionService>())
                { 
                    TransactionRequest = request
                };

                var result = await AuthorizeTransactionDialog.Handle(vm);

                if (result != null)
                {
                    //do something
                }
            });

            Task.Run(async () => await WalletExists());
            Task.Run(async () => await CheckPendingConnections());
            Task.Run(async () => await CheckPendingTransactionRequests());
        }

        private async Task CheckPendingConnections()
        {
            var appConnectionDb = Locator.Current.GetService<IAppConnectDatabase>();

            while (true)
            {
                if (!AuthAppWindowIsOpen)
                {
                    var connection = await appConnectionDb.GetPendingAuthAsync();
                    if (connection != null)
                    {
                        AuthAppWindowIsOpen = true;
                        connection.HasReviewed = true;
                        await appConnectionDb.SaveAsync(connection);

                        await Dispatcher.UIThread.InvokeAsync(() => AuthorizeAppCommand.Execute(connection));
                    }
                }
            }
        }

        private async Task CheckPendingTransactionRequests()
        {
            var transactionRequestDatabase = Locator.Current.GetService<ITransactionRequestDatabase>();

            while (true)
            {
                if (!AuthTransactionWindowIsOpen)
                {
                    var transactionRequest = await transactionRequestDatabase.GetPendingAuthAsync();
                    if (transactionRequest != null)
                    {
                        var appDatabase = Locator.Current.GetService<IAppConnectDatabase>();
                        transactionRequest.App = await appDatabase.GetByAppIdAsync(transactionRequest.AppId);
                        
                        AuthTransactionWindowIsOpen = true;
                        transactionRequest.HasReviewed = true;
                        await transactionRequestDatabase.SaveAsync(transactionRequest);

                        await Dispatcher.UIThread.InvokeAsync(() => AuthorizeTransactionCommand.Execute(transactionRequest));
                    }
                }
            }
        }

        private async Task SetupWalletDashboard()
        {
            Address = await GetWalletAddress();
        }
        
        private async Task<string> GetWalletAddress()
        {
            var wallet = await _walletKeyDatabase.GetFirstAsync();
            var publicKey = JsonConvert.DeserializeObject<PublicKey>(wallet.Vkey);
            if (publicKey is null)
                throw new Exception("Wallet not found");
            var payment = publicKey
                .Derive(RoleType.ExternalChain)
                .Derive(0);

            var stake = publicKey
                .Derive(RoleType.Staking)
                .Derive(0);

            var address = new AddressService()
                .GetBaseAddress(payment.PublicKey, stake.PublicKey, NetworkType.Testnet);

            return address.ToString();
        }

        private async Task<bool> WalletExists()
        {
            var wallet = await _walletDatabase.GetFirstAsync();
            
            WalletId = wallet?.Id;

            return wallet is not null;
        }

        public void CopyAddress()
        {
            Application.Current.Clipboard.SetTextAsync(Address);
        }

        public async void ViewWallet()
        {
            var settings = await _settingsDatabase.GetByKeyAsync(SettingKeys.NETWORK);
            if (settings is null) return;

            var network = settings.Value == NetworkOptions.TESTNET
                ? "testnet"
                : "";
            var url = $"https://{network}.cardanoscan.io/address/{Address}";
            
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
    }
}
