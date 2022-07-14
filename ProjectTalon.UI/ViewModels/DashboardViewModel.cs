using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using ProjectTalon.Core.Data.Models;
using ProjectTalon.UI.Models;
using ReactiveUI;

namespace ProjectTalon.UI.ViewModels;

public class Pnl
{
    public DateTime Time { get; set; }
    public double Value { get; set; }

    public override string ToString()
    {
        return String.Format("{0:HH:mm} {1:0.0}", this.Time, this.Value);
    }
}

public class DashboardViewModel: ReactiveObject
{
    public ObservableCollection<WalletSummary> WalletSummaries { get; set; }

    
    public List<Pnl> Pnls { get; private set; }
    
    public DashboardViewModel()
    {
        WalletSummaries = new ObservableCollection<WalletSummary>();

        Task.Run(() => AddWalletSummaries());
        Task.Run((() => CreatePlot()));
    }

    private Task CreatePlot()
    {
        this.Pnls = new List<Pnl>();

        var random = new Random(31);
        var dateTime = DateTime.Today.Add(TimeSpan.FromHours(9));
        for (var pointIndex = 0; pointIndex < 50; pointIndex++)
        {
            this.Pnls.Add(new Pnl
            {
                Time = dateTime,
                Value = -200 + random.Next(1000),
            });
            dateTime = dateTime.AddMinutes(1);
        }

        return Task.CompletedTask;
    }

    private Task AddWalletSummaries()
    {
        WalletSummaries.Add(new WalletSummary()
        {
            Wallet = new Wallet()
            {
                Name = "Wallet 1"
            }
        });
        WalletSummaries.Add(new WalletSummary()
        {
            Wallet = new Wallet()
            {
                Name = "Wallet 2"
            }
        });
        WalletSummaries.Add(new WalletSummary()
        {
            Wallet = new Wallet()
            {
                Name = "Wallet 3"
            }
        });
        WalletSummaries.Add(new WalletSummary()
        {
            Wallet = new Wallet()
            {
                Name = "Wallet 4"
            }
        });
        WalletSummaries.Add(new WalletSummary()
        {
            Wallet = new Wallet()
            {
                Name = "Wallet 5"
            }
        });
        WalletSummaries.Add(new WalletSummary()
        {
            Wallet = new Wallet()
            {
                Name = "Wallet 6"
            }
        });
        WalletSummaries.Add(new WalletSummary()
        {
            Wallet = new Wallet()
            {
                Name = "Wallet 7"
            }
        });
        WalletSummaries.Add(new WalletSummary()
        {
            Wallet = new Wallet()
            {
                Name = "Wallet 8"
            }
        });
        WalletSummaries.Add(new WalletSummary()
        {
            Wallet = new Wallet()
            {
                Name = "Wallet 9"
            }
        });

        return Task.CompletedTask;
    }
}