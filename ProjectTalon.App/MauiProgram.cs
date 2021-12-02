using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using ProjectTalon.App.Data;
using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Maui.LifecycleEvents;

namespace ProjectTalon.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .RegisterBlazorMauiWebView()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.ConfigureLifecycleEvents(lc =>
            {
#if WINDOWS
                lc.AddWindows(w =>
                {
                    w.OnLaunched((_,_) =>
                    {
                        TalonApi.Start();
                    });

                    w.OnClosed((_, _) =>
                    {
                        TalonApi.Stop();
                    });
                });
#endif
            });

            builder.Services.AddBlazorWebView();

            builder.Services.AddTransient<IWalletDatabase, WalletDatabase>();
            builder.Services.AddTransient<IWalletKeyDatabase, WalletKeyDatabase>();

            var app = builder.Build();

            return app;
        }
    }
}