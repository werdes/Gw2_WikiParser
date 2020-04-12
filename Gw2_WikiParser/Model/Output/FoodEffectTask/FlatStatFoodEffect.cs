using Gw2_WikiParser.Exceptions;
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
        private static Regex _regexValue = new Regex("((\\+|\\-)* *([0-9]+ +))", RegexOptions.Compiled);
        private static Regex _regexAffectedStat = new Regex($"\\b(\\w*({Enum.GetNames(typeof(StatType)).Join("|")})\\w*)\\b", RegexOptions.IgnoreCase);

        public new static Dictionary<string, string> InvalidWords = new Dictionary<string, string>(FoodEffect.InvalidWords)
        {
            {"Condition Damage", StatType.ConditionDamage.ToString() },
            {"Healing Power", StatType.HealingPower.ToString() },
            {"All Attributes", StatType.AllAttributes.ToString() }
        };

        public new static Dictionary<Regex, string> RegexReplacementMatches = new Dictionary<Regex, string>(FoodEffect.RegexReplacementMatches)
        {

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

        public FlatStatFoodEffect(string line) : base(line)
        {
            Type = EffectType.Flat;

            if(!ParseEffect(line))
            {
                throw new UnmatchedFoodEffectException(line, nameof(FlatStatFoodEffect));
            }
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
            bool wasSuccessful = false;

            line = FoodEffect.NormalizeLine(line, FlatStatFoodEffect.InvalidWords, FlatStatFoodEffect.RegexReplacementMatches);

            if (_regexValue.Match(line).Success &&
               _regexAffectedStat.Match(line).Success)
            {
                string matchValue = _regexValue.Match(line).Value.Trim();
                if (int.TryParse(matchValue, out value) &&
                   Enum.TryParse(_regexAffectedStat.Match(line).Value, true, out statType))
                {
                    Value = value;
                    AffectedStat = statType;
                    wasSuccessful = true;
                }
            }

            return wasSuccessful;
        }

        public static bool MatchLine(string line)
        {
            line = FoodEffect.NormalizeLine(line, FlatStatFoodEffect.InvalidWords, FlatStatFoodEffect.RegexReplacementMatches);
            return _regexValue.IsMatch(line) &&
                   _regexAffectedStat.IsMatch(line);
        }
    }

}
