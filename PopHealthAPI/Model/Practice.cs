using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PopHealthAPI.Model
{
    public class Practice
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("alternate_id")]
        public string AlternateId { get; set; }
        [JsonProperty("facilitator_banner_text")]
        public string FacilitatorBannerText { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("organization")]
        public string Organization { get; set; }
        [JsonProperty("provider_id")]
        public string ProviderId { get; set; }
        [JsonProperty("patient_count")]
        public int? PatientCount { get; set; }
    }
}
