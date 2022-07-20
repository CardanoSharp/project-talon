using ProjectTalon.Core.Data.Models;

public class CurrentWalletState
{
    private Wallet? wallet;

    public Wallet? Wallet
    {
        get => wallet;
        set
        {
            wallet = value;
            NotifyStateChanged();
        }
    }

    public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();
}
