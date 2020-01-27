using Gw2_WikiParser.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2_WikiParser.Model.Output.FoodEffectTask
{
    public abstract class FoodEffect
    {
        protected static Dictionary<string, string> InvalidWords = new Dictionary<string, string>()
        {
            {" to ", "" },
            {"[[", "" },
            {"]]", "" }
        };

        public enum EffectType
        {
            Flat,
            Variable,
            Chance,
            ContinuousHealth
        }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("type")]
        public EffectType Type { get; set; }

        public virtual bool ParseEffect(string line)
        {
            return false;
        }

        public static FoodEffect GetEffect(string line)
        {
            if (FlatStatFoodEffect.MatchLine(line))
            {
                return new FlatStatFoodEffect(line);
            }
            else if (VariableStatFoodEffect.MatchLine(line))
            {
                return new VariableStatFoodEffect(line);
            }
            else if (ContinuousHealthFoodEffect.MatchLine(line))
            {
                return new ContinuousHealthFoodEffect();
            }
            else if (ChanceFoodEffect.MatchLine(line))
            {
                return new ChanceFoodEffect();
            }
            else throw new UnmatchedFoodEffectException(line);
        }
    }

}
