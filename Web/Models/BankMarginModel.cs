using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Web.Models
{
    public class BankMarginModel
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("bankName")]
        public string BankName { get; set; }

        [JsonProperty("start")]
        public DateTime Start { get; set; }

        [JsonProperty("end")]
        public DateTime End { get; set; }

        [JsonProperty("isBuy")]
        public bool IsBuy { get; set; }
    }
}
