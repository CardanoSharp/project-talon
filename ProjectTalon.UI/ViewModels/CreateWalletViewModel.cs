using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Models.Keys;
using Microsoft.VisualBasic;
using ProjectTalon.UI.ViewModels.AddWalletWizardViewModels;
using ProjectTalon.UI.Views;
using ProjectTalon.UI.Views.AddWalletWizardViews;
using ReactiveUI;
using IWalletService = ProjectTalon.Core.Services.IWalletService;

namespace ProjectTalon.UI.ViewModels;

public class CreateWalletViewModel: ViewModelBase
{
    private UserControl _activeItem;
    public UserControl ActiveItem 
    {
        get => _activeItem;
        set => this.RaiseAndSetIfChanged(ref _activeItem, value);
    }

    private IWalletService _walletService { get; set; }
    private DisclaimerViewModel DisclaimerStep { get; set; }
    private ShowMnemonicViewModel ShowMnemonicStep { get; set; }
    private EnterMnemonicViewModel EnterMnemonicStep { get; set; }
    private NameAndSecureViewModel NameAndSecureStep { get; set; }
    
    public CreateWalletViewModel(IWalletService walletService)
    {
        _walletService = walletService;
        SetActiveView(CreateWizardSteps.Disclaimer);
    }
    
    private Mnemonic Mnemonic { get; set; }

    private void SetActiveView(CreateWizardSteps step)
    {
        ActiveItem = step switch
        {
            CreateWizardSteps.Disclaimer => SetDisclaimerStepActive(),
            CreateWizardSteps.ShowMnemonic => SetShowMnemonicStepActive(),
            CreateWizardSteps.EnterMnemonic => SetEnterMnemonicStepActive(),
            CreateWizardSteps.NameAndSecure => SetNameAndSecureStepActive()
        };
    }

    private DisclaimerView SetDisclaimerStepActive()
    {
        DisclaimerStep = new DisclaimerViewModel()
        {
            Next = ReactiveCommand.CreateFromTask(DisclaimerNext)
        };
        
        return new DisclaimerView()
        {
            DataContext = DisclaimerStep
        };
    }

    private ShowMnemonicView SetShowMnemonicStepActive()
    {
        if (Mnemonic is null)
            Mnemonic = new MnemonicService().Generate(24);
        
        ShowMnemonicStep = new ShowMnemonicViewModel()
        {
            Mnemonic = Mnemonic.Words.Split(' ').ToList(),
            Next = ReactiveCommand.CreateFromTask(ShowMnemonicNext),
            Previous = ReactiveCommand.CreateFromTask(ShowMnemonicPrevious)
        };
        
        return new ShowMnemonicView()
        {
            DataContext = ShowMnemonicStep
        };
    }

    private EnterMnemonicView SetEnterMnemonicStepActive()
    {
        EnterMnemonicStep = new EnterMnemonicViewModel()
        {
            CurrentMnemonic = Mnemonic.Words.Split(' ').ToList(),
            Next = ReactiveCommand.CreateFromTask(EnterMnemonicNext),
            Previous = ReactiveCommand.CreateFromTask(EnterMnemonicPrevious)
        };
        
        return new EnterMnemonicView()
        {
            DataContext = EnterMnemonicStep
        };
    }

    private NameAndSecureView SetNameAndSecureStepActive()
    {
        NameAndSecureStep = new NameAndSecureViewModel()
        {
            Next = ReactiveCommand.CreateFromTask(NameAndSecureNext),
            Previous = ReactiveCommand.CreateFromTask(NameAndSecurePrevious)
        };
        
        return new NameAndSecureView()
        {
            DataContext = NameAndSecureStep
        };
    }
    
    public ICommand ExitWizard { get; set; }
    private async Task NameAndSecureNext(CancellationToken arg)
    {
        await _walletService.AddWallet(NameAndSecureStep.Name, Mnemonic.Words, NameAndSecureStep.ConfirmPassword);
        
        //close parent window
        ExitWizard.Execute(null);
    }
    
    private async Task NameAndSecurePrevious(CancellationToken arg) =>
        SetActiveView(CreateWizardSteps.EnterMnemonic);
    

    private async Task EnterMnemonicNext(CancellationToken arg) =>
        SetActiveView(CreateWizardSteps.NameAndSecure);
    
    private async Task EnterMnemonicPrevious(CancellationToken arg)
    {
        SetActiveView(CreateWizardSteps.ShowMnemonic);
    }

    private async Task ShowMnemonicNext(CancellationToken arg)
    {
        SetActiveView(CreateWizardSteps.EnterMnemonic);
    }
    
    private async Task ShowMnemonicPrevious(CancellationToken arg)
    {
        SetActiveView(CreateWizardSteps.Disclaimer);
    }

    private async Task DisclaimerNext(CancellationToken arg)
    {
        SetActiveView(CreateWizardSteps.ShowMnemonic);
    }
}

public enum CreateWizardSteps
{
    Disclaimer,
    ShowMnemonic,
    EnterMnemonic,
    NameAndSecure
}