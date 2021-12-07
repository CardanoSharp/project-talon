using System;

namespace ProjectTalon.App.Models
{
    public class TransactionSummary
    {
        public DateTime Date { get; set; }
        public int TransactionStatus { get; set; }
        public decimal Amount { get; set; }
        public string TokenName { get; set; }
    }
}
