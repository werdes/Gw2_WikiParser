using Gw2_WikiParser.Model.Output.FoodEffectTask;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Gw2_WikiParser.Test
{
    public class FoodEffectParserTest
    {
        [Fact]
        public void TestFlatStatEffect()
        {
            FoodEffect foodEffectHealing = FoodEffect.GetEffect("+20 [[Healing Power]]");
            Assert.IsType<FlatStatFoodEffect>(foodEffectHealing);
            Assert.Equal(FlatStatFoodEffect.StatType.HealingPower, ((FlatStatFoodEffect)foodEffectHealing).AffectedStat);
            Assert.Equal(20, ((FlatStatFoodEffect)foodEffectHealing).Value);

            FoodEffect foodEffectPower = FoodEffect.GetEffect("+20 [[Power]]");
            Assert.IsType<FlatStatFoodEffect>(foodEffectPower);
            Assert.Equal(FlatStatFoodEffect.StatType.Power, ((FlatStatFoodEffect)foodEffectPower).AffectedStat);
            Assert.Equal(20, ((FlatStatFoodEffect)foodEffectPower).Value);

            FoodEffect foodEffectPrecision = FoodEffect.GetEffect("+20 [[Precision]]");
            Assert.IsType<FlatStatFoodEffect>(foodEffectPrecision);
            Assert.Equal(FlatStatFoodEffect.StatType.Precision, ((FlatStatFoodEffect)foodEffectPrecision).AffectedStat);
            Assert.Equal(20, ((FlatStatFoodEffect)foodEffectPrecision).Value);

            FoodEffect foodEffectFerocity = FoodEffect.GetEffect("+60 [[Ferocity]]");
            Assert.IsType<FlatStatFoodEffect>(foodEffectFerocity);
            Assert.Equal(FlatStatFoodEffect.StatType.Ferocity, ((FlatStatFoodEffect)foodEffectFerocity).AffectedStat);
            Assert.Equal(60, ((FlatStatFoodEffect)foodEffectFerocity).Value);

            //Power,
            //Precision,
            //Ferocity,
            //Toughness,
            //Vitality,
            //Concentration,
            //ConditionDamage,
            //HealingPower,
            //Expertise,
            //AllAttributes


        }

        [Fact]
        public void TestVariableStatEffects()
        {
            //MagicFind,
            FoodEffect effectMagicFind = FoodEffect.GetEffect("18% [[Magic Find]]");
            Assert.IsType<VariableStatFoodEffect>(effectMagicFind);
            Assert.Equal(VariableStatFoodEffect.StatType.MagicFind, ((VariableStatFoodEffect)effectMagicFind).AffectedStat);
            Assert.Equal(18, ((VariableStatFoodEffect)effectMagicFind).Value);

            //OutgoingHealing,
            FoodEffect effectOutgoingHealing = FoodEffect.GetEffect("+10% Healing Effectiveness (Outgoing)");
            Assert.IsType<VariableStatFoodEffect>(effectOutgoingHealing);
            Assert.Equal(VariableStatFoodEffect.StatType.OutgoingHealing, ((VariableStatFoodEffect)effectOutgoingHealing).AffectedStat);
            Assert.Equal(10, ((VariableStatFoodEffect)effectOutgoingHealing).Value);

            FoodEffect effectOutgoingHealing2 = FoodEffect.GetEffect("+10% Outgoing Healing");
            Assert.IsType<VariableStatFoodEffect>(effectOutgoingHealing2);
            Assert.Equal(VariableStatFoodEffect.StatType.OutgoingHealing, ((VariableStatFoodEffect)effectOutgoingHealing2).AffectedStat);
            Assert.Equal(10, ((VariableStatFoodEffect)effectOutgoingHealing2).Value);

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
        }
    }
}
