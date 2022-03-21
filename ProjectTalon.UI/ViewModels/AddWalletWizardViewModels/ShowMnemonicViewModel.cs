using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Windows.Input;
using Avalonia.Controls;
using Microsoft.CodeAnalysis.Scripting.Hosting;

namespace ProjectTalon.UI.ViewModels.AddWalletWizardViewModels;

public class ShowMnemonicViewModel: ViewModelBase
{
    public List<string> Mnemonic { get; set; }

    public ICommand Previous { get; set; }
    public ICommand Next { get; set; }
}