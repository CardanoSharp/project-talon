namespace ProjectTalon.UI.ViewModels;

public class AddWalletViewModel: ViewModelBase
{
    public WalletCreation WalletCreation { get; set; }
    public AddWalletViewModel()
    {
        
    }

    public bool ShowImport
    {
        get
        {
            return WalletCreation.Import == WalletCreation;
        }
    }

    public bool ShowGenerate
    {
        get
        {
            return WalletCreation.Generate == WalletCreation;
        }
    }
}

public enum WalletCreation
{
    Import,
    Generate
}