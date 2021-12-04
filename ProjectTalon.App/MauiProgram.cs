using CardanoSharp.Wallet;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using ProjectTalon.App.Data;
using ProjectTalon.App.ViewModel;
using System;
using System.IO;

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

            builder.Services.AddBlazorWebView();

            //CardanoSharp
            builder.Services.AddTransient<IMnemonicService, MnemonicService>();

            //ViewModels
            builder.Services.AddTransient<IGenerateMnemonicViewModel, GenerateMnemonicViewModel>();

            //SQLite
            builder.Services.AddTransient<IWalletDatabase, WalletDatabase>();
            builder.Services.AddTransient<IWalletKeyDatabase, WalletKeyDatabase>();

            return builder.Build();
        }
    }
}