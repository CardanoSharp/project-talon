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

namespace ProjectTalon.App.ViewModel
{
    public class RestoreViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly IMnemonicService _mnemonicService;
        private readonly WalletDatabase _walletDatabase;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public Command RestoreWallet { get; set; }

        public RestoreViewModel(IMnemonicService mnemonicService, WalletDatabase walletDatabase)
        {
            _mnemonicService = mnemonicService;
            _walletDatabase = walletDatabase;
        }

        private Mnemonic RestoreMnemonicHDWallet(string mnemonicPharase, string walletName)
        {
            // Restore a Mnemonic
            var mnemonic = _mnemonicService.Restore(mnemonicPharase);

            PrivateKey rootKey = mnemonic.GetRootKey();

            // This path will give us our Payment Key on index 0
            string paymentPath = $"m/1852'/1815'/0'/0/0";
            // The paymentPrv is Private Key of the specified path.
            PrivateKey paymentPrv = rootKey.Derive(paymentPath);
            // Get the Public Key from the Private Key
            PublicKey paymentPub = paymentPrv.GetPublicKey(false);

            // This path will give us our Stake Key on index 0
            string stakePath = $"m/1852'/1815'/0'/2/0";
            // The stakePrv is Private Key of the specified path
            PrivateKey stakePrv = rootKey.Derive(stakePath);
            // Get the Public Key from the Stake Private Key
            PublicKey stakePub = stakePrv.GetPublicKey(false);

            _walletDatabase.SaveWalletAsync(new Wallet
            {
                Name = walletName,
                WalletType = WalletType.HD,
            });
            
        }
    }
}
