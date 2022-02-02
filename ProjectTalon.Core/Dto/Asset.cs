using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTalon.Core.Dto
{
    public class Asset
    {
        public Asset()
        {

        }

        public Asset(string? policyId, string? assetName, ulong quantity)
        {
            PolicyId = policyId; 
            AssetName = assetName;
            Quantity = quantity;
        }

        public string? PolicyId { get; set; }
        public string? AssetName { get; set; }
        public ulong Quantity { get; set; }
    }
}
