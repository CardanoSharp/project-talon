namespace ProjectTalon.Core.Data
{
    public abstract class BaseDatabase
    {
        protected readonly SQLite.SQLiteAsyncConnection database;

        public BaseDatabase()
        {
            var dbName = "Talon.db3";
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Talon");

            if(!Directory.Exists(dbPath))
                Directory.CreateDirectory(dbPath);

            database = new SQLite.SQLiteAsyncConnection(Path.Combine(dbPath, dbName), true);
        }
    }
}
