using Gw2_WikiParser.Exceptions;
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
        private static Regex _regexValue = new Regex("[+-]?\\d+(\\.\\d{1,4})? *(?=%)*", RegexOptions.Compiled);
        private static Regex _regexAffectedStat = new Regex($"\\b(\\w*({Enum.GetNames(typeof(StatType)).Join("|")})\\w*)\\b", RegexOptions.IgnoreCase);

        public new static Dictionary<string, string> InvalidWords = new Dictionary<string, string>(FoodEffect.InvalidWords)
        {
            {"Magic Find", StatType.MagicFind.ToString() },
            {"Healing Effectiveness (Outgoing)", StatType.OutgoingHealing.ToString() },
            {"Outgoing Healing", StatType.OutgoingHealing.ToString() },
            {"Outgoing HealingPower", StatType.OutgoingHealing.ToString() },
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
            {"Incoming Stun Duration", StatType.IncomingStunDuration.ToString() },
            {"Endurance Regeneration", StatType.EnduranceRegeneration.ToString() },
            {"Damage While Moving", StatType.DamageWhileMoving.ToString() },
            {"Damage While Downed", StatType.DamageWhileDowned.ToString() },
            {"Downed Health", StatType.DownedHealth.ToString() },
            {"Movement Speed", StatType.MovementSpeed.ToString() },
            {"Incoming Damage", StatType.IncomingDamageReduction.ToString() }
        };

        public new static Dictionary<Regex, string> RegexReplacementMatches = new Dictionary<Regex, string>(FoodEffect.RegexReplacementMatches)
        {

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
            MovementSpeed,
            WxpGain,
            IncomingDamageReduction,
            IncomingConditionDamage,
            IncomingConditionDuration,
            IncomingDamageStunned,
            IncomingStunDuration,
            EnduranceRegeneration,
            DamageWhileMoving,
            DamageWhileDowned,
            DownedHealth,                
        }

        public VariableStatFoodEffect(string line) : base(line)
        {
            Type = EffectType.Variable;

            if (!ParseEffect(line))
            {
                throw new UnmatchedFoodEffectException(line);
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

            line = FoodEffect.NormalizeLine(line, VariableStatFoodEffect.InvalidWords, VariableStatFoodEffect.RegexReplacementMatches);

            if (_regexValue.Match(line).Success &&
               _regexAffectedStat.Match(line).Success)
            {
                string valueMatch = _regexValue.Match(line).Value;
                string statTypeMatch = _regexAffectedStat.Match(line).Value;
                if (int.TryParse(valueMatch, out value) &&
                   Enum.TryParse(statTypeMatch, out statType))
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
            line = FoodEffect.NormalizeLine(line, VariableStatFoodEffect.InvalidWords, VariableStatFoodEffect.RegexReplacementMatches);
            return _regexValue.IsMatch(line) &&
                   _regexAffectedStat.IsMatch(line);
        }
    }
}
