using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WojciechKaszycki
{
    public class Country
    {
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("alpha2Code")]
        public string Alpha2Code { get; set; }
        [JsonProperty("flag")]
        public string Flag { get; set; }

    }
}
