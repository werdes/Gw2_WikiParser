using Gw2_WikiParser.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Gw2_WikiParser.Model.Output.FoodEffectTask
{

    public class VariableStatFoodEffect : FoodEffect
    {
        private static Regex _regexValue = new Regex("[+-]?\\d+(\\.\\d{1,4})? *(?=%)", RegexOptions.Compiled);
        private static Regex _regexAffectedStat = new Regex($"\\b(\\w*({Enum.GetNames(typeof(StatType)).Join("|")})\\w*)\\b", RegexOptions.IgnoreCase);

        public new static Dictionary<string, string> InvalidWords = new Dictionary<string, string>(FoodEffect.InvalidWords)
        {
            {"Magic Find", StatType.MagicFind.ToString() },
            {"Healing Effectiveness (Outgoing)", StatType.OutgoingHealing.ToString() },
            {"Outgoing Healing", StatType.OutgoingHealing.ToString() },
            {"Experience from Kills", StatType.KillExperience.ToString() },
            {"All Experience Gained", StatType.Experience.ToString() },
            {"Poison Duration", StatType.PoisonDuration.ToString() },
            {"Burning Duration", StatType.BurningDuration.ToString() },
            {"Chill Duration", StatType.ChillDuration.ToString() },
            {"Gold Find", StatType.Gold.ToString() },
            {"Gold from Monsters", StatType.MonsterGold.ToString() },
            {"WXP Gained", StatType.WxpGain.ToString() },
            {"Incoming Damage While Stunned,", StatType.IncomingDamageStunned.ToString() },
            {"Incoming Damage Reduction", StatType.IncomingDamageReduction.ToString() },
            {"Incoming Condition Damage", StatType.IncomingConditionDamage.ToString() },
            {"Incoming Condition Duration", StatType.IncomingConditionDuration.ToString() },
            {"Incoming Stun Duration", StatType.IncomingStunDuration.ToString() }
        };

        public enum StatType
        {
            MagicFind,
            OutgoingHealing,
            Experience,
            KillExperience,
            Karma,
            PoisonDuration,
            BurningDuration,
            ChillDuration,
            Gold,
            MonsterGold,
            WxpGain,
            IncomingDamageReduction,
            IncomingConditionDamage,
            IncomingConditionDuration,
            IncomingDamageStunned,
            IncomingStunDuration,
        }

        public VariableStatFoodEffect(string line)
        {
            Type = EffectType.Variable;
            ParseEffect(line);
        }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("affected_stat")]
        public StatType AffectedStat { get; set; }

        [JsonProperty("value")]
        public int Value { get; set; }

        [JsonProperty("condition")]
        public string Condition { get; set; }


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
