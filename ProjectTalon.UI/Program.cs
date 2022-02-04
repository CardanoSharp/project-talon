﻿using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectTalon.UI
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            Thread api = new Thread(new ThreadStart(() => RunApi(args)));
            api.Start();

            Thread app = new Thread(new ThreadStart(() => RunApp(args)));
            app.Start();
        }

        public static void RunApp(string[] args)
        {
            //Desktop App
            AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI()
                .StartWithClassicDesktopLifetime(args);
        }

        public static void RunApi(string[] args)
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
