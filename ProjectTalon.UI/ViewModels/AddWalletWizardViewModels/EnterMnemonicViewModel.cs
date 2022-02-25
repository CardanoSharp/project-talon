using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ReactiveUI;

namespace ProjectTalon.UI.ViewModels.AddWalletWizardViewModels;

public class EnterMnemonicViewModel: ViewModelBase
{
    public List<string> ConfirmMnemonic { get; set; }
    public List<string> CurrentMnemonic { get; set; }
    public ICommand Previous { get; set; }
    public ICommand Next { get; set; }

    public EnterMnemonicViewModel()
    {
        ConfirmMnemonic = new List<string>()
        {
            "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""
        };
    }

    private bool _match;

    public bool Match
    {
        get => _match;
        set => this.RaiseAndSetIfChanged(ref _match, value);
    }

    public void WordCheck()
    {
        var result = true;
        for (var i = 0; i < CurrentMnemonic.Count(); i++)
        {
            if (!ConfirmMnemonic[i].Equals(CurrentMnemonic[i]))
            {
                result = false;
            }
        }

        Match = result;
    }
}