using ProjectTalon.Core.Data.Models;

namespace ProjectTalon.Core.Data
{
    public interface IAppConnectDatabase
    {
        Task<List<AppConnect>> GetAppConnectionsAsync();
        Task<AppConnect> GetAppConnectionAsync(int id);
        Task<AppConnect> GetAppConnectionByAppIdAsync(string appId);
        Task<int> SaveAppConnectionAsync(AppConnect appConnect);
        Task<int> DeleteAppConnectionAsync(AppConnect appConnect);
    }

    public class AppConnectDatabase : BaseDatabase, IAppConnectDatabase
    {
        public AppConnectDatabase()
        {
            database.CreateTableAsync<AppConnect>().Wait();
        }

        public async Task<List<AppConnect>> GetAppConnectionsAsync()
        {
            return await database.Table<AppConnect>().ToListAsync();
        }

        public async Task<AppConnect> GetAppConnectionAsync(int id)
        {
            return await database.Table<AppConnect>()
                            .Where(i => i.Id == id)
                            .FirstOrDefaultAsync();
        }

        public async Task<AppConnect> GetAppConnectionByAppIdAsync(string appId)
        {
            return await database.Table<AppConnect>()
                            .Where(i => i.AppId == appId)
                            .FirstOrDefaultAsync();
        }

        public async Task<int> SaveAppConnectionAsync(AppConnect appConnect)
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
            
        public async Task<int> DeleteAppConnectionAsync(AppConnect appConnect)
        {
            return await database.DeleteAsync(appConnect);
        }
    }
}
