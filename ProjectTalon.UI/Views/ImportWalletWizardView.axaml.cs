using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ProjectTalon.UI.ViewModels;

namespace ProjectTalon.UI.Views;

public partial class ImportWalletWizardView : UserControl
{
    public ImportWalletWizardView()
    {
        InitializeComponent();
        DataContext = new ImportWalletWizardViewModel
        {
            CurrentStep = ImportWizardSteps.EnterMnemonic
        };
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}