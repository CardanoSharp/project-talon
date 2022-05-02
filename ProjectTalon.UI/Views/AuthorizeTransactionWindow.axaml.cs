using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ProjectTalon.UI.ViewModels;
using ReactiveUI;

namespace ProjectTalon.UI.Views;

public partial class AuthorizeTransactionWindow : ReactiveWindow<AuthorizeTransactionViewModel>
{
    public AuthorizeTransactionWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif

        this.WhenActivated(d =>
        {
            ViewModel.CloseWindowCommand = ReactiveCommand.CreateFromTask(CloseWindow);
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

