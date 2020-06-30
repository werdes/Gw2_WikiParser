using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2_WikiParser.Model.Input.EventTimerTask
{
    public class EventSequence
    {
        public EventSequence()
        {
            Partial = new List<EventSequenceDetails>();
            Pattern = new List<EventSequenceDetails>();
        }

        [JsonProperty("partial")]
        public List<EventSequenceDetails> Partial { get; set; }

        [JsonProperty("pattern")]
        public List<EventSequenceDetails> Pattern { get; set; }
    }
}
