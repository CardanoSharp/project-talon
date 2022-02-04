using Hardcodet.Wpf.TaskbarNotification.Interop;
using Microsoft.Maui;
using Microsoft.UI.Xaml;
using ProjectTalon.App.Services;
using System;

namespace ProjectTalon.App.WinUI;

public class TrayService : ITrayService
{
    WindowsTrayIcon tray;

    public Action ClickHandler { get; set; }

    public void Initialize()
    {
        tray = new WindowsTrayIcon("Platforms/Windows/trayicon.ico");
        tray.LeftClick = () => {
            WindowExtensions.BringToFront();
            ClickHandler?.Invoke();
        };
    }
}