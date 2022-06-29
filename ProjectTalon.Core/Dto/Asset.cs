using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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

        [JsonPropertyName("policy_id")]
        public string? PolicyId { get; set; }

        [JsonPropertyName("asset_name")]
        public string? AssetName { get; set; }

        [JsonPropertyName("quantity")]
        public ulong Quantity { get; set; }
    }
}
