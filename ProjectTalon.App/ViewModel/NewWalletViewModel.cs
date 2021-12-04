using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using ProjectTalon.App.Common;
using ProjectTalon.App.Data;
using ProjectTalon.App.Data.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions;
using Newtonsoft.Json;

namespace ProjectTalon.App.ViewModel
{
    public interface INewWalletViewModel
    {
        Task RestoreMnemonicHDWalletAsync(NewWalletForm newWallet);
    }

    public class NewWalletViewModel : INewWalletViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly IMnemonicService _mnemonicService;
        private readonly WalletDatabase _walletDatabase;
        private readonly SQLiteAsyncConnection database;
        private readonly WalletKeyDatabase _walletKeyDatabase;

        public Task RestoreWallet { get; private set; }

        public NewWalletViewModel(
            IMnemonicService mnemonicService, 
            WalletDatabase walletDatabase,
            WalletKeyDatabase walletKeyDatabase)
        {
            _mnemonicService = mnemonicService;
            _walletDatabase = walletDatabase;
            _walletKeyDatabase = walletKeyDatabase;
        }

        public async Task RestoreMnemonicHDWalletAsync(NewWalletForm newWallet)
        {
            // Restore a Mnemonic
            var mnemonic = _mnemonicService.Restore(newWallet.SpendingPassword);
            Wallet newlyCreatedWallet;

            await database.RunInTransactionAsync(async (SQLiteConnection transaction) =>
            {
                transaction.BeginTransaction();
                int accountIx = 0;

                var walletId = await _walletDatabase.SaveWalletAsync(new Wallet
                {
                    Name = newWallet.Name,
                    WalletType = (int)WalletType.HD,
                });

                newlyCreatedWallet = await _walletDatabase.GetWalletAsync(walletId);

                var accountNode = mnemonic.GetMasterNode()
                    .Derive(PurposeType.Shelley)
                    .Derive(CoinType.Ada)
                    .Derive(accountIx);

                await _walletKeyDatabase.SaveWalletAsync(new WalletKey
                {
                    WalletId = newlyCreatedWallet.Id,
                    KeyType = (int)KeyType.Account,
                    Skey = JsonConvert.SerializeObject(accountNode.PrivateKey.Key.Encrypt(newWallet.SpendingPassword)),
                    Vkey = JsonConvert.SerializeObject(accountNode.PublicKey.Key),
                    KeyIndex = accountIx
                });

                transaction.Commit();
            });
        }
    }

    public class NewWalletForm
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Mnemonic { get; set; }

        [Required]
        public string SpendingPassword { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }
    }
}
