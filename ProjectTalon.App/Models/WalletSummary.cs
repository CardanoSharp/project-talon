namespace ProjectTalon.App.Models
{
    public class WalletSummary
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountInUSD { get; set; }
        public int CurrencyId { get; set; }
        public decimal CurrencyConversionRate { get; set; }
        public string SingleAddress { get; set; }
    }
}
