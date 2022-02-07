﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ProjectTalon.UI.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
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