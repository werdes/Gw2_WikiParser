using Gw2_WikiParser.Exceptions;
using Gw2_WikiParser.Extensions;
using Gw2_WikiParser.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Gw2_WikiParser.Model.Output.FoodEffectTask
{
    public class Food
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("effects")]
        public List<FoodEffect> Effects { get; set; }

        [JsonProperty("duration")]
        public int DurationSeconds { get; set; }

        public Food()
        {
            Effects = new List<FoodEffect>();
        }
    }
}
