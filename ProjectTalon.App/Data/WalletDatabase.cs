using ProjectTalon.App.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTalon.App.Data
{
    public class WalletDatabase
    {
        readonly SQLite.SQLiteAsyncConnection database;

        public WalletDatabase(string dbPath)
        {
            database = new SQLite.SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<Wallet>().Wait();
        }

        public Task<List<Wallet>> GetWalletsAsync()
        {
            //Get all wallets.
            return database.Table<Wallet>().ToListAsync();
        }

        public Task<Wallet> GetWalletAsync(int id)
        {
            // Get a specific wallet.
            return database.Table<Wallet>()
                            .Where(i => i.Id == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveWalletAsync(Wallet wallet)
        {
            if (wallet.Id != 0)
            {
                // Update an existing wallet.
                return database.UpdateAsync(wallet);
            }
            else
            {
                // Save a new wallet.
                return database.InsertAsync(wallet);
            }
        }

        public Task<int> DeleteWalletAsync(Wallet wallet)
        {
            // Delete a wallet.
            return database.DeleteAsync(wallet);
        }
    }
}
