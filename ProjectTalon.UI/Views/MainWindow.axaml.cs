using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.ReactiveUI;
using ProjectTalon.UI.ViewModels;
using ReactiveUI;

namespace ProjectTalon.UI.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        private int paddingRight = 20;
        private int paddingBottom = 85;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            this.WhenActivated(d => d(ViewModel!.ImportWalletDialog.RegisterHandler(ShowImportWalletDialogAsync)));
            this.WhenActivated(d => d(ViewModel!.GenerateWalletDialog.RegisterHandler(ShowGenerateWalletDialogAsync)));
            
            SetupWindow();
            ExtendClientAreaChromeHints = Avalonia.Platform.ExtendClientAreaChromeHints.OSXThickTitleBar;
        }
        private async Task ShowImportWalletDialogAsync(InteractionContext<AddWalletViewModel, ImportWalletViewModel?> interaction)
        {
            var dialog = new AddWalletWindow();
            dialog.DataContext = interaction.Input;

            var result = await dialog.ShowDialog<ImportWalletViewModel?>(this);
            interaction.SetOutput(result);
        }
        private async Task ShowGenerateWalletDialogAsync(InteractionContext<AddWalletViewModel, GenerateWalletViewModel?> interaction)
        {
            var dialog = new AddWalletWindow();
            dialog.DataContext = interaction.Input;

            var result = await dialog.ShowDialog<GenerateWalletViewModel?>(this);
            interaction.SetOutput(result);
        }
        
        private void SetupWindow()
        {
            Width = 350;
            Height = 600;
            CanResize = false;

            var window = this.GetSelfAndLogicalAncestors().OfType<Window>().First();

            var screen = window.Screens.ScreenFromPoint(Position);

            var newX = screen.Bounds.Width - paddingRight - 350;
            var newY = screen.Bounds.Height - paddingBottom - 600;

            Position = new PixelPoint(newX, newY);
        }

        private void InitializeComponent()
        {

            AvaloniaXamlLoader.Load(this);
        }
    }
}