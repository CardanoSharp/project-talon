using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CardanoSharp.Wallet.Models.Transactions;
using ProjectTalon.Core.Common;
using ProjectTalon.Core.Data;
using ProjectTalon.Core.Data.Models;
using ReactiveUI;
using Splat;

namespace ProjectTalon.UI.ViewModels;

public class AuthorizeAppViewModel: ViewModelBase
{
    public AppConnect Connection { get; set; } = new AppConnect()
    {
        Id = 1,
        AppId = Guid.NewGuid().ToString(),
        Name = "Awesome New App",
        ConnectionStatus = (int) ConnectionStatus.Pending
    };
    
    public ICommand ApproveAppCommand { get; }
    public ICommand DenyAppCommand { get; }
    public ICommand CloseWindowCommand { get; set; }

    public AuthorizeAppViewModel()
    {
        ApproveAppCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await UpdateAppConnection(ConnectionStatus.Approved);
            CloseWindowCommand?.Execute(null);
        });
        
        DenyAppCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await UpdateAppConnection(ConnectionStatus.Denied);
            CloseWindowCommand?.Execute(null);
        });
    }

    private async Task UpdateAppConnection(ConnectionStatus status)
    {
        var appConnectionDb = Locator.Current.GetService<IAppConnectDatabase>();
        
        var connection = await appConnectionDb.GetAsync(Connection.Id);
        connection.ConnectionStatus = (int) status;
        
        await appConnectionDb.SaveAsync(connection);
    }
}
