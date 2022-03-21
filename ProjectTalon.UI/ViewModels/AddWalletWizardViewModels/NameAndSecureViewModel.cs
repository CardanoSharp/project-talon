using System.Collections.Generic;
using System.Windows.Input;
using SQLitePCL;

namespace ProjectTalon.UI.ViewModels.AddWalletWizardViewModels;

public class NameAndSecureViewModel: ViewModelBase
{
    public string Name { get; set; }
    public string SpendingPassword { get; set; }
    public string ConfirmPassword { get; set; }
    public ICommand Previous { get; set; }
    public ICommand Next { get; set; }
    
    public List<string> Errors { get; set; }
    
}