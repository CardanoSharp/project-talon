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

public class AuthorizeTransactionViewModel: ViewModelBase
{
    public TransactionRequest TransactionRequest { get; set; }
    public ICommand ApproveAppCommand { get; }
    public ICommand DenyAppCommand { get; }
    public ICommand CloseWindowCommand { get; set; }

    public AuthorizeTransactionViewModel()
    {
        ApproveAppCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await SubmitTransaction();
            CloseWindowCommand?.Execute(null);
        });
        
        DenyAppCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await RejectTransaction();
            CloseWindowCommand?.Execute(null);
        });
    }

    private async Task SubmitTransaction()
    {
        
    }

    private async Task RejectTransaction()
    {
        //successful transaction
        var transactionRequestDb = Locator.Current.GetService<ITransactionRequestDatabase>();
        
        var transactionRequest = await transactionRequestDb.GetAsync(TransactionRequest.Id);
        transactionRequest.StatusId = (int) TransactionRequestStatus.Rejected;
        
        await transactionRequestDb.SaveAsync(transactionRequest);
    }
}
