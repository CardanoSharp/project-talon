using ReactiveUI;

namespace ProjectTalon.UI.ViewModels;

public class ImportWalletWizardViewModel: ViewModelBase
{
    
    private ImportWizardSteps _currentStep;

    public ImportWizardSteps CurrentStep
    {
        get => _currentStep;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentStep, value);
            this.RaiseAndSetIfChanged(ref _showStep1, _currentStep == ImportWizardSteps.EnterMnemonic);
            this.RaiseAndSetIfChanged(ref _showStep2, _currentStep == ImportWizardSteps.NameAndSecure);
        } 
    }

    private bool _showStep1 = true;
    private bool _showStep2 = false;

    public bool ShowStep1 => _showStep1;
    public bool ShowStep2 => _showStep2;
}

public enum ImportWizardSteps
{
    EnterMnemonic,
    NameAndSecure
}