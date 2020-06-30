using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2_WikiParser.Model.Output.EventTimerTask
{
    public class Event
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("offset")]
        public TimeSpan Offset { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("waypoint")]
        public string WayPoint { get; set; }

        [JsonProperty("wiki_url")]
        public string WikiUrl { get; set; }

        [JsonProperty("duration")]
        public TimeSpan Duration { get; set; }
    }
}
