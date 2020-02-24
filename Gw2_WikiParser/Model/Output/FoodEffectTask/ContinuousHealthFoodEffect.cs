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
            InvalidWords.ForEach((key, value) => line = line.RegexReplace(key, value, RegexOptions.IgnoreCase));
            return _regexDescription.IsMatch(line);
        }
    }
}
