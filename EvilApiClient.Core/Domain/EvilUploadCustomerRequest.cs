using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EvilApiClient.Core.Domain
{
    /// <summary>
    /// This class represents parameters of upload customer data api
    /// </summary>
    public class EvilUploadCustomerRequest
    {
        [JsonProperty(PropertyName = "property")]
        public string Property { get; set; }

        [JsonProperty(PropertyName = "customer")]
        public string Customer { get; set; }

        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; }

        [JsonProperty(PropertyName = "value")]
        public int Value { get; set; }

        [JsonProperty(PropertyName = "file")]
        public string File { get; set; }
    }
}
