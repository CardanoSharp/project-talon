using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ProjectTalon.UI.ViewModels;
using ReactiveUI;

namespace ProjectTalon.UI.Views;

public partial class DashboardView : ReactiveUserControl<DashboardViewModel>
{
    public DashboardView()
    {
        DataContext = new DashboardViewModel();

        this.WhenActivated(d =>
        {
            
        });
        
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}