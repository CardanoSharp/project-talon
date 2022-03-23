using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTalon.Core.Data.Models
{
    public class AppConnect
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string AppId { get; set; }   
        public int ConnectionStatus { get; set; }
        
        public bool HasReviewed { get; set; }
    }
}
