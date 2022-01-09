using System.Collections.Generic;
using ProjectTalon.App.Models;
using System.Transactions;
using Microsoft.AspNetCore.Components;
using ProjectTalon.Core.Data;
using System;
using System.Threading.Tasks;

namespace ProjectTalon.App.ViewModel
{
    public interface IWalletDashboardViewModel
    {
        WalletSummary WalletSummary { get; set; }
        List<Token> Tokens { get; set; }
        List<TransactionSummary> Transactions { get; set; }
        void GoTo(string url);
        void FetchDashBoard();
        Task<int> GetWalletCount();
    }

    public class WalletDashboardViewModel : IWalletDashboardViewModel
    {
        private IWalletDatabase _walletDatabase;
        private NavigationManager _uriHelper;
        public WalletSummary WalletSummary { get; set; }
        public List<Token> Tokens { get; set; }
        public List<TransactionSummary> Transactions { get; set; }

        public WalletDashboardViewModel(IWalletDatabase walletDatabase, NavigationManager uriHelper)
        {
            _walletDatabase = walletDatabase;
            _uriHelper = uriHelper;
        }

        public void GoTo(string url)
        {
            _uriHelper.NavigateTo("wallet/" + url);
        }

        public void FetchDashBoard()
        {
            WalletSummary = new()
            {
                Amount = (decimal)370.825743,
                AmountInUSD = (decimal)741.65,
                Name = "Wallet 1",
                CurrencyConversionRate = 2
            };

            Tokens = new();
            Tokens.Add(new()
            {
                Amount = 100000,
                Name = "Token1",
                Asset = "Asset123"
            });

            Transactions = new();
            Transactions.Add(new()
            {
                Amount = 5163,
                Date = DateTime.Now,
                TransactionStatus = (int)Enums.TransactionStatus.Recieved,
                TokenName = "Ada"
            });
        }

        public async Task<int> GetWalletCount()
        {
            return await _walletDatabase.GetWalletCountAsync();
        }
    }
}
