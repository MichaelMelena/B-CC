using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Web.Models
{
    public class BCCSession
    {
        [JsonProperty("overviewModels")]
        public List<OverviewModel> OverviewModels { get; set; }

        [JsonProperty("bankMarginModels")]
        public List<BankMarginModel> BankMarginModels { get; set; }
    }
}
