using Gw2_WikiParser.Exceptions;
using Gw2_WikiParser.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Gw2_WikiParser.Model.Output.FoodEffectTask
{
    public abstract class FoodEffect
    {
        private static Regex _regexAffectedStat = new Regex($"\\b(\\w*({Enum.GetNames(typeof(SpecialCondition)).Join("|")})\\w*)\\b", RegexOptions.IgnoreCase);
        private static Regex _regexTriggers = new Regex($"\\b(\\w*({Enum.GetNames(typeof(Trigger)).Join("|")})\\w*)\\b", RegexOptions.IgnoreCase);

        protected static Dictionary<string, string> InvalidWords = new Dictionary<string, string>()
        {
            {" to ", " " },
            {"Outgoing [[Healing]]", VariableStatFoodEffect.StatType.OutgoingHealing.ToString() },
            {"[[Healing]]", FlatStatFoodEffect.StatType.HealingPower.ToString() },
            {"[[", "" },
            {"]]", "" },
            {"Night:", SpecialCondition.DuringNight.ToString() },
            {"Day:", SpecialCondition.DuringDay.ToString() },
            {"during the Night", SpecialCondition.DuringNight.ToString() },
            {"during the Day", SpecialCondition.DuringDay.ToString() },
            {"during Lunar New Year", SpecialCondition.DuringLunarNewYear.ToString() },
            {"Health Is Below 50%", SpecialCondition.HealthBelow50Percent.ToString() },
            {"Health Below 50%", SpecialCondition.HealthBelow50Percent.ToString() },
            {"Health Is Above 90%", SpecialCondition.HealthAbove90Percent.ToString() },
            {"Health Above 90%", SpecialCondition.HealthAbove90Percent.ToString() },



            {"Critical Hit", Trigger.CriticalHit.ToString() },
            {"Using a Heal Skill", Trigger.HealSkill.ToString() },
            {"Using a Healing Skill", Trigger.HealSkill.ToString() },
            {"Dodge", Trigger.Dodge.ToString() },
            {"Kill effect", Trigger.Kill.ToString() },
            {"on Kill", Trigger.Kill.ToString() },
            {"from Kills", Trigger.Kill.ToString() },
            {"When You Kill a Foe", Trigger.Kill.ToString() },
            {"on Dismount", Trigger.Dismount.ToString() }
        };

        protected static Dictionary<Regex, string> RegexReplacementMatches = new Dictionary<Regex, string>()
        {
            {new Regex(@"<span(.*)<\/span>"), " " }
        };

        public enum EffectType
        {
            Flat,
            Variable,
            Chance,
            ContinuousHealth,
            Static
        }

        public enum SpecialCondition
        {
            None,
            DuringDay,
            DuringNight,
            DuringLunarNewYear,
            HealthBelow50Percent,
            HealthAbove90Percent
        }

        public enum Trigger
        {
            None,
            CriticalHit,
            Dodge,
            Kill,
            HealSkill,
            Evade,
            Dismount,
            Downstate
        }

        [JsonProperty("trigger")]
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(Trigger.None)]
        public Trigger On { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("type")]
        public EffectType Type { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("special_condition")]
        public SpecialCondition Condition { get; set; }

        public FoodEffect()
        {

        }

        public FoodEffect(string line)
        {
            ParseSpecialEffect(line);
            ParseTrigger(line);
        }

        public virtual bool ParseEffect(string line)
        {
            return false;
        }

        /// <summary>
        /// Tries to find a special condition in the effect description
        /// </summary>
        /// <param name="line"></param>
        private void ParseSpecialEffect(string line)
        {
            SpecialCondition specialCondition = SpecialCondition.None;
            line = NormalizeLine(line, InvalidWords, RegexReplacementMatches);

            if (_regexAffectedStat.IsMatch(line))
            {
                if (Enum.TryParse(_regexAffectedStat.Match(line).Value, out specialCondition))
                {
                    Condition = specialCondition;
                }
                else throw new UnmatchedFoodEffectException(line, "SpecialEffect");
            }
        }

        /// <summary>
        /// Tries to find a trigger in the effect description
        /// </summary>
        /// <param name="line"></param>
        private void ParseTrigger(string line)
        {
            Trigger trigger = Trigger.None;
            line = NormalizeLine(line, InvalidWords, RegexReplacementMatches);
            if (_regexTriggers.IsMatch(line))
            {
                if (Enum.TryParse(_regexTriggers.Match(line).Value, out trigger))
                {
                    On = trigger;
                }
                else throw new UnmatchedFoodEffectException(line, "Trigger");
            }
        }

        /// <summary>
        /// Tries to match the effect type
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
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
                return new ChanceFoodEffect(line);
            }
            else if (StaticFoodEffect.MatchLine(line))
            {
                return new StaticFoodEffect(line);
            }
            else throw new UnmatchedFoodEffectException(line, "Effect Match");
        }

        protected static string NormalizeLine(string line, Dictionary<string, string> invalidWords, Dictionary<Regex, string> regexReplacementMatches)
        {
            invalidWords.ForEach((key, value) => line = line.RegexReplace(key, value, RegexOptions.IgnoreCase));
            regexReplacementMatches.ForEach((regex, replacement) => line = regex.Replace(line, replacement));
            return line;
        }
    }

}
