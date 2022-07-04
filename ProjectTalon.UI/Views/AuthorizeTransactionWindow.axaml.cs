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
    private TextBox word1 => this.FindControl<TextBox>("Password");
    public AuthorizeTransactionWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif

        this.WhenActivated(d =>
        {
            word1.PropertyChanged += TextChanged;
            ViewModel.CloseWindowCommand = ReactiveCommand.CreateFromTask(CloseWindow);
        });
    }    
    
    private void TextChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if(sender is TextBox)
        {
            var box = (TextBox) sender;
            ViewModel.UpdatePassword(box.Text);
        }
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

