using ProjectTalon.App.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTalon.App.Data
{
    public class WalletKeyDatabase
    {
        readonly SQLite.SQLiteAsyncConnection database;

        public WalletKeyDatabase(string dbPath)
        {
            database = new SQLite.SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<WalletKey>().Wait();
        }

        public Task<List<WalletKey>> GetWalletKeysAsync(int walletId)
        {
            //Get all wallet keys by wallet id.
            return database.Table<WalletKey>()
                            .Where(i => i.WalletId == walletId)
                            .ToListAsync();
        }

        public Task<WalletKey> GetWalletKeyAsync(int id)
        {
            // Get a specific wallet.
            return database.Table<WalletKey>()
                            .Where(i => i.Id == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveWalletAsync(WalletKey walletKey)
        {
            if (walletKey.Id != 0)
            {
                // Update an existing wallet.
                return database.UpdateAsync(walletKey);
            }
            else
            {
                // Save a new wallet.
                return database.InsertAsync(walletKey);
            }
        }

        public Task<int> DeleteWalletAsync(WalletKey walletKey)
        {
            // Delete a wallet.
            return database.DeleteAsync(walletKey);
        }
    }
}
