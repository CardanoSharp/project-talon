using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ProjectTalon.UI.ViewModels.AddWalletWizardViewModels;
using ReactiveUI;

namespace ProjectTalon.UI.Views.AddWalletWizardViews;

public partial class ShowMnemonicView : ReactiveUserControl<ShowMnemonicViewModel>
{
    private Label word1 => this.FindControl<Label>("Word1");
    private Label word2 => this.FindControl<Label>("Word2");
    private Label word3 => this.FindControl<Label>("Word3");
    private Label word4 => this.FindControl<Label>("Word4");
    private Label word5 => this.FindControl<Label>("Word5");
    private Label word6 => this.FindControl<Label>("Word6");
    private Label word7 => this.FindControl<Label>("Word7");
    private Label word8 => this.FindControl<Label>("Word8");
    private Label word9 => this.FindControl<Label>("Word9");
    private Label word10 => this.FindControl<Label>("Word10");
    private Label word11 => this.FindControl<Label>("Word11");
    private Label word12 => this.FindControl<Label>("Word12");
    private Label word13 => this.FindControl<Label>("Word13");
    private Label word14 => this.FindControl<Label>("Word14");
    private Label word15 => this.FindControl<Label>("Word15");
    private Label word16 => this.FindControl<Label>("Word16");
    private Label word17 => this.FindControl<Label>("Word17");
    private Label word18 => this.FindControl<Label>("Word18");
    private Label word19 => this.FindControl<Label>("Word19");
    private Label word20 => this.FindControl<Label>("Word20");
    private Label word21 => this.FindControl<Label>("Word21");
    private Label word22 => this.FindControl<Label>("Word22");
    private Label word23 => this.FindControl<Label>("Word23");
    private Label word24 => this.FindControl<Label>("Word24");
    
    public ShowMnemonicView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            word1.Content = $"1. {ViewModel?.Mnemonic[0] ?? ""}";
            word2.Content = $"2. {ViewModel?.Mnemonic[1] ?? ""}";
            word3.Content = $"3. {ViewModel?.Mnemonic[2] ?? ""}";
            word4.Content = $"4. {ViewModel?.Mnemonic[3] ?? ""}";
            word5.Content = $"5. {ViewModel?.Mnemonic[4] ?? ""}";
            word6.Content = $"6. {ViewModel?.Mnemonic[5] ?? ""}";
            word7.Content = $"7. {ViewModel?.Mnemonic[6] ?? ""}";
            word8.Content = $"8. {ViewModel?.Mnemonic[7] ?? ""}";
            word9.Content = $"9. {ViewModel?.Mnemonic[8] ?? ""}";
            word10.Content = $"10. {ViewModel?.Mnemonic[9] ?? ""}";
            word11.Content = $"11. {ViewModel?.Mnemonic[10] ?? ""}";
            word12.Content = $"12. {ViewModel?.Mnemonic[11] ?? ""}";
            word13.Content = $"13. {ViewModel?.Mnemonic[12] ?? ""}";
            word14.Content = $"14. {ViewModel?.Mnemonic[13] ?? ""}";
            word15.Content = $"15. {ViewModel?.Mnemonic[14] ?? ""}";
            word16.Content = $"16. {ViewModel?.Mnemonic[15] ?? ""}";
            word17.Content = $"17. {ViewModel?.Mnemonic[16] ?? ""}";
            word18.Content = $"18. {ViewModel?.Mnemonic[17] ?? ""}";
            word19.Content = $"19. {ViewModel?.Mnemonic[18] ?? ""}";
            word20.Content = $"20. {ViewModel?.Mnemonic[19] ?? ""}";
            word21.Content = $"21. {ViewModel?.Mnemonic[20] ?? ""}";
            word22.Content = $"22. {ViewModel?.Mnemonic[21] ?? ""}";
            word23.Content = $"23. {ViewModel?.Mnemonic[22] ?? ""}";
            word24.Content = $"24. {ViewModel?.Mnemonic[23] ?? ""}";
        });
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}