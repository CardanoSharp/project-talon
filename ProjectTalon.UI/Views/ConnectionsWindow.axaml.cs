using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ProjectTalon.UI.Views;

public partial class ConnectionsWindow : Window
{
    public ConnectionsWindow()
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