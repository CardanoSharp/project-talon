using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ProjectTalon.Core.Data;
using ProjectTalon.UI.ViewModels;
using Splat;

namespace ProjectTalon.UI.Views;

public partial class ManageSettingsView : UserControl
{
    public ManageSettingsView()
    {
        InitializeComponent();

        DataContext = new ManageSettingsViewModel(Locator.Current.GetService<ISettingsDatabase>());
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}