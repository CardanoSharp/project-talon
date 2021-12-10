﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTalon.App.Models
{
    public class WalletSummary
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountInUSD { get; set; }
        public int CurrencyId { get; set; }
        public decimal CurrencyConversionRate { get; set; }
    }
}