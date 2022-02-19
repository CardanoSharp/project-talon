using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;

namespace ProjectTalon.UI.ViewModels.AddWalletWizardViewModels;

public class DisclaimerViewModel: ViewModelBase
{
    public ICommand Next { get; set; }


    private bool _accepted;
    public bool Accepted { 
        get => _accepted; 
        set => this.RaiseAndSetIfChanged(ref _accepted, value); 
    }
}