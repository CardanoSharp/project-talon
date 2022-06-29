using CardanoSharp.Wallet.Models.Transactions;
using ProjectTalon.Core.Dto;

namespace ProjectTalon.Core.Common;

public static class TransactionUtility
{
    public static List<Asset> ConvertFromOutputs(List<TransactionOutput> outputs)
    {
        var assets = new List<Asset>();

        foreach (var o in outputs)
        {
            var lovelaces = assets.FirstOrDefault(x => x.AssetName == "lovelace");
            if (lovelaces is null)
            {
                lovelaces = new Asset()
                {
                    AssetName = "lovelace"
                };
                assets.Add(lovelaces);
            }

            lovelaces.Quantity = lovelaces.Quantity + o.Value.Coin;

            //do tokens
        }
        
        return assets;
    }
}