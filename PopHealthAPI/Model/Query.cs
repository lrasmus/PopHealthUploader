using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PopHealthAPI.Model
{
    public class Query
    {
        [JsonProperty("effective_date")]
        public uint EffectiveDate { get; set; }
        [JsonProperty("effective_start_date")]
        public uint EffectiveStartDate { get; set; }
        [JsonProperty("measure_id")]
        public string MeasureId { get; set; }
        [JsonProperty("sub_id")]
        public string SubId { get; set; }
        [JsonProperty("providers")]
        public string[] Providers { get; set; }

        public Query() { }

        public Query(Query query)
        {
            EffectiveDate = query.EffectiveDate;
            EffectiveStartDate = query.EffectiveStartDate;
            MeasureId = query.MeasureId;
            SubId = query.SubId;
            Providers = query.Providers == null ? null : (string[])query.Providers.Clone();
        }
    }
}
