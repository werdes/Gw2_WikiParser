using Gw2_WikiParser.Exceptions;
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
            {"Burning|Burn", Action.InflictBurning.ToString() },
            {"Life stealing|Steal Life", Action.LifeSteal.ToString() },
            {"Chance Steal Life", Action.LifeSteal.ToString() },
            {"Chance Life Steal", Action.LifeSteal.ToString() },
            {"Chance Gain Might", Action.GainMight.ToString() },
            {"Chance Might Gain", Action.GainMight.ToString() },
            {"Chance Gain Health", Action.GainHealth.ToString() },
            {"Chance Health Gain", Action.GainHealth.ToString() },
            {"Chance GainHealth", Action.GainHealth.ToString() },
            {"Chance Gain Fury", Action.GainFury.ToString() },
            {"Chance Remove a Condition", Action.ConditionRemove.ToString() },
            {"Chance Inflict Chill", Action.InflictChill.ToString() },
            {"Chance Burn", Action.InflictBurning.ToString() },
            {"Chance Inflict Burning", Action.InflictBurning.ToString() },
            {"Seconds of Quickness", Action.GainQuickness.ToString() },
            {"Gain Swiftness", Action.GainSwiftness.ToString() },
            {"Gain Might", Action.GainMight.ToString() },
            {"Gain Health", Action.GainHealth.ToString() },
            {"Lose a Condition", Action.LoseCondition.ToString() }
        };

        public new static Dictionary<Regex, string> RegexReplacementMatches = new Dictionary<Regex, string>(FoodEffect.RegexReplacementMatches)
        {
            {new Regex(@"(Inflict Chill[A-Za-z0-9 ,]*Burning[A-Za-z0-9 ,]*Poisoned)", RegexOptions.IgnoreCase), Action.InflictChillBurningPoisoned.ToString() }
        };

        public ChanceFoodEffect(string line) : base(line)
        {
            Type = EffectType.Chance;

            if (!ParseEffect(line))
            {
                throw new UnmatchedFoodEffectException(line);
            }
        }

        public enum Action
        {
            LifeSteal,
            ConditionRemove,
            GainMight,
            GainHealth,
            GainFury,
            InflictChill,
            InflictBurning,
            GainQuickness,
            GainSwiftness,
            LoseCondition,
            InflictChillBurningPoisoned
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

            line = FoodEffect.NormalizeLine(line, ChanceFoodEffect.InvalidWords, ChanceFoodEffect.RegexReplacementMatches);

            if (_regexValue.Match(line).Success &&
               _regexActions.Match(line).Success)
            {
                string valueMatch = _regexValue.Match(line).Value;
                string actionMatch = _regexActions.Match(line).Value;

                if (int.TryParse(valueMatch, out value) &&
                   Enum.TryParse(actionMatch, out action))
                {
                    Chance = value;
                    Effect = action;
                    wasSuccessful = true;
                }
            }
            else if (!_regexValue.Match(line).Success &&
                     _regexActions.Match(line).Success)
            {
                if (Enum.TryParse(_regexActions.Match(line).Value, out action))
                {
                    //Action found, but no Value -> probably 100%
                    Chance = 100;
                    Effect = action;
                    wasSuccessful = true;
                }
            }

            return wasSuccessful;
        }


        public static bool MatchLine(string line)
        {
            line = FoodEffect.NormalizeLine(line, ChanceFoodEffect.InvalidWords, ChanceFoodEffect.RegexReplacementMatches);
            return _regexActions.IsMatch(line);
        }
    }
}
