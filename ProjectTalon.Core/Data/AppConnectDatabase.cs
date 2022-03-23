using ProjectTalon.Core.Common;
using ProjectTalon.Core.Data.Models;

namespace ProjectTalon.Core.Data
{
    public interface IAppConnectDatabase
    {
        Task<List<AppConnect>> ListAsync();
        Task<AppConnect> GetAsync(int id);
        Task<AppConnect?> GetPendingAuthAsync();
        Task<AppConnect> GetByAppIdAsync(string appId);
        Task<int> SaveAsync(AppConnect appConnect);
        Task<int> DeleteAsync(AppConnect appConnect);
    }

    public class AppConnectDatabase : BaseDatabase, IAppConnectDatabase
    {
        public AppConnectDatabase()
        {
            database.CreateTableAsync<AppConnect>().Wait();
        }

        public async Task<List<AppConnect>> ListAsync()
        {
            return await database.Table<AppConnect>().ToListAsync();
        }

        public async Task<AppConnect> GetAsync(int id)
        {
            return await database.Table<AppConnect>()
                            .Where(i => i.Id == id)
                            .FirstOrDefaultAsync();
        }
        public async Task<AppConnect?> GetPendingAuthAsync()
        {
            return await database.Table<AppConnect>()
                .Where(i => !i.HasReviewed 
                    && i.ConnectionStatus == (int)ConnectionStatus.Pending)
                .FirstOrDefaultAsync();
        }

        public async Task<AppConnect> GetByAppIdAsync(string appId)
        {
            return await database.Table<AppConnect>()
                            .Where(i => i.AppId == appId)
                            .FirstOrDefaultAsync();
        }

        public async Task<int> SaveAsync(AppConnect appConnect)
        {
            if (appConnect.Id != 0)
            {
                return await database.UpdateAsync(appConnect);
            }
            else
            {
                return await database.InsertAsync(appConnect);
            }
        }
            
        public async Task<int> DeleteAsync(AppConnect appConnect)
        {
            return await database.DeleteAsync(appConnect);
        }
    }
}
