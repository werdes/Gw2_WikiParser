using Gw2_WikiParser.Exceptions;
using Gw2_WikiParser.Model.Output.FoodEffectTask;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Gw2_WikiParser.Test
{
    public class FoodEffectParserTest
    {
        private ITestOutputHelper _output;

        public FoodEffectParserTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestFlatStatEffect()
        {




            //Power,
            FoodEffect foodEffectPower = FoodEffect.GetEffect("+20 [[Power]]");
            Assert.IsType<FlatStatFoodEffect>(foodEffectPower);
            Assert.Equal(FlatStatFoodEffect.StatType.Power, ((FlatStatFoodEffect)foodEffectPower).AffectedStat);
            Assert.Equal(20, ((FlatStatFoodEffect)foodEffectPower).Value);

            //Precision,
            FoodEffect foodEffectPrecision = FoodEffect.GetEffect("+20 [[Precision]]");
            Assert.IsType<FlatStatFoodEffect>(foodEffectPrecision);
            Assert.Equal(FlatStatFoodEffect.StatType.Precision, ((FlatStatFoodEffect)foodEffectPrecision).AffectedStat);
            Assert.Equal(20, ((FlatStatFoodEffect)foodEffectPrecision).Value);

            //Ferocity,
            FoodEffect foodEffectFerocity = FoodEffect.GetEffect("+60 [[Ferocity]]");
            Assert.IsType<FlatStatFoodEffect>(foodEffectFerocity);
            Assert.Equal(FlatStatFoodEffect.StatType.Ferocity, ((FlatStatFoodEffect)foodEffectFerocity).AffectedStat);
            Assert.Equal(60, ((FlatStatFoodEffect)foodEffectFerocity).Value);

            //Toughness,
            FoodEffect foodEffectToughness = FoodEffect.GetEffect("+70 [[toughness]]");
            Assert.IsType<FlatStatFoodEffect>(foodEffectToughness);
            Assert.Equal(FlatStatFoodEffect.StatType.Toughness, ((FlatStatFoodEffect)foodEffectToughness).AffectedStat);
            Assert.Equal(70, ((FlatStatFoodEffect)foodEffectToughness).Value);
            //Vitality,
            FoodEffect foodEffectVitality = FoodEffect.GetEffect("+70 [[Vitality]]");
            Assert.IsType<FlatStatFoodEffect>(foodEffectVitality);
            Assert.Equal(FlatStatFoodEffect.StatType.Vitality, ((FlatStatFoodEffect)foodEffectVitality).AffectedStat);
            Assert.Equal(70, ((FlatStatFoodEffect)foodEffectVitality).Value);
            //Concentration,
            FoodEffect foodEffectConcentration = FoodEffect.GetEffect("+100 [[Concentration]]");
            Assert.IsType<FlatStatFoodEffect>(foodEffectConcentration);
            Assert.Equal(FlatStatFoodEffect.StatType.Concentration, ((FlatStatFoodEffect)foodEffectConcentration).AffectedStat);
            Assert.Equal(100, ((FlatStatFoodEffect)foodEffectConcentration).Value);
            //ConditionDamage,
            FoodEffect foodEffectConditionDamage = FoodEffect.GetEffect("+40 [[Condition Damage]]");
            Assert.IsType<FlatStatFoodEffect>(foodEffectConditionDamage);
            Assert.Equal(FlatStatFoodEffect.StatType.ConditionDamage, ((FlatStatFoodEffect)foodEffectConditionDamage).AffectedStat);
            Assert.Equal(40, ((FlatStatFoodEffect)foodEffectConditionDamage).Value);
            //HealingPower,
            FoodEffect foodEffectHealing = FoodEffect.GetEffect("+20 [[Healing Power]]");
            Assert.IsType<FlatStatFoodEffect>(foodEffectHealing);
            Assert.Equal(FlatStatFoodEffect.StatType.HealingPower, ((FlatStatFoodEffect)foodEffectHealing).AffectedStat);
            Assert.Equal(20, ((FlatStatFoodEffect)foodEffectHealing).Value);

            //Expertise,
            FoodEffect foodEffectExpertise = FoodEffect.GetEffect("+70 [[Expertise]]");
            Assert.IsType<FlatStatFoodEffect>(foodEffectExpertise);
            Assert.Equal(FlatStatFoodEffect.StatType.Expertise, ((FlatStatFoodEffect)foodEffectExpertise).AffectedStat);
            Assert.Equal(70, ((FlatStatFoodEffect)foodEffectExpertise).Value);

            //AllAttributes
            FoodEffect foodEffectAllStats = FoodEffect.GetEffect("+45 to All Attributes");
            Assert.IsType<FlatStatFoodEffect>(foodEffectAllStats);
            Assert.Equal(FlatStatFoodEffect.StatType.AllAttributes, ((FlatStatFoodEffect)foodEffectAllStats).AffectedStat);
            Assert.Equal(45, ((FlatStatFoodEffect)foodEffectAllStats).Value);
        }

        [Fact]
        public void TestVariableStatEffects()
        {
            //MagicFind,
            FoodEffect effectMagicFind = FoodEffect.GetEffect("18% [[Magic Find]]");
            Assert.IsType<VariableStatFoodEffect>(effectMagicFind);
            Assert.Equal(VariableStatFoodEffect.StatType.MagicFind, ((VariableStatFoodEffect)effectMagicFind).AffectedStat);
            Assert.Equal(18, ((VariableStatFoodEffect)effectMagicFind).Value);

            //OutgoingHealing, +10% Outgoing [[Healing]]
            FoodEffect effectOutgoingHealing = FoodEffect.GetEffect("+10% Healing Effectiveness (Outgoing)");
            Assert.IsType<VariableStatFoodEffect>(effectOutgoingHealing);
            Assert.Equal(VariableStatFoodEffect.StatType.OutgoingHealing, ((VariableStatFoodEffect)effectOutgoingHealing).AffectedStat);
            Assert.Equal(10, ((VariableStatFoodEffect)effectOutgoingHealing).Value);

            FoodEffect effectOutgoingHealing2 = FoodEffect.GetEffect("+10% Outgoing Healing");
            Assert.IsType<VariableStatFoodEffect>(effectOutgoingHealing2);
            Assert.Equal(VariableStatFoodEffect.StatType.OutgoingHealing, ((VariableStatFoodEffect)effectOutgoingHealing2).AffectedStat);
            Assert.Equal(10, ((VariableStatFoodEffect)effectOutgoingHealing2).Value);

            FoodEffect effectOutgoingHealing3 = FoodEffect.GetEffect("+10% Outgoing [[Healing]]");
            Assert.IsType<VariableStatFoodEffect>(effectOutgoingHealing3);
            Assert.Equal(VariableStatFoodEffect.StatType.OutgoingHealing, ((VariableStatFoodEffect)effectOutgoingHealing3).AffectedStat);
            Assert.Equal(10, ((VariableStatFoodEffect)effectOutgoingHealing3).Value);

            //Experience,
            FoodEffect effectAllExperience = FoodEffect.GetEffect("+1% All [[Experience]] Gained");
            Assert.IsType<VariableStatFoodEffect>(effectAllExperience);
            Assert.Equal(VariableStatFoodEffect.StatType.Experience, ((VariableStatFoodEffect)effectAllExperience).AffectedStat);
            Assert.Equal(1, ((VariableStatFoodEffect)effectAllExperience).Value);

            FoodEffect effectAllExperience2 = FoodEffect.GetEffect("+10% [[Experience]]");
            Assert.IsType<VariableStatFoodEffect>(effectAllExperience2);
            Assert.Equal(VariableStatFoodEffect.StatType.Experience, ((VariableStatFoodEffect)effectAllExperience2).AffectedStat);
            Assert.Equal(10, ((VariableStatFoodEffect)effectAllExperience2).Value);

            //KillExperience, 
            FoodEffect effectKillExperience = FoodEffect.GetEffect("+15% [[Experience]] from Kills");
            Assert.IsType<VariableStatFoodEffect>(effectKillExperience);
            Assert.Equal(VariableStatFoodEffect.StatType.KillExperience, ((VariableStatFoodEffect)effectKillExperience).AffectedStat);
            Assert.Equal(15, ((VariableStatFoodEffect)effectKillExperience).Value);

            //Karma,
            FoodEffect effectKarma = FoodEffect.GetEffect("+10% [[Karma]]");
            Assert.IsType<VariableStatFoodEffect>(effectKarma);
            Assert.Equal(VariableStatFoodEffect.StatType.Karma, ((VariableStatFoodEffect)effectKarma).AffectedStat);
            Assert.Equal(10, ((VariableStatFoodEffect)effectKarma).Value);

            //PoisonDuration,
            FoodEffect effectPoisonDuration = FoodEffect.GetEffect("+3% [[Poisoned|Poison]] Duration");
            Assert.IsType<VariableStatFoodEffect>(effectPoisonDuration);
            Assert.Equal(VariableStatFoodEffect.StatType.PoisonDuration, ((VariableStatFoodEffect)effectPoisonDuration).AffectedStat);
            Assert.Equal(3, ((VariableStatFoodEffect)effectPoisonDuration).Value);

            //BurningDuration,
            FoodEffect effectBurningDuration = FoodEffect.GetEffect("+3% [[Burning]] Duration");
            Assert.IsType<VariableStatFoodEffect>(effectBurningDuration);
            Assert.Equal(VariableStatFoodEffect.StatType.BurningDuration, ((VariableStatFoodEffect)effectBurningDuration).AffectedStat);
            Assert.Equal(3, ((VariableStatFoodEffect)effectBurningDuration).Value);

            //ChillDuration,
            FoodEffect effectChillDuration = FoodEffect.GetEffect("+3% [[Chill]] Duration");
            Assert.IsType<VariableStatFoodEffect>(effectChillDuration);
            Assert.Equal(VariableStatFoodEffect.StatType.ChillDuration, ((VariableStatFoodEffect)effectChillDuration).AffectedStat);
            Assert.Equal(3, ((VariableStatFoodEffect)effectChillDuration).Value);

            //Gold,
            FoodEffect effectGold = FoodEffect.GetEffect("+20% Gold Find");
            Assert.IsType<VariableStatFoodEffect>(effectGold);
            Assert.Equal(VariableStatFoodEffect.StatType.Gold, ((VariableStatFoodEffect)effectGold).AffectedStat);
            Assert.Equal(20, ((VariableStatFoodEffect)effectGold).Value);

            //MonsterGold,
            FoodEffect effectMonsterGold = FoodEffect.GetEffect("10% [[Coin|Gold]] from Monsters");
            Assert.IsType<VariableStatFoodEffect>(effectMonsterGold);
            Assert.Equal(VariableStatFoodEffect.StatType.MonsterGold, ((VariableStatFoodEffect)effectMonsterGold).AffectedStat);
            Assert.Equal(10, ((VariableStatFoodEffect)effectMonsterGold).Value);

            //WxpGain,
            FoodEffect effectWxpGain = FoodEffect.GetEffect("+10% WXP Gained");
            Assert.IsType<VariableStatFoodEffect>(effectWxpGain);
            Assert.Equal(VariableStatFoodEffect.StatType.WxpGain, ((VariableStatFoodEffect)effectWxpGain).AffectedStat);
            Assert.Equal(10, ((VariableStatFoodEffect)effectWxpGain).Value);

            //IncomingDamage,
            FoodEffect effectIncomingDamage = FoodEffect.GetEffect("-10% Incoming [[Damage Reduction|Damage]]");
            Assert.IsType<VariableStatFoodEffect>(effectIncomingDamage);
            Assert.Equal(VariableStatFoodEffect.StatType.IncomingDamageReduction, ((VariableStatFoodEffect)effectIncomingDamage).AffectedStat);
            Assert.Equal(-10, ((VariableStatFoodEffect)effectIncomingDamage).Value);
            FoodEffect effectIncomingDamage2 = FoodEffect.GetEffect("-10% Incoming Damage");
            Assert.IsType<VariableStatFoodEffect>(effectIncomingDamage2);
            Assert.Equal(VariableStatFoodEffect.StatType.IncomingDamageReduction, ((VariableStatFoodEffect)effectIncomingDamage2).AffectedStat);
            Assert.Equal(-10, ((VariableStatFoodEffect)effectIncomingDamage2).Value);

            //IncomingConditionDamage,
            FoodEffect effectIncomingConditionDamage = FoodEffect.GetEffect("-5% Incoming [[Condition]] Damage");
            Assert.IsType<VariableStatFoodEffect>(effectIncomingConditionDamage);
            Assert.Equal(VariableStatFoodEffect.StatType.IncomingConditionDamage, ((VariableStatFoodEffect)effectIncomingConditionDamage).AffectedStat);
            Assert.Equal(-5, ((VariableStatFoodEffect)effectIncomingConditionDamage).Value);

            //IncomingConditionDuration,
            FoodEffect effectIncomingConditionDuration = FoodEffect.GetEffect("-6% Incoming [[Condition Duration]]");
            Assert.IsType<VariableStatFoodEffect>(effectIncomingConditionDuration);
            Assert.Equal(VariableStatFoodEffect.StatType.IncomingConditionDuration, ((VariableStatFoodEffect)effectIncomingConditionDuration).AffectedStat);
            Assert.Equal(-6, ((VariableStatFoodEffect)effectIncomingConditionDuration).Value);

            //IncomingDamageStunned,
            FoodEffect effectIncomingDamageStunned = FoodEffect.GetEffect("-14% Incoming Damage while [[Stun]]ned, [[Knockdown|Knocked Down]], or [[Knockback|Knocked Back]]");
            Assert.IsType<VariableStatFoodEffect>(effectIncomingDamageStunned);
            Assert.Equal(VariableStatFoodEffect.StatType.IncomingDamageStunned, ((VariableStatFoodEffect)effectIncomingDamageStunned).AffectedStat);
            Assert.Equal(-14, ((VariableStatFoodEffect)effectIncomingDamageStunned).Value);

            //IncomingStunDuration,
            FoodEffect effectIncomingStunDuration = FoodEffect.GetEffect("-40% Incoming [[Stun]] Duration");
            Assert.IsType<VariableStatFoodEffect>(effectIncomingStunDuration);
            Assert.Equal(VariableStatFoodEffect.StatType.IncomingStunDuration, ((VariableStatFoodEffect)effectIncomingStunDuration).AffectedStat);
            Assert.Equal(-40, ((VariableStatFoodEffect)effectIncomingStunDuration).Value);

            //EnduranceRegeneration,
            FoodEffect effectEnduranceRegeneration = FoodEffect.GetEffect("+30% to [[Endurance]] Regeneration");
            Assert.IsType<VariableStatFoodEffect>(effectEnduranceRegeneration);
            Assert.Equal(VariableStatFoodEffect.StatType.EnduranceRegeneration, ((VariableStatFoodEffect)effectEnduranceRegeneration).AffectedStat);
            Assert.Equal(30, ((VariableStatFoodEffect)effectEnduranceRegeneration).Value);

            //DamageWhileMoving,
            FoodEffect effectDamageWhileMoving = FoodEffect.GetEffect("+4% Damage While Moving");
            Assert.IsType<VariableStatFoodEffect>(effectDamageWhileMoving);
            Assert.Equal(VariableStatFoodEffect.StatType.DamageWhileMoving, ((VariableStatFoodEffect)effectDamageWhileMoving).AffectedStat);
            Assert.Equal(4, ((VariableStatFoodEffect)effectDamageWhileMoving).Value);

            //DownedHealth,
            FoodEffect effectDownedHealth = FoodEffect.GetEffect("+80% [[Downed]] [[Health]]");
            Assert.IsType<VariableStatFoodEffect>(effectDownedHealth);
            Assert.Equal(VariableStatFoodEffect.StatType.DownedHealth, ((VariableStatFoodEffect)effectDownedHealth).AffectedStat);
            Assert.Equal(80, ((VariableStatFoodEffect)effectDownedHealth).Value);

            //DamageWhileDowned,
            FoodEffect effectDamageWhileDowned = FoodEffect.GetEffect("+14% Damage While Downed");
            Assert.IsType<VariableStatFoodEffect>(effectDamageWhileDowned);
            Assert.Equal(VariableStatFoodEffect.StatType.DamageWhileDowned, ((VariableStatFoodEffect)effectDamageWhileDowned).AffectedStat);
            Assert.Equal(14, ((VariableStatFoodEffect)effectDamageWhileDowned).Value);
        }

        [Fact]
        public void TestContinuousHealthEffect()
        {
            FoodEffect effectIncomingStunDuration = FoodEffect.GetEffect("Gain [[Health]] Every Second");
            Assert.IsType<ContinuousHealthFoodEffect>(effectIncomingStunDuration);
        }

        [Fact]
        public void TestSpecialConditions()
        {
            FoodEffect effectHealthBelow50 = FoodEffect.GetEffect("+160 [[Power]] while Health Is Below 50%");
            Assert.IsType<FlatStatFoodEffect>(effectHealthBelow50);
            Assert.Equal(FlatStatFoodEffect.StatType.Power, ((FlatStatFoodEffect)effectHealthBelow50).AffectedStat);
            Assert.Equal(160, ((FlatStatFoodEffect)effectHealthBelow50).Value);
            Assert.Equal(FoodEffect.SpecialCondition.HealthBelow50Percent, effectHealthBelow50.Condition);

            FoodEffect effectLunarNewYear = FoodEffect.GetEffect("+25% [[Magic Find]] during [[Lunar New Year]]");
            Assert.IsType<VariableStatFoodEffect>(effectLunarNewYear);
            Assert.Equal(VariableStatFoodEffect.StatType.MagicFind, ((VariableStatFoodEffect)effectLunarNewYear).AffectedStat);
            Assert.Equal(25, ((VariableStatFoodEffect)effectLunarNewYear).Value);
            Assert.Equal(FoodEffect.SpecialCondition.DuringLunarNewYear, effectLunarNewYear.Condition);

        }

        [Fact]
        public void TestChanceEffects()
        {
            FoodEffect effectDay1 = FoodEffect.GetEffect("Day: 8% Chance to Burn on Critical Hit");
            Assert.IsType<ChanceFoodEffect>(effectDay1);
            Assert.Equal(ChanceFoodEffect.Action.InflictBurning, ((ChanceFoodEffect)effectDay1).Effect);
            Assert.Equal(8, ((ChanceFoodEffect)effectDay1).Chance);
            Assert.Equal(FoodEffect.SpecialCondition.DuringDay, effectDay1.Condition);
            Assert.Equal(FoodEffect.Trigger.CriticalHit, effectDay1.On);

            FoodEffect effectStealLifeOnCrit = FoodEffect.GetEffect("40% Chance to [[Life steal|Steal Life]] on [[critical hit|Critical Hit]]");
            Assert.IsType<ChanceFoodEffect>(effectStealLifeOnCrit);
            Assert.Equal(ChanceFoodEffect.Action.LifeSteal, ((ChanceFoodEffect)effectStealLifeOnCrit).Effect);
            Assert.Equal(40, ((ChanceFoodEffect)effectStealLifeOnCrit).Chance);
            Assert.Equal(FoodEffect.Trigger.CriticalHit, effectStealLifeOnCrit.On);
        }

        [Fact]
        public void TestStaticEffects()
        {
            FoodEffect effectCausesGastricDistress = FoodEffect.GetEffect("May Cause Intermittent Gastric Distress");
            Assert.IsType<StaticFoodEffect>(effectCausesGastricDistress);
            Assert.Equal(StaticFoodEffect.StaticEffect.CausesIntermittentGastricDistress, ((StaticFoodEffect)effectCausesGastricDistress).Effect);
            Assert.Equal(FoodEffect.SpecialCondition.None, effectCausesGastricDistress.Condition);
            Assert.Equal(FoodEffect.Trigger.None, effectCausesGastricDistress.On);
        }

        [Fact]
        public void TestList()
        {
            string[] lines = File.ReadAllLines("Resources/lines.txt", Encoding.UTF8);
            bool errors = false;

            foreach (string line in lines)
            {
                try
                {
                    FoodEffect effect = FoodEffect.GetEffect(line);
                }
                catch(UnmatchedFoodEffectException ex)
                {
                    _output.WriteLine(ex.Line);
                    errors = true;
                }
            }

            Assert.False(errors);
        }
    }
}
