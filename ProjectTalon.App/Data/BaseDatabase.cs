using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTalon.App.Data
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
