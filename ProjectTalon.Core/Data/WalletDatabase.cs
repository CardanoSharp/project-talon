using ProjectTalon.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTalon.Core.Data
{
    public interface IWalletDatabase
    {
        Task<List<Wallet>> GetWalletsAsync();
        Task<Wallet> GetWalletAsync(int id);
        Task<int> SaveWalletAsync(Wallet wallet);
        Task<int> DeleteWalletAsync(Wallet wallet);
    }

    public class WalletDatabase: BaseDatabase, IWalletDatabase
    {
        public WalletDatabase()
        {
            database.CreateTableAsync<Wallet>().Wait();
        }

        public async Task<List<Wallet>> GetWalletsAsync()
        {
            //Get all wallets.
            return await database.Table<Wallet>().ToListAsync();
        }

        public async Task<Wallet> GetWalletAsync(int id)
        {
            // Get a specific wallet.
            return await database.Table<Wallet>()
                            .Where(i => i.Id == id)
                            .FirstOrDefaultAsync();
        }

        public async Task<int> SaveWalletAsync(Wallet wallet)
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

        public async Task<int> DeleteWalletAsync(Wallet wallet)
        {
            // Delete a wallet.
            return await database.DeleteAsync(wallet);
        }
    }
}
