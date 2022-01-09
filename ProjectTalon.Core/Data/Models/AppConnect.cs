using SQLite;

namespace ProjectTalon.Core.Data.Models
{
    public class AppConnect
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string AppId { get; set; }
        public bool IsAllowed { get; set; }
    }
}
