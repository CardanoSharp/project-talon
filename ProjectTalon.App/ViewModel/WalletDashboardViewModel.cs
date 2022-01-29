using Microsoft.AspNetCore.Components;
using ProjectTalon.App.Models;
using ProjectTalon.Core.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Json;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet;
using ProjectTalon.Core.Data.Models;
using System.ComponentModel;
using ProjectTalon.Core.Common;

namespace ProjectTalon.App.ViewModel
{
    public interface IWalletDashboardViewModel: INotifyPropertyChanged
    {
        WalletSummary WalletSummary { get; set; }
        List<Token> Tokens { get; set; }
        List<TransactionSummary> Transactions { get; set; }
        List<AppConnect> Connections { get; set; }
        List<TransactionRequest> TransactionRequests { get; set; }
        void GoTo(string url);
        Task FetchDashBoard();
        Task<int> GetWalletCount();
        void UpdateConnection(int id, ConnectionStatus connectionStatus);
        void UpdateTransactionRequest(int id, TransactionRequestStatus transactionRequestStatus);
    }

    public class WalletDashboardViewModel : IWalletDashboardViewModel
    {
        private IWalletDatabase _walletDatabase;
        private IWalletKeyDatabase _walletKeyDatabase;
        private IAppConnectDatabase _appConnectDatabase;
        private ITransactionRequestDatabase _transactionRequestDatabase;
        private NavigationManager _uriHelper;
        public WalletSummary WalletSummary { get; set; }
        public List<Token> Tokens { get; set; }
        public List<TransactionSummary> Transactions { get; set; }

        private List<AppConnect> _connections = new List<AppConnect>();
        public List<AppConnect> Connections
        {
            get => _connections;
            set
            {
                if (value != _connections)
                {
                    _connections = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Connections)));
                }
            }
        }

        private List<TransactionRequest> _transactionRequests = new List<TransactionRequest>();
        public List<TransactionRequest> TransactionRequests
        {
            get => _transactionRequests;
            set
            {
                if (value != _transactionRequests)
                {
                    _transactionRequests = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TransactionRequests)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public WalletDashboardViewModel(
            IWalletDatabase walletDatabase,
            IWalletKeyDatabase walletKeyDatabase,
            IAppConnectDatabase appConnectDatabase,
            ITransactionRequestDatabase transactionRequestDatabase,
            NavigationManager uriHelper)
        {
            _walletDatabase = walletDatabase;
            _walletKeyDatabase = walletKeyDatabase;
            _appConnectDatabase = appConnectDatabase;
            _transactionRequestDatabase = transactionRequestDatabase;

            _uriHelper = uriHelper;
        }

        public void GoTo(string url)
        {
            _uriHelper.NavigateTo(url);
        }

        public async Task FetchDashBoard()
        {
            var wallet = (await _walletDatabase.GetWalletsAsync()).FirstOrDefault();

            if (wallet != null)
            {
                var keys = await _walletKeyDatabase.GetWalletKeysAsync(wallet.Id);
                if (keys.Any())
                {
                    var acctPubKey = JsonSerializer.Deserialize<PublicKey>(keys.First().Vkey);

                    var paymentPub = acctPubKey
                        .Derive(RoleType.ExternalChain)
                        .Derive(0);

                    var stakePub = acctPubKey
                        .Derive(RoleType.Staking)
                        .Derive(0);

                    var address = new AddressService().GetAddress(paymentPub.PublicKey, stakePub.PublicKey, NetworkType.Testnet, AddressType.Base);

                    WalletSummary = new()
                    {
                        Amount = (decimal)0,
                        AmountInUSD = (decimal)0,
                        Name = wallet.Name,
                        CurrencyConversionRate = 2,
                        SingleAddress = address.ToString()
                    };

                    GetConnections();
                    GetTransactionRequests();
                }
            }
        }

        public async Task<int> GetWalletCount()
        {
            return await _walletDatabase.GetWalletCountAsync();
        }

        private async Task GetConnections()
        {
            while (true)
            {
                Connections = await _appConnectDatabase.ListAsync();
                await Task.Delay(5000);
            }
        }

        private async Task GetTransactionRequests()
        {
            while (true)
            {
                var transactionRequests = await _transactionRequestDatabase.ListAsync();

                var apps = new List<AppConnect>();
                foreach(var tr in transactionRequests)
                {
                    var app = apps.FirstOrDefault(x => x.AppId.Equals(tr.AppId));
                    if(app is null)
                    {
                        app = await _appConnectDatabase.GetByAppIdAsync(tr.AppId);
                    }
                    tr.App = app;
                }

                TransactionRequests = transactionRequests;

                await Task.Delay(5000);
            }
        }

        public async void UpdateConnection(int id, ConnectionStatus connectionStatus)
        {
            var appConnect = await _appConnectDatabase.GetAsync(id);
            appConnect.ConnectionStatus = (int)connectionStatus;

            await _appConnectDatabase.SaveAsync(appConnect);
        }

        public async void UpdateTransactionRequest(int id, TransactionRequestStatus transactionRequestStatus)
        {
            var transactionRequest = await _transactionRequestDatabase.GetAsync(id);
            transactionRequest.StatusId = (int)transactionRequestStatus;

            if(transactionRequestStatus == TransactionRequestStatus.Accepted)
            {

            }
        }
    }
}
