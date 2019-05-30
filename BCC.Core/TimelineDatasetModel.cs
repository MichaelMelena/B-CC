using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BCC.Core
{
    public class TimelineDatasetModel
    {
        [JsonProperty("labels")]
        public HashSet<DateTime> Labels { get; set; }

        [JsonProperty("dataset")]
        public Dictionary<string, Dictionary<string, double>> Dataset { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("isBuy")]
        public bool IsBuy { get; set; }

        [JsonProperty("start")]
        public DateTime Start { get; set; }

        [JsonProperty("end")]
        public DateTime End { get; set; }
    }
}
