using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProjectTalon.Core.Requests
{
    public class TransactionSubmitRequest
    {
        [JsonPropertyName("include_change_output")]
        public bool IncludeChangeOutput { get; set; }

        [JsonPropertyName("outputs")]
        public List<TransactionSubmitOutputRequest> Outputs { get; set; }

        [JsonPropertyName("metadata")]
        public object Metadata { get; set; }
    }

    public class TransactionSubmitOutputRequest
    {
        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("assets")]
        public List<TransactionSubmitTokenRequest> Assets { get; set; }
    }

    public class TransactionSubmitTokenRequest
    {
        [JsonPropertyName("policy_id")]
        public string? PolicyId { get; set; }

        [JsonPropertyName("asset_name")]
        public string? AssetName { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }
}
