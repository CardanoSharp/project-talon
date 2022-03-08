using ProjectTalon.Core.Data.Models;

namespace ProjectTalon.Core.Data
{
    public interface IWalletDatabase
    {
        Task<Wallet?> GetFirstAsync();
        Task<List<Wallet>> ListAsync();
        Task<Wallet?> GetByIdAsync(int id);
        Task<Wallet?> GetByNameAsync(string name);
        Task<int> GetCountAsync();
        Task<bool> ExistsAsync(string name);
        Task<int> SaveAsync(Wallet wallet);
        Task<int> DeleteAsync(Wallet wallet);
    }

    public class WalletDatabase : BaseDatabase, IWalletDatabase
    {
        public WalletDatabase()
        {
            database.CreateTableAsync<Wallet>().Wait();
        }

        public async Task<Wallet?> GetFirstAsync()
        {
            return (await database.Table<Wallet>().ToListAsync()).FirstOrDefault();
        }
        public async Task<List<Wallet>> ListAsync()
        {
            //Get all wallets.
            return await database.Table<Wallet>().ToListAsync();
        }

        public async Task<Wallet?> GetByIdAsync(int id)
        {
            // Get a specific wallet.
            return await database.Table<Wallet>()
                            .Where(i => i.Id == id)
                            .FirstOrDefaultAsync();
        }

        public async Task<Wallet?> GetByNameAsync(string name)
        {
            return await database.Table<Wallet>()
                    .Where(i => i.Name == name)
                    .FirstOrDefaultAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await database.Table<Wallet>().CountAsync();
        }

        public async Task<bool> ExistsAsync(string name)
        {
            return (await database.Table<Wallet>()
                    .Where(i => i.Name == name)
                    .FirstOrDefaultAsync()) != null;
        }

        public async Task<int> SaveAsync(Wallet wallet)
        {
            if (wallet.Id != 0)
            {
                // Update an existing wallet.
                return await database.UpdateAsync(wallet);
            }
            else
            {
                // Save a new wallet.
                return await database.InsertAsync(wallet);
            }
        }

        public async Task<int> DeleteAsync(Wallet wallet)
        {
            // Delete a wallet.
            return await database.DeleteAsync(wallet);
        }
    }
}
