using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ProjectTalon.UI.ViewModels;
using ProjectTalon.UI.ViewModels.AddWalletWizardViewModels;
using ProjectTalon.UI.Views.AddWalletWizardViews;
using ReactiveUI;
using SQLitePCL;

namespace ProjectTalon.UI.Views;

public partial class CreateWalletWindow : ReactiveWindow<CreateWalletViewModel>
{
    public CreateWalletWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif

        this.WhenActivated(d =>
        {
            ViewModel.ExitWizard = ReactiveCommand.CreateFromTask(CloseWindow);
        });
    } 

    private async Task CloseWindow(CancellationToken arg)
    {
        this.Close();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
}