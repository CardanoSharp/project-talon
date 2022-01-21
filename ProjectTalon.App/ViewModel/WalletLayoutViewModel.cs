using ProjectTalon.Core.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectTalon.App.ViewModel
{
    public interface IWalletLayoutViewModel
    {
        List<Core.Data.Models.Wallet> Wallets { get; set; }
        int CurrentWallet { get; set; }
        Task GetWallets();
    }

    public class WalletLayoutViewModel : IWalletLayoutViewModel
    {
        private IWalletDatabase _walletDatabase;

        public int CurrentWallet { get; set; }
        public List<Core.Data.Models.Wallet> Wallets { get; set; } = new List<Core.Data.Models.Wallet>();

        public WalletLayoutViewModel(IWalletDatabase walletDatabase)
        {
            _walletDatabase = walletDatabase;
        }

        public async Task GetWallets()
        {
            Wallets = await _walletDatabase.GetWalletsAsync();
            if (Wallets.Any())
            {
                CurrentWallet = 1;
            }
        }
    }
}
