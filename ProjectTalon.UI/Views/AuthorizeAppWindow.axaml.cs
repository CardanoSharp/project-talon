using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ProjectTalon.UI.ViewModels;

namespace ProjectTalon.UI.Views;

public partial class AuthorizeAppWindow : ReactiveWindow<AuthorizeAppViewModel>
{
    public AuthorizeAppWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

