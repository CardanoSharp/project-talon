using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Models.Keys;
using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProjectTalon.App.ViewModel
{
    public class RestoreViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public Command RestoreWallet { get; set; }

        public RestoreViewModel()
        {

        }

        private Mnemonic RestoreMnemonic(string mnemonicPharase)
        {
            // Restore a Mnemonic
            var mnemonic = new MnemonicService().Restore(mnemonicPharase);

            // Fluent derivation API
            var derivation = mnemonic
                .GetMasterNode("password")      
                .Derive(PurposeType.Shelley)    
                .Derive(CoinType.Ada)           
                .Derive(0)                      
                .Derive(RoleType.ExternalChain) 
                .Derive(0);                     

            PrivateKey privateKey = derivation.PrivateKey;
            PublicKey publicKey = derivation.PublicKey;
        }
    }
}
