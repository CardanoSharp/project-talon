using ProjectTalon.Core.Data.Models;

namespace ProjectTalon.Core.Data;
public interface ISettingsDatabase
{
    Task<List<Settings>> ListAsync();
    Task<Settings> GetAsync(int id);
    Task<Settings> GetByKeyAsync(string key);
    Task<int> SaveAsync(Settings setting);
}

public class SettingsDatabase: BaseDatabase, ISettingsDatabase
{
    public SettingsDatabase()
    {
        database.CreateTableAsync<Settings>().Wait();
    }

    public async Task<List<Settings>> ListAsync()
    {
        return await database.Table<Settings>().ToListAsync();
    }

    public async Task<Settings> GetAsync(int id)
    {
        return await database.Table<Settings>()
            .Where(i => i.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Settings> GetByKeyAsync(string key)
    {
        return await database.Table<Settings>()
            .Where(i => i.Key.Equals(key))
            .FirstOrDefaultAsync();
    }

    public async Task<int> SaveAsync(Settings setting)
    {
        if (setting.Id != 0)
        {
            // Update an existing wallet.
            return await database.UpdateAsync(setting);
        }
        else
        {
            // Save a new wallet.
            return await database.InsertAsync(setting);
        }
    }
}