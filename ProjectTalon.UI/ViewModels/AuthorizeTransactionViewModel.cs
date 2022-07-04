using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CardanoSharp.Wallet.Models.Transactions;
using ProjectTalon.Core.Common;
using ProjectTalon.Core.Data;
using ProjectTalon.Core.Data.Models;
using ProjectTalon.Core.Services;
using ReactiveUI;
using Splat;

namespace ProjectTalon.UI.ViewModels;

public class AuthorizeTransactionViewModel: ViewModelBase
{
    public TransactionRequest TransactionRequest { get; set; }
    public ICommand ApproveAppCommand { get; }
    public ICommand DenyAppCommand { get; }
    public ICommand CloseWindowCommand { get; set; }

    private readonly ITransactionService _transactionService;

    private string _password;
    private bool _hasPassword;
    public bool HasPassword
    {
        get => _hasPassword;
        set => this.RaiseAndSetIfChanged(ref _hasPassword, value);
    }
    

    public AuthorizeTransactionViewModel(ITransactionService transactionService)
    {
        _transactionService = transactionService;
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

    public void UpdatePassword(string enteredPassword)
    {
        HasPassword = !string.IsNullOrEmpty(enteredPassword);
        _password = enteredPassword;
    }

    private async Task SubmitTransaction()
    {
        var txHash = await _transactionService.SubmitTransactionAsync(TransactionRequest, _password);
        
        var transactionRequestDb = Locator.Current.GetService<ITransactionRequestDatabase>();
        
        var transactionRequest = await transactionRequestDb.GetAsync(TransactionRequest.Id);
        if (!string.IsNullOrEmpty(txHash))
        {
            transactionRequest.StatusId = (int) TransactionRequestStatus.Submitted;
            transactionRequest.TransactionHash = txHash;
        }
        else
        {
            transactionRequest.StatusId = (int) TransactionRequestStatus.Rejected;
        }

        await transactionRequestDb.SaveAsync(transactionRequest);
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
