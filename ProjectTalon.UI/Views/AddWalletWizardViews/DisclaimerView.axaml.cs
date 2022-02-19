using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ProjectTalon.UI.ViewModels;
using ProjectTalon.UI.ViewModels.AddWalletWizardViewModels;
using ReactiveUI;

namespace ProjectTalon.UI.Views.AddWalletWizardViews;

public partial class DisclaimerView : ReactiveUserControl<DisclaimerViewModel>
{
    public CheckBox AcceptedCheckBox => this.FindControl<CheckBox>("cbAccept");
    public DisclaimerView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            AcceptedCheckBox.Command = ReactiveCommand.CreateFromTask(async () =>
            {
                if(AcceptedCheckBox.IsChecked.HasValue)
                    ViewModel.Accepted = AcceptedCheckBox.IsChecked.Value;
            });
        });
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}