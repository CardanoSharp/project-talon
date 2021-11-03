using ProjectTalon.App.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTalon.App.Data
{
    public interface IWalletKeyDatabase
    {
        Task<List<WalletKey>> GetWalletKeysAsync(int walletId);
        Task<WalletKey> GetWalletKeyAsync(int id);
        Task<int> SaveWalletAsync(WalletKey walletKey);
        Task<int> DeleteWalletAsync(WalletKey walletKey);
    }

    public class WalletKeyDatabase: BaseDatabase, IWalletKeyDatabase
    {
        public WalletKeyDatabase()
        {
            database.CreateTableAsync<WalletKey>().Wait();
        }

        public async Task<List<WalletKey>> GetWalletKeysAsync(int walletId)
        {
            //Get all wallet keys by wallet id.
            return await database.Table<WalletKey>()
                            .Where(i => i.WalletId == walletId)
                            .ToListAsync();
        }

        public async Task<WalletKey> GetWalletKeyAsync(int id)
        {
            // Get a specific wallet.
            return await database.Table<WalletKey>()
                            .Where(i => i.Id == id)
                            .FirstOrDefaultAsync();
        }

        public async Task<int> SaveWalletAsync(WalletKey walletKey)
        {
            if (walletKey.Id != 0)
            {
                // Update an existing wallet.
                return await database.UpdateAsync(walletKey);
            }
            else
            {
                // Save a new wallet.
                return await database.InsertAsync(walletKey);
            }
        }

        public async Task<int> DeleteWalletAsync(WalletKey walletKey)
        {
            // Delete a wallet.
            return await database.DeleteAsync(walletKey);
        }
    }
}
