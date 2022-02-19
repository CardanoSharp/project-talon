using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ProjectTalon.UI.ViewModels.AddWalletWizardViewModels;
using ReactiveUI;

namespace ProjectTalon.UI.Views.AddWalletWizardViews;

public partial class EnterMnemonicView : ReactiveUserControl<EnterMnemonicViewModel>
{
    private TextBox word1 => this.FindControl<TextBox>("Word1");
    private TextBox word2 => this.FindControl<TextBox>("Word2");
    private TextBox word3 => this.FindControl<TextBox>("Word3");
    private TextBox word4 => this.FindControl<TextBox>("Word4");
    private TextBox word5 => this.FindControl<TextBox>("Word5");
    private TextBox word6 => this.FindControl<TextBox>("Word6");
    private TextBox word7 => this.FindControl<TextBox>("Word7");
    private TextBox word8 => this.FindControl<TextBox>("Word8");
    private TextBox word9 => this.FindControl<TextBox>("Word9");
    private TextBox word10 => this.FindControl<TextBox>("Word10");
    private TextBox word11 => this.FindControl<TextBox>("Word11");
    private TextBox word12 => this.FindControl<TextBox>("Word12");
    private TextBox word13 => this.FindControl<TextBox>("Word13");
    private TextBox word14 => this.FindControl<TextBox>("Word14");
    private TextBox word15 => this.FindControl<TextBox>("Word15");
    private TextBox word16 => this.FindControl<TextBox>("Word16");
    private TextBox word17 => this.FindControl<TextBox>("Word17");
    private TextBox word18 => this.FindControl<TextBox>("Word18");
    private TextBox word19 => this.FindControl<TextBox>("Word19");
    private TextBox word20 => this.FindControl<TextBox>("Word20");
    private TextBox word21 => this.FindControl<TextBox>("Word21");
    private TextBox word22 => this.FindControl<TextBox>("Word22");
    private TextBox word23 => this.FindControl<TextBox>("Word23");
    private TextBox word24 => this.FindControl<TextBox>("Word24");
    public EnterMnemonicView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
        });
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}