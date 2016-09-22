using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EvilApiClient.Core.Domain
{
    /// <summary>
    /// This class represents the response data of customer data upload api
    /// </summary>
    public class EvilUploadCustomerResponse
    {
        [JsonProperty(PropertyName = "customer")]
        public string Customer { get; set; }

        [JsonProperty(PropertyName = "value")]
        public int Value { get; set; }

        [JsonProperty(PropertyName = "added")]
        public bool Added { get; set; }

        [JsonProperty(PropertyName = "hash")]
        public string Hash { get; set; }

        [JsonProperty(PropertyName = "errors")]
        public string[] Errors { get; set; }
    }
}
