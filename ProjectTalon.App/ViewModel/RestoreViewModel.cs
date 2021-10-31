using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using Microsoft.Maui.Controls;
using ProjectTalon.App.Common;
using ProjectTalon.App.Data;
using ProjectTalon.App.Data.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;
using System.Threading.Tasks;
using ProjectTalon.App.View;

namespace ProjectTalon.App.ViewModel
{
    public class RestoreViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly IMnemonicService _mnemonicService;
        private readonly WalletDatabase _walletDatabase;
        readonly SQLiteAsyncConnection database;
        private readonly WalletKeyDatabase _walletKeyDatabase;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public Command RestoreWallet { get; set; }

        public RestoreViewModel(IMnemonicService mnemonicService, WalletDatabase walletDatabase, WalletKeyDatabase walletKeyDatabase)
        {
            _mnemonicService = mnemonicService;
            _walletDatabase = walletDatabase;
            _walletKeyDatabase = walletKeyDatabase;
        }

        private async Task<WalletView> RestoreMnemonicHDWalletAsync(string mnemonicPharase, string walletName)
        {
            // Restore a Mnemonic
            var mnemonic = _mnemonicService.Restore(mnemonicPharase);

            PrivateKey rootKey = mnemonic.GetRootKey();
            Wallet newlyCreatedWallet; 



            await database.RunInTransactionAsync(async (SQLiteConnection transaction) =>
            {
                transaction.BeginTransaction();

                var walletId = await _walletDatabase.SaveWalletAsync(new Wallet
                {
                    Name = walletName,
                    WalletType = (int)WalletType.HD,
                });

                newlyCreatedWallet = await  _walletDatabase.GetWalletAsync(walletId);



                // Creates the account keys for the wallet
                string paymentPath = $"m/1852'/1815'/0'/0/0";
                PrivateKey paymentPrv = rootKey.Derive(paymentPath);
                PublicKey paymentPub = paymentPrv.GetPublicKey(false);
                await _walletKeyDatabase.SaveWalletAsync(new WalletKey
                {
                    WalletId = newlyCreatedWallet.Id,
                    KeyType = (int)KeyType.Account,
                    Skey = paymentPub.Key.ToString(),
                    Vkey = paymentPrv.Key.ToString(),
                    KeyIndex = 0,

                });


                // Creates the staking keys for the wallet
                string stakePath = $"m/1852'/1815'/0'/2/0";
                PrivateKey stakePrv = rootKey.Derive(stakePath);
                PublicKey stakePub = stakePrv.GetPublicKey(false);
                await _walletKeyDatabase.SaveWalletAsync(new WalletKey
                {
                    WalletId = newlyCreatedWallet.Id,
                    KeyType = (int)KeyType.Staking,
                    Skey = stakePub.Key.ToString(),
                    Vkey = stakePrv.Key.ToString(),
                    KeyIndex = 0,

                });

                transaction.Commit();
            });

            return new WalletView
            {
                Name = walletName,
                WalletType = (int)WalletType.HD,
            }; 
        }
    }
}
