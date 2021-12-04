using CardanoSharp.Wallet;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTalon.App.ViewModel
{
    public interface IGenerateMnemonicViewModel
    {
        void GenerateMnemonic();
        void RecordedConfirmed();
        void HandleValidSubmit();

        public WalletCreationRequest WalletCreationForm { get; set; }
        bool ConsentToSeeMnumonic { get; set; }
        bool DoneSeeingMnumonic { get; set; }
        string Mnemonic { get; set; }

    }
    public class GenerateMnemonicViewModel : IGenerateMnemonicViewModel
    {
        private IMnemonicService _mnemonicService;
        private NavigationManager _uriHelper;

        public WalletCreationRequest WalletCreationForm { get; set; }
        public bool ConsentToSeeMnumonic { get; set; } = false;
        public bool DoneSeeingMnumonic { get; set; } = false;
        public string Mnemonic { get; set; }

        public GenerateMnemonicViewModel(IMnemonicService mnemonicService, NavigationManager uriHelper)
        {
            _mnemonicService = mnemonicService;
            _uriHelper = uriHelper;

            WalletCreationForm = new WalletCreationRequest();
        }

        public void GenerateMnemonic()
        {
            Mnemonic = _mnemonicService.Generate(24).Words;
            ConsentToSeeMnumonic = true;
        }

        public void RecordedConfirmed()
        {
            DoneSeeingMnumonic = true;
        }

        public void HandleValidSubmit()
        {
            _uriHelper.NavigateTo("wallet/dashboard");
        }
    }

    public class WalletCreationRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string RecoveryPhrase { get; set; }

        [Required]
        public string SpendingPassword { get; set; }

        [Required]
        public string ConfirmSpendingPassword { get; set; }
    }
}
