using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2_WikiParser.Model.Output.FoodEffectTask
{
    public class ContinuousHealthFoodEffect : FoodEffect
    {
        public ContinuousHealthFoodEffect()
        {
            Type = EffectType.ContinuousHealth;
        }

        [JsonProperty("health_per_second")]
        public int HealthPerSecond { get; set; }

        public static bool MatchLine(string line)
        {
            return false;
        }
    }
}
