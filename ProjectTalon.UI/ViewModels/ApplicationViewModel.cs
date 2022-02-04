using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using MiniMvvm;

namespace ProjectTalon.UI.ViewModels
{
    public class ApplicationViewModel : ViewModelBase
    {
        private Window _window { get; set; }

        public ApplicationViewModel()
        {
            ExitCommand = MiniCommand.Create(() =>
            {
                if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
                {
                    lifetime.Shutdown();
            }
            });

            ToggleCommand = MiniCommand.Create(() => { });
            FocusWindowCommand = MiniCommand.Create(() => {
                if (_window is not null)
                {
                    _window.WindowState = WindowState.Normal;
                    _window.Activate();
                }
            });
        }

        public void SetWindow(Window window)
        {
            _window = window;
        }

        public MiniCommand ExitCommand { get; }

        public MiniCommand ToggleCommand { get; }

        public MiniCommand FocusWindowCommand { get; }
    }
}
