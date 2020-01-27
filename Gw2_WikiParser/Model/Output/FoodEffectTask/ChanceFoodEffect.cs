using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2_WikiParser.Model.Output.FoodEffectTask
{
    public class ChanceFoodEffect : FoodEffect
    {
        public ChanceFoodEffect()
        {
            Type = EffectType.Chance;
        }

        public enum Action
        {
            LifeSteal,
            ConditionRemove,
            GainMight,
            InflictChill,
            Burn,
            GainQuickness
        }

        public enum Trigger
        {
            CriticalHit,
            HealSkill,
            Dismount
        }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("effect")]
        public Action Effect { get; set; }

        [JsonProperty("on")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Trigger On { get; set; }

        [JsonProperty("chance")]
        public double Chance { get; set; }

        [JsonProperty("condition")]
        public string Condition { get; set; }

        public static bool MatchLine(string line)
        {
            return false;
        }
    }
}
