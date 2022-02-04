using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ProjectTalon.UI.ViewModels;
using ProjectTalon.UI.Views;

namespace ProjectTalon.UI
{
    public partial class App : Application
    {
        public App()
        {
            DataContext = new ApplicationViewModel();
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel()
                };
                ((ApplicationViewModel)DataContext).SetWindow(desktop.MainWindow);
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}