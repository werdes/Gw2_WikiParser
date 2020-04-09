using Gw2_WikiParser.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Gw2_WikiParser.Model.Output.FoodEffectTask
{
    public class ContinuousHealthFoodEffect : FoodEffect
    {
        private static Regex _regexDescription = new Regex("(Gain Health Every Second)+", RegexOptions.IgnoreCase);


        public ContinuousHealthFoodEffect() : base()
        {
            Type = EffectType.ContinuousHealth;
        }

        public static bool MatchLine(string line)
        {
            line = FoodEffect.NormalizeLine(line, FoodEffect.InvalidWords, FoodEffect.RegexReplacementMatches);
            return _regexDescription.IsMatch(line);
        }
    }
}
