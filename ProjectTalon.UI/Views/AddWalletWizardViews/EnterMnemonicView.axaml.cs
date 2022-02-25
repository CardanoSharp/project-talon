using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Newtonsoft.Json;
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
            word1.PropertyChanged += TextChanged;
            word2.PropertyChanged += TextChanged;
            word3.PropertyChanged += TextChanged;
            word4.PropertyChanged += TextChanged;
            word5.PropertyChanged += TextChanged;
            word6.PropertyChanged += TextChanged;
            word7.PropertyChanged += TextChanged;
            word8.PropertyChanged += TextChanged;
            word9.PropertyChanged += TextChanged;
            word10.PropertyChanged += TextChanged;
            word11.PropertyChanged += TextChanged;
            word12.PropertyChanged += TextChanged;
            word13.PropertyChanged += TextChanged;
            word14.PropertyChanged += TextChanged;
            word15.PropertyChanged += TextChanged;
            word16.PropertyChanged += TextChanged;
            word17.PropertyChanged += TextChanged;
            word18.PropertyChanged += TextChanged;
            word19.PropertyChanged += TextChanged;
            word20.PropertyChanged += TextChanged;
            word21.PropertyChanged += TextChanged;
            word22.PropertyChanged += TextChanged;
            word23.PropertyChanged += TextChanged;
            word24.PropertyChanged += TextChanged;
            
        });
    }

    private void TextChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if(sender is TextBox)
            SetConfirmingMnemonic((TextBox)sender);
    }

    private void SetConfirmingMnemonic(TextBox box)
    {
        switch(box.Name)
        {
            case "Word1":
                ViewModel.ConfirmMnemonic[0] = box.Text ?? string.Empty;
                break;
            case "Word2":
                ViewModel.ConfirmMnemonic[1] = box.Text ?? string.Empty;
                break;
            case "Word3":
                ViewModel.ConfirmMnemonic[2] = box.Text ?? string.Empty;
                break;
            case "Word4":
                ViewModel.ConfirmMnemonic[3] = box.Text ?? string.Empty;
                break;
            case "Word5":
                ViewModel.ConfirmMnemonic[4] = box.Text ?? string.Empty;
                break;
            case "Word6":
                ViewModel.ConfirmMnemonic[5] = box.Text ?? string.Empty;
                break;
            case "Word7":
                ViewModel.ConfirmMnemonic[6] = box.Text ?? string.Empty;
                break;
            case "Word8":
                ViewModel.ConfirmMnemonic[7] = box.Text ?? string.Empty;
                break;
            case "Word9":
                ViewModel.ConfirmMnemonic[8] = box.Text ?? string.Empty;
                break;
            case "Word10":
                ViewModel.ConfirmMnemonic[9] = box.Text ?? string.Empty;
                break;
            case "Word11":
                ViewModel.ConfirmMnemonic[10] = box.Text ?? string.Empty;
                break;
            case "Word12":
                ViewModel.ConfirmMnemonic[11] = box.Text ?? string.Empty;
                break;
            case "Word13":
                ViewModel.ConfirmMnemonic[12] = box.Text ?? string.Empty;
                break;
            case "Word14":
                ViewModel.ConfirmMnemonic[13] = box.Text ?? string.Empty;
                break;
            case "Word15":
                ViewModel.ConfirmMnemonic[14] = box.Text ?? string.Empty;
                break;
            case "Word16":
                ViewModel.ConfirmMnemonic[15] = box.Text ?? string.Empty;
                break;
            case "Word17":
                ViewModel.ConfirmMnemonic[16] = box.Text ?? string.Empty;
                break;
            case "Word18":
                ViewModel.ConfirmMnemonic[17] = box.Text ?? string.Empty;
                break;
            case "Word19":
                ViewModel.ConfirmMnemonic[18] = box.Text ?? string.Empty;
                break;
            case "Word20":
                ViewModel.ConfirmMnemonic[19] = box.Text ?? string.Empty;
                break;
            case "Word21":
                ViewModel.ConfirmMnemonic[20] = box.Text ?? string.Empty;
                break;
            case "Word22":
                ViewModel.ConfirmMnemonic[21] = box.Text ?? string.Empty;
                break;
            case "Word23":
                ViewModel.ConfirmMnemonic[22] = box.Text ?? string.Empty;
                break;
            case "Word24":
                ViewModel.ConfirmMnemonic[23] = box.Text ?? string.Empty;
                break;
        }

        Console.WriteLine(JsonConvert.SerializeObject(ViewModel.ConfirmMnemonic));
        
        ViewModel.WordCheck();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}