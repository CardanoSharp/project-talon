using System.Windows.Input;

namespace ProjectTalon.UI.ViewModels.AddWalletWizardViewModels;

public class NameAndSecureViewModel: ViewModelBase
{
    public ICommand Previous { get; set; }
    public ICommand Next { get; set; }
}