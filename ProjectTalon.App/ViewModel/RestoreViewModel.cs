using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using ProjectTalon.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTalon.App.ViewModel
{
    public interface IRestoreViewModel
    {
        EditContext? EditContext { get; set; }
        WalletCreationRequest WalletCreationForm { get; set; }
        void HandleValidSubmit();
    }

    public class RestoreViewModel: IRestoreViewModel
    {
        private Core.Services.IWalletService _walletService;
        private NavigationManager _uriHelper;
        private ValidationMessageStore? messageStore;

        public EditContext? EditContext { get; set; }

        public WalletCreationRequest WalletCreationForm { get; set; }

        public RestoreViewModel(Core.Services.IWalletService walletService, NavigationManager uriHelper)
        {
            _walletService = walletService;
            _uriHelper = uriHelper;

            WalletCreationForm = new WalletCreationRequest();
            EditContext = new(WalletCreationForm);
            EditContext.OnValidationRequested += HandleValidationRequested;
            messageStore = new(EditContext);
        }

        public async void HandleValidSubmit()
        {
            await _walletService.AddWallet(WalletCreationForm.Name, WalletCreationForm.RecoveryPhrase, WalletCreationForm.SpendingPassword);
            _uriHelper.NavigateTo("");
        }

        private void HandleValidationRequested(
            object? sender,
            ValidationRequestedEventArgs args)
        {
            messageStore?.Clear();

            if (WalletCreationForm.SpendingPassword != WalletCreationForm.ConfirmSpendingPassword)
            {
                messageStore?.Add(() => WalletCreationForm.ConfirmSpendingPassword, "Spending passwords do not match.");
            }
        }
    }
}
