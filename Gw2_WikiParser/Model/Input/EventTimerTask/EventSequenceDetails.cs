using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2_WikiParser.Model.Input.EventTimerTask
{
    public class EventSequenceDetails
    {
        public EventSequenceDetails()
        {

        }

        [JsonProperty("r")]
        public int R { get; set; }

        [JsonProperty("d")]
        public int Duration { get; set; }

        [JsonIgnore]
        public int Start { get; set; }

        [JsonIgnore]
        public int End { get; set; }
    }
}
