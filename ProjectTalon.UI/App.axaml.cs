using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectTalon.Core.Data;
using ProjectTalon.UI.ViewModels;
using ProjectTalon.UI.Views;
using Splat;

namespace ProjectTalon.UI
{
    public partial class App : Application
    {
        public App()
        {
            DataContext = new ApplicationViewModel();
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            // Thread api = new Thread(new ThreadStart(() => RunApi(args)));
            // api.Start();
            
            base.OnFrameworkInitializationCompleted();
        }

        public void RunApi(string[] args)
        {
            //Api
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapGet("/hello", () =>
            {
                return Results.Ok("Hello, World!");
            });

            app.Run();
        }
    }
}