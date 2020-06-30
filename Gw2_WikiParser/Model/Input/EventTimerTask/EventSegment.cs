using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2_WikiParser.Model.Input.EventTimerTask
{
    public class EventSegment
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("chatlink")]
        public string ChatLink { get; set; }       
    }
}
