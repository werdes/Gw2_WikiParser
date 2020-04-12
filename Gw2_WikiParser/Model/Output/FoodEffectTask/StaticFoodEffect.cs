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
    public class StaticFoodEffect : FoodEffect
    {
        private static Regex _regexEffect = new Regex($"\\b(\\w*({Enum.GetNames(typeof(StaticEffect)).Join("|")})\\w*)\\b", RegexOptions.IgnoreCase);

        public new static Dictionary<string, string> InvalidWords = new Dictionary<string, string>(FoodEffect.InvalidWords)
        {
            {"May Cause Intermittent Gastric Distress", StaticEffect.CausesIntermittentGastricDistress.ToString() },
        };

        public new static Dictionary<Regex, string> RegexReplacementMatches = new Dictionary<Regex, string>(FoodEffect.RegexReplacementMatches)
        {

        };

        public enum StaticEffect
        {
            CausesIntermittentGastricDistress
        }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("static_effect")]
        public StaticEffect Effect { get; set; }


        public StaticFoodEffect(string line) : base(line)
        {
            Type = EffectType.Static;

            if (!ParseEffect(line))
            {
                throw new UnmatchedFoodEffectException(line, nameof(StaticFoodEffect));
            }
        }


        public override bool ParseEffect(string line)
        {
            int value;
            StaticEffect effect;
            bool wasSuccessful = false;

            line = FoodEffect.NormalizeLine(line, StaticFoodEffect.InvalidWords, StaticFoodEffect.RegexReplacementMatches);

            if (_regexEffect.Match(line).Success &&
                Enum.TryParse(_regexEffect.Match(line).Value, true, out effect))
            {
                Effect = effect;
                wasSuccessful = true;
            }

            return wasSuccessful;
        }


        public static bool MatchLine(string line)
        {
            StaticEffect effect;
            line = FoodEffect.NormalizeLine(line, StaticFoodEffect.InvalidWords, StaticFoodEffect.RegexReplacementMatches);
            return _regexEffect.IsMatch(line) && Enum.TryParse(_regexEffect.Match(line).Value, true, out effect);
        }
    }
}
