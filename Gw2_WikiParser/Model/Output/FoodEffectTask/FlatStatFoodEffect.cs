using Gw2_WikiParser.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Gw2_WikiParser.Model.Output.FoodEffectTask
{

    public class FlatStatFoodEffect : FoodEffect
    {
        private static Regex _regexValue = new Regex("((\\+|\\-) *[0-9])\\w", RegexOptions.Compiled);
        private static Regex _regexAffectedStat = new Regex($"\\b(\\w*({Enum.GetNames(typeof(StatType)).Join("|")})\\w*)\\b", RegexOptions.IgnoreCase);

        public new static Dictionary<string, string> InvalidWords = new Dictionary<string, string>(FoodEffect.InvalidWords)
        {
            {"Condition Damage", StatType.ConditionDamage.ToString() },
            {"Healing Power", StatType.HealingPower.ToString() },
            {"All Attributes", StatType.AllAttributes.ToString() }
        };

        public enum StatType
        {
            Power,
            Precision,
            Ferocity,
            Toughness,
            Vitality,
            Concentration,
            ConditionDamage,
            HealingPower,
            Expertise,
            AllAttributes
        }

        public FlatStatFoodEffect(string line)
        {
            Type = EffectType.Flat;

            ParseEffect(line);
        }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("affected_stat")]
        public StatType AffectedStat { get; set; }

        [JsonProperty("value")]
        public int Value { get; set; }

        public override bool ParseEffect(string line)
        {
            int value;
            StatType statType;

            InvalidWords.ForEach((key, value) => line = line.RegexReplace(key, value, RegexOptions.IgnoreCase));

            if (_regexValue.Match(line).Success &&
               _regexAffectedStat.Match(line).Success)
            {
                if (int.TryParse(_regexValue.Match(line).Value, out value) &&
                   Enum.TryParse(_regexAffectedStat.Match(line).Value, out statType))
                {
                    Value = value;
                    AffectedStat = statType;
                }
            }

            return base.ParseEffect(line);
        }

        public static bool MatchLine(string line)
        {
            InvalidWords.ForEach((key, value) => line = line.RegexReplace(key, value, RegexOptions.IgnoreCase));
            return _regexValue.IsMatch(line) &&
                   _regexAffectedStat.IsMatch(line);
        }
    }

}
