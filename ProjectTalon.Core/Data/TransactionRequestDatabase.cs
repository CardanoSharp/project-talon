using ProjectTalon.Core.Common;
using ProjectTalon.Core.Data.Models;

namespace ProjectTalon.Core.Data;

public interface ITransactionRequestDatabase
{
    Task<List<TransactionRequest>> ListAsync();
    Task<TransactionRequest> GetAsync(int id);
    Task<TransactionRequest?> GetPendingAuthAsync();
    Task<TransactionRequest> GetAsync(string referenceId, string appId);
    Task<List<TransactionRequest>> GetByAppIdAsync(string appId);
    Task<int> SaveAsync(TransactionRequest transactionRequest);
}

public class TransactionRequestDatabase: BaseDatabase, ITransactionRequestDatabase
{
    public TransactionRequestDatabase()
    {
        database.CreateTableAsync<TransactionRequest>().Wait();
    }

    public async Task<List<TransactionRequest>> ListAsync()
    {
        return await database.Table<TransactionRequest>().ToListAsync();
    }

    public async Task<TransactionRequest?> GetPendingAuthAsync()
    {
        return await database.Table<TransactionRequest>()
            .Where(i => !i.HasReviewed 
                        && i.StatusId == (int)TransactionRequestStatus.Pending)
            .FirstOrDefaultAsync();
    }

    public async Task<TransactionRequest> GetAsync(int id)
    {
        // Get a specific wallet.
        return await database.Table<TransactionRequest>()
            .Where(i => i.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<TransactionRequest> GetAsync(string referenceId, string appId)
    {
        return await database.Table<TransactionRequest>()
            .Where(i => i.ReferenceId == referenceId && i.AppId == appId)
            .FirstOrDefaultAsync();
    }

    public async Task<List<TransactionRequest>> GetByAppIdAsync(string appId)
    {
        return await database.Table<TransactionRequest>()
            .Where(i => i.AppId == appId)
            .ToListAsync();
    }

    public async Task<int> SaveAsync(TransactionRequest transactionRequest)
    {
        if (transactionRequest.Id != 0)
        {
            // Update an existing wallet.
            return await database.UpdateAsync(transactionRequest);
        }
        else
        {
            // Save a new wallet.
            return await database.InsertAsync(transactionRequest);
        }
    }
}