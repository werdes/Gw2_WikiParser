using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2_WikiParser.Model.Input.EventTimerTask
{
    public class EventGroup
    {
        public EventGroup()
        {
            Segments = new Dictionary<string, EventSegment>();
            Sequences = new EventSequence();
        }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("segments")]
        public Dictionary<string, EventSegment> Segments { get; set; }

        [JsonProperty("sequences")]
        public EventSequence Sequences { get; set; }
    }


}
