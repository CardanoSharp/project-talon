using Blockfrost.Api.Extensions;
using CardanoSharp.Wallet;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;
using ProjectTalon.App.ViewModel;
using ProjectTalon.Core.Data;

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
                    w.OnLaunched((_, _) =>
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

            //CardanoSharp
            builder.Services.AddTransient<IMnemonicService, MnemonicService>();

            //ViewModels & Services
            builder.Services.AddTransient<IGenerateMnemonicViewModel, GenerateMnemonicViewModel>();
            //builder.Services.AddTransient<IAddressListViewModel, AddressListViewModel>();
            builder.Services.AddTransient<INewWalletViewModel, NewWalletViewModel>();
            builder.Services.AddTransient<IRestoreViewModel, RestoreViewModel>();
            builder.Services.AddTransient<ISendFundsViewModel, SendFundsViewModel>();
            builder.Services.AddTransient<IStakingViewModel, StakingViewModel>();
            builder.Services.AddTransient<ITransactionHistoryViewModel, TransactionHistoryViewModel>();
            builder.Services.AddTransient<IWalletDashboardViewModel, WalletDashboardViewModel>();
            builder.Services.AddTransient<IWalletLayoutViewModel, WalletLayoutViewModel>();
            builder.Services.AddTransient<Services.IWalletService, Services.WalletService>();

            builder.Services.AddBlockfrost("testnet", "kL2vAF27FpfuzrnhSofc1JawdlL0BNkh");

            //SQLite
            builder.Services.AddTransient<IWalletDatabase, WalletDatabase>();
            builder.Services.AddTransient<IWalletKeyDatabase, WalletKeyDatabase>();
            builder.Services.AddTransient<IAppConnectDatabase, AppConnectDatabase>();
            builder.Services.AddTransient<ITransactionRequestDatabase, TransactionRequestDatabase>();

            var app = builder.Build();

            return app;
        }
    }
}