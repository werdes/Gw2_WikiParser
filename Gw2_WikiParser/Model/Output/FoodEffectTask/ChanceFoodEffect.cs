using Gw2_WikiParser.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace Gw2_WikiParser.Model.Output.FoodEffectTask
{
    public class ChanceFoodEffect : FoodEffect
    {
        private static Regex _regexValue = new Regex("[+-]?\\d+(\\.\\d{1,4})? *(?=%)", RegexOptions.Compiled);
        private static Regex _regexActions = new Regex($"\\b(\\w*({Enum.GetNames(typeof(Action)).Join("|")})\\w*)\\b", RegexOptions.IgnoreCase);

        public new static Dictionary<string, string> InvalidWords = new Dictionary<string, string>(FoodEffect.InvalidWords)
        {
            {"Chance Steal Life", Action.LifeSteal.ToString() },
            {"Chance Life Steal", Action.LifeSteal.ToString() },
            {"Chance Gain Might", Action.GainMight.ToString() },
            {"Chance Might Gain", Action.GainMight.ToString() },
            {"Chance Gain Fury", Action.GainFury.ToString() },
            {"Chance Remove a Condition", Action.ConditionRemove.ToString() },
            {"Chance Inflict Chill", Action.InflictChill.ToString() },
            {"Chance Burn", Action.Burn.ToString() },
            {"Chance GainHealth", Action.GainHealth.ToString() },
            {"Seconds of Quickness", Action.GainQuickness.ToString() },
            {"to Gain Swiftness", Action.GainSwiftness.ToString() },
            {"Lose a Condition", Action.LoseCondition.ToString() },
        };

        public ChanceFoodEffect(string line) : base(line)
        {
            Type = EffectType.Chance;

            ParseEffect(line);
        }

        public enum Action
        {
            LifeSteal,
            ConditionRemove,
            GainMight,
            GainHealth,
            GainFury,
            InflictChill,
            Burn,
            GainQuickness,
            GainSwiftness,
            LoseCondition
        }


        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("effect")]
        public Action Effect { get; set; }


        [JsonProperty("chance")]
        public double Chance { get; set; }

        public override bool ParseEffect(string line)
        {
            int value;
            Action action;
            bool wasSuccessful = false;

            InvalidWords.ForEach((key, value) => line = line.RegexReplace(key, value, RegexOptions.IgnoreCase));

            if (_regexValue.Match(line).Success &&
               _regexActions.Match(line).Success)
            {
                if (int.TryParse(_regexValue.Match(line).Value, out value) &&
                   Enum.TryParse(_regexActions.Match(line).Value, out action))
                {
                    Chance = value;
                    Effect = action;
                    wasSuccessful = true;
                }
            }

            return wasSuccessful;
        }


        public static bool MatchLine(string line)
        {
            InvalidWords.ForEach((key, value) => line = line.RegexReplace(key, value, RegexOptions.IgnoreCase));
            return _regexActions.IsMatch(line) &&
                   _regexValue.IsMatch(line);
        }
    }
}
