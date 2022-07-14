using System.Collections.Generic;
using ProjectTalon.Core.Data.Models;

namespace ProjectTalon.UI.Models;

public class WalletSummary
{
    public Wallet Wallet { get; set; }
    public List<WalletKey> WalletKeys { get; set; }
}