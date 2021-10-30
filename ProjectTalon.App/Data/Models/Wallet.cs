using SQLite;

namespace ProjectTalon.App.Data.Models
{
    public class Wallet
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public int WalletType { get; set; }
    }
}
