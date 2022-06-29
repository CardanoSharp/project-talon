using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ProjectTalon.Core.Dto;

namespace ProjectTalon.Core.Requests
{
    public class TransactionSubmitRequest
    {
        [JsonPropertyName("include_change_output")]
        public bool IncludeChangeOutput { get; set; }

        [JsonPropertyName("outputs")]
        public List<Output> Outputs { get; set; }

        [JsonPropertyName("metadata")]
        public object Metadata { get; set; }
    }
}
