using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using System.Linq;

namespace ProjectTalon.UI.Views
{
    public partial class MainWindow : Window
    {
        private int paddingRight = 20;
        private int paddingBottom = 85;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            Width = 350;
            Height = 600;
            CanResize = false;

            var window = this.GetSelfAndLogicalAncestors().OfType<Window>().First();

            var screen = window.Screens.ScreenFromPoint(Position);

            var newX = screen.Bounds.Width - paddingRight - 350;
            var newY = screen.Bounds.Height - paddingBottom - 600;

            Position = new PixelPoint(newX, newY);

            ExtendClientAreaChromeHints = Avalonia.Platform.ExtendClientAreaChromeHints.OSXThickTitleBar;
        }

        private void InitializeComponent()
        {

            AvaloniaXamlLoader.Load(this);
        }
    }
}