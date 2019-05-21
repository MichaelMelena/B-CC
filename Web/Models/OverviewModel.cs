using Newtonsoft.Json;

namespace Web.Models
{
    public class OverviewModel
    {
        [JsonProperty("interval")]
        public int Interval { get; set; }

        [JsonProperty("isBuy")]
        public bool IsBuy { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

}
