namespace ProjectTalon.Core.Data
{
    public abstract class BaseDatabase
    {
        protected readonly SQLite.SQLiteAsyncConnection database;

        public BaseDatabase()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Talon.db3");
            database = new SQLite.SQLiteAsyncConnection(dbPath);
        }
    }
}
