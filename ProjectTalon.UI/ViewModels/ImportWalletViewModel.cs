using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data;
using MiniMvvm;
using ReactiveUI;

namespace ProjectTalon.UI.ViewModels;

public class ImportWalletViewModel: ViewModelBase
{
    public ImportWallet ImportWallet { get; set; }
    public string Phrase { get; set; }
    public ImportWalletViewModel()
    {
    }

    public bool ShowImport => ImportWallet.Phrase == ImportWallet;

    public bool ShowHardware => ImportWallet.Hardware == ImportWallet;

    public void ImportWalletFromPhrase()
    {
        var textBlock = new TextBlock
        {
            [!TextBlock.TextProperty] = new Binding("Phrase")

        };
        Console.WriteLine(textBlock.Text);
    }
}

public enum ImportWallet
{
    Phrase,
    Hardware
}