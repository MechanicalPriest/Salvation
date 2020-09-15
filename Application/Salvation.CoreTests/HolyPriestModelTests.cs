using Newtonsoft.Json;
using Salvation.Core;
using Salvation.Core.Models.HolyPriest;
using Salvation.Core.Profile;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Salvation.Core.Models.Common;
using Xunit;

namespace Salvation.CoreTests
{
    public class HolyPriestModelTests
    {
        private static HolyPriestModel Model;

        public HolyPriestModelTests()
        {
            var data = GetTestConstantsJson();// File.ReadAllText(@"constants.json");

            var globalConstants = ConstantsManager.ParseConstants(data);

            var basicProfile = DefaultProfiles.GetDefaultProfile(Core.Models.Spec.HolyPriest);

            Model = new HolyPriestModel(globalConstants, basicProfile);
        }

        [Fact]
        public void HolyPriestModelGivesResults()
        {
            var data = GetTestConstantsJson();// File.ReadAllText(@"constants.json");

            var globalConstants = ConstantsManager.ParseConstants(data);

            var basicProfile = DefaultProfiles.GetDefaultProfile(Core.Models.Spec.HolyPriest);

            var hpriest = new HolyPriestModel(globalConstants, basicProfile);

            var spellsRaw = JsonConvert.SerializeObject(hpriest.Spells, Formatting.Indented);

            Console.WriteLine(spellsRaw);
        }

        [Theory]
        [InlineData(typeof(FlashHeal), "2368.7775761650000000000000001", "2.8507733333333333333333333332", "42.933333333333333333333333331", "441.60781955647499999999999995", "1645.3529044042090000000000001", "0", "1.3975155279503105590062111802", "1.3975155279503105590062111802", "0", "1800.000", 1, true)]

        public void HolyPriestSpellIsCorrect(Type spellType,
            string averageRawDirectHeal,
            string castsPerMinute,
            string maximumCastsPerMinute,
            string averageRawMasteryHeal,
            string averageTotalHeal,
            string averageDamage,
            string hastedCastTime,
            string hastedGcd,
            string hastedCooldown,
            string actualManaCost,
            int additionalCastsCount,
            bool hasEcho)
        {
            var spell = new SpellTestHarness(spellType);
            Assert.Equal(Convert.ToDecimal(averageRawDirectHeal), spell.ExposeDecimalMethod("calcAverageRawDirectHeal"));
            Assert.Equal(Convert.ToDecimal(castsPerMinute), spell.ExposeDecimalMethod("calcCastsPerMinute"));
            Assert.Equal(Convert.ToDecimal(maximumCastsPerMinute), spell.ExposeDecimalMethod("calcMaximumCastsPerMinute"));
            Assert.Equal(Convert.ToDecimal(averageRawMasteryHeal), spell.ExposeDecimalMethod("calcAverageRawMasteryHeal"));
            Assert.Equal(Convert.ToDecimal(averageRawMasteryHeal), spell.Spell.AverageRawMasteryHeal);
            Assert.Equal(Convert.ToDecimal(averageTotalHeal), spell.ExposeDecimalMethod("calcAverageTotalHeal"));
            Assert.Equal(Convert.ToDecimal(averageDamage), spell.ExposeDecimalMethod("calcAverageDamage"));
            Assert.Equal(Convert.ToDecimal(hastedCastTime), spell.ExposeDecimalMethod("getHastedCastTime"));
            Assert.Equal(Convert.ToDecimal(hastedGcd), spell.ExposeDecimalMethod("getHastedGcd"));
            Assert.Equal(Convert.ToDecimal(hastedCooldown), spell.ExposeDecimalMethod("getHastedCooldown"));
            Assert.Equal(Convert.ToDecimal(actualManaCost), spell.ExposeDecimalMethod("getActualManaCost"));

            var castAverageSpell = spell.Spell.CastAverageSpell();
            Assert.Equal(Convert.ToDecimal(averageRawDirectHeal), castAverageSpell.RawHealing);
            Assert.Equal(Convert.ToDecimal(castsPerMinute), castAverageSpell.CastsPerMinute);
            Assert.Equal(Convert.ToDecimal(maximumCastsPerMinute), castAverageSpell.MaximumCastsPerMinute);
            Assert.Equal(Convert.ToDecimal(averageTotalHeal), castAverageSpell.Healing);
            Assert.Equal(Convert.ToDecimal(averageDamage), castAverageSpell.Damage);
            Assert.Equal(Convert.ToDecimal(hastedCastTime), castAverageSpell.CastTime);
            Assert.Equal(Convert.ToDecimal(hastedGcd), castAverageSpell.Gcd);
            Assert.Equal(Convert.ToDecimal(hastedCooldown), castAverageSpell.Cooldown);
            Assert.Equal(Convert.ToDecimal(actualManaCost), castAverageSpell.ManaCost);

            Assert.Equal(additionalCastsCount, castAverageSpell.AdditionalCasts.Count);

            var echo = castAverageSpell.AdditionalCasts.FirstOrDefault(ac => ac.SpellId == (int) HolyPriestModel.SpellIds.EchoOfLight);

            Assert.Equal(hasEcho, echo != null);

            if (hasEcho)
            {
                Assert.Equal(Convert.ToDecimal(averageRawMasteryHeal), echo.RawHealing);
            }
        }

        class SpellTestHarness
        {
            public BaseHolyPriestHealingSpell Spell;
            private Type TypeOfT;
            public SpellTestHarness(Type T)
            {
                TypeOfT = T;
                var name = T.Name;
                HolyPriestModel.SpellIds spellId = (HolyPriestModel.SpellIds) Enum.Parse(typeof(HolyPriestModel.SpellIds), name, true);
                object[] args = { Model, Model.GetSpecSpellDataById((int)spellId) };
                Spell = (BaseHolyPriestHealingSpell) Activator.CreateInstance(TypeOfT!, args);
            }

            public decimal ExposeDecimalMethod(string method)
            {
                MethodInfo methodInfo = TypeOfT.GetMethod(method,
                    BindingFlags.NonPublic | BindingFlags.Instance);
                object[] parameters = Array.Empty<object>();
                return (decimal) methodInfo.Invoke(Spell, parameters);
            }
        }

        private string GetTestConstantsJson()
        {
            string json = @"{
  ""GameVersion"": ""9.0.0.1"",
  ""Specs"": [
    {
      ""Class"": ""Priest"",
      ""Spec"": ""Holy"",
      ""SpecId"": 257,

      ""CritBase"": 0.05,
      ""HasteBase"": 0.0,
      ""VersBase"": 0.0,
      ""MasteryBase"": 0.1,
      ""IntBase"": 450.0,
      ""StamBase"": 416.0,
      ""ManaBase"": 50000,

      // https://github.com/simulationcraft/simc/blob/shadowlands/engine/dbc/generated/sc_scale_data.inc
      ""CritCost"": 35,
      ""HasteCost"": 33,
      ""VersCost"": 40,
      ""MasteryCost"": 28,
      ""LeechCost"": 21,
      ""speed_cost"": 10,
      ""avoidance_cost"": 14,
      ""stam_cost"": 20,

      ""Modifiers"": [
        {
          ""Name"": ""BeneRenewChance"",
          ""Value"": 0.25
        },
        {
          // TODO: look into this once the double cast bug is fixed.
          ""Name"": ""HolyNovaDoubleCastFrequency"",
          ""Value"": 0.25
        },
        {
          ""Name"": ""DivineHymnBonusHealing"",
          ""Value"": 0.1
        },
        {
          ""Name"": ""PrayerOfMendingBounces"",
          ""Value"": 4.0
        },
        {
          // Stored as a decimal. 1.4 is 40% more healing.
          ""Name"": ""HolyPriestAuraHealingMultiplier"",
          ""Value"": 1.0
        },
        {
          // Stored as a decimal. 1.4 is 40% more damage.
          ""Name"": ""HolyPriestAuraDamageMultiplier"",
          ""Value"": 1.25
        },
        {
          ""Name"": ""HolyWordsBaseCDR"",
          ""Value"": 6.0
        },
        {
          ""Name"": ""SalvationHolyWordCDR"",
          ""Value"": 30
        },
        {
          ""Name"": ""BoonStacksPerANTarget"",
          ""Value"": 1
        },
        {
          ""Name"": ""BoonStacksPerABTarget"",
          ""Value"": 5
        }
      ],

      // Spell List
      // https://github.com/simulationcraft/simc/blob/shadowlands/engine/dbc/generated/active_spells.inc#L218
      ""Spells"": [
        {
          ""Id"": 2060,
          ""Name"": ""Heal"",
          ""ManaCost"": 0.024,
          ""Range"": 40.0,
          ""NumberOfHealingTargets"": 1.0,
          ""NumberOfDamageTargets"": 0.0,
          ""BaseCastTime"": 2.5,
          ""IsCastTimeHasted"": true,
          ""BaseCooldown"": 0.0,
          ""IsCooldownHasted"": false,
          ""Gcd"": 1.5,
          ""Coeff1"": 2.95,
          ""IsMasteryTriggered"": true
        },
        {
          ""Id"": 2061,
          ""Name"": ""Flash Heal"",
          ""ManaCost"": 0.036,
          ""Range"": 40.0,
          ""NumberOfHealingTargets"": 1.0,
          ""NumberOfDamageTargets"": 0.0,
          ""BaseCastTime"": 1.5,
          ""IsCastTimeHasted"": true,
          ""BaseCooldown"": 0.0,
          ""IsCooldownHasted"": false,
          ""Gcd"": 1.5,
          ""Coeff1"": 2.03,
          ""IsMasteryTriggered"": true
        },
        {
          ""Id"": 596,
          ""Name"": ""Prayer of Healing"",
          ""ManaCost"": 0.05,
          ""Range"": 40.0,
          ""NumberOfHealingTargets"": 1.0, // bounces are set using the modifier PrayerOfMendingBounces
          ""NumberOfDamageTargets"": 0.0,
          ""BaseCastTime"": 2.0,
          ""IsCastTimeHasted"": true,
          ""BaseCooldown"": 0.0,
          ""IsCooldownHasted"": false,
          ""Gcd"": 1.5,
          ""Coeff1"": 0.875,
          ""IsMasteryTriggered"": true
        },
        {
          ""Id"": 132157,
          ""Name"": ""Holy Nova"",
          ""ManaCost"": 0.016,
          ""Range"": 12.0,
          ""NumberOfHealingTargets"": 20.0,
          ""NumberOfDamageTargets"": 1.0,
          ""BaseCastTime"": 0.0,
          ""IsCastTimeHasted"": true,
          ""BaseCooldown"": 0.0,
          ""IsCooldownHasted"": false,
          ""Gcd"": 1.5,
          ""Coeff1"": 0.14,
          ""IsMasteryTriggered"": true
        },
        {
          ""Id"": 204883,
          ""Name"": ""Circle of Healing"",
          ""ManaCost"": 0.033,
          // The heal range off its target is only 30.0y
          ""Range"": 40.0,
          ""NumberOfHealingTargets"": 5.0,
          ""NumberOfDamageTargets"": 0.0,
          ""BaseCastTime"": 0.0,
          ""IsCastTimeHasted"": true,
          ""BaseCooldown"": 15.0,
          ""IsCooldownHasted"": true,
          ""Gcd"": 1.5,
          ""Coeff1"": 1.05,
          ""IsMasteryTriggered"": true
        },
        {
          ""Id"": 139,
          ""Name"": ""Renew"",
          ""ManaCost"": 0.018,
          ""Range"": 40.0,
          ""NumberOfHealingTargets"": 1.0,
          ""NumberOfDamageTargets"": 0.0,
          ""BaseCastTime"": 0.0,
          ""IsCastTimeHasted"": true,
          ""BaseCooldown"": 0.0,
          ""IsCooldownHasted"": false,
          ""Gcd"": 1.5,
          ""Coeff1"": 0.32,
          ""IsMasteryTriggered"": false
        },
        {
          ""Id"": 17,
          ""Name"": ""Power Word: Shield"",
          ""ManaCost"": 0.031,
          ""Range"": 40.0,
          ""NumberOfHealingTargets"": 1.0,
          ""NumberOfDamageTargets"": 0.0,
          ""BaseCastTime"": 0.0,
          ""IsCastTimeHasted"": true,
          ""BaseCooldown"": 0.0,
          ""IsCooldownHasted"": false,
          ""Gcd"": 1.5,
          ""Coeff1"": 1.0,
          ""IsMasteryTriggered"": false
        },
        {
          ""Id"": 64843,
          ""Name"": ""Divine Hymn"",
          ""ManaCost"": 0.044,
          ""Range"": 40.0,
          ""NumberOfHealingTargets"": 20.0,
          ""NumberOfDamageTargets"": 0.0,
          ""BaseCastTime"": 8.0,
          ""IsCastTimeHasted"": true,
          ""BaseCooldown"": 180.0,
          ""IsCooldownHasted"": false,
          ""Gcd"": 1.5,
          ""Coeff1"": 0.6, // Comes from 64844
          ""IsMasteryTriggered"": true
        },
        {
          ""Id"": 34861,
          ""Name"": ""Holy Word: Sanctify"",
          ""ManaCost"": 0.05,
          ""Range"": 40.0,
          ""NumberOfHealingTargets"": 6.0,
          ""NumberOfDamageTargets"": 0.0,
          ""BaseCastTime"": 0.0,
          ""IsCastTimeHasted"": true,
          ""BaseCooldown"": 60.0,
          ""IsCooldownHasted"": false,
          ""Gcd"": 1.5,
          ""Coeff1"": 2.45,
          ""IsMasteryTriggered"": true
        },
        {
          ""Id"": 2050,
          ""Name"": ""Holy Word: Serenity"",
          ""ManaCost"": 0.04,
          ""Range"": 40.0, // aoe range is 10y
          ""NumberOfHealingTargets"": 1.0,
          ""NumberOfDamageTargets"": 0.0,
          ""BaseCastTime"": 0.0,
          ""IsCastTimeHasted"": true,
          ""BaseCooldown"": 60.0,
          ""IsCooldownHasted"": false,
          ""Gcd"": 1.5,
          ""Coeff1"": 7.0,
          ""IsMasteryTriggered"": true
        },
        {
          ""Id"": 33076,
          ""Name"": ""Prayer of Mending"",
          ""ManaCost"": 0.02,
          ""Range"": 40.0, // Jump range is 20y
          ""NumberOfHealingTargets"": 1.0,
          ""NumberOfDamageTargets"": 0.0,
          ""BaseCastTime"": 0.0,
          ""IsCastTimeHasted"": true,
          ""BaseCooldown"": 12.0,
          ""IsCooldownHasted"": true,
          ""Gcd"": 1.5,
          ""Coeff1"": 0.61, // comes from 33110
          ""IsMasteryTriggered"": true
        },
        {
          ""Id"": 32546,
          ""Name"": ""Binding Heal"",
          ""ManaCost"": 0.034,
          ""Range"": 40.0,
          ""NumberOfHealingTargets"": 3.0,
          ""NumberOfDamageTargets"": 0.0,
          ""BaseCastTime"": 1.5,
          ""IsCastTimeHasted"": true,
          ""BaseCooldown"": 0.0,
          ""IsCooldownHasted"": false,
          ""Gcd"": 1.5,
          ""Coeff1"": 0.94,
          ""IsMasteryTriggered"": true
        },
        {
          ""Id"": 120517,
          ""Name"": ""Halo"",
          ""ManaCost"": 0.027,
          ""Range"": 30.0,
          ""NumberOfHealingTargets"": 6.0,
          ""NumberOfDamageTargets"": 1.0,
          ""BaseCastTime"": 1.5,
          ""IsCastTimeHasted"": true,
          ""BaseCooldown"": 40.0,
          ""IsCooldownHasted"": false,
          ""Gcd"": 1.5,
          ""Coeff1"": 1.44, // comes from 120692
          ""IsMasteryTriggered"": true
        },
        {
          ""Id"": 110744,
          ""Name"": ""Divine Star"",
          ""ManaCost"": 0.02,
          ""Range"": 30.0,
          ""NumberOfHealingTargets"": 6.0,
          ""NumberOfDamageTargets"": 1.0,
          ""BaseCastTime"": 0.0,
          ""IsCastTimeHasted"": true,
          ""BaseCooldown"": 15.0,
          ""IsCooldownHasted"": false,
          ""Gcd"": 1.5,
          ""Coeff1"": 0.7, // comes from 110745
          ""IsMasteryTriggered"": true
        },
        {
          ""Id"": 265202,
          ""Name"": ""Holy Word: Salvation"",
          ""ManaCost"": 0.06,
          ""Range"": 40.0,
          ""NumberOfHealingTargets"": 20.0,
          ""NumberOfDamageTargets"": 0.0,
          ""BaseCastTime"": 2.5,
          ""IsCastTimeHasted"": true,
          ""BaseCooldown"": 720.0,
          ""IsCooldownHasted"": false,
          ""Gcd"": 1.5,
          ""Coeff1"": 1.1,
          ""IsMasteryTriggered"": true
        },
        {
          ""Id"": 323673,
          ""Name"": ""Mindgames"",
          ""ManaCost"": 0.02,
          ""Range"": 40.0,
          ""NumberOfHealingTargets"": 1.0,
          ""NumberOfDamageTargets"": 1.0,
          ""BaseCastTime"": 1.5,
          ""IsCastTimeHasted"": true,
          ""BaseCooldown"": 45.0,
          ""IsCooldownHasted"": false,
          ""Gcd"": 1.5,
          ""Coeff1"": 450, // Damage that heals. Comes from effect 2
          ""Coeff2"": 2.0, // The mana% return. Comes from 323706 effect 3.
          ""Coeff3"": 3.0, // The initial damage component
          ""IsMasteryTriggered"": false
        },
        {
          ""Id"": 327661,
          ""Name"": ""Fae Guardians"",
          ""ManaCost"": 0.02,
          ""Range"": 0.0,
          ""NumberOfHealingTargets"": 0.0,
          ""NumberOfDamageTargets"": 0.0,
          ""BaseCastTime"": 0.0,
          ""IsCastTimeHasted"": true,
          ""BaseCooldown"": 90.0,
          ""IsCooldownHasted"": false,
          ""Duration"": 20.0,
          ""Gcd"": 1.5,
          ""Coeff1"": -10.0, // Damage reduced. Comes from 327694 Effect #1
          ""Coeff2"": 100.0, // CDR increase. 100 = 100%. Comes from 327710 Effect #1
          ""Coeff3"": 50, // The mana return. 50 / 100 = 0.5% mana. Comes from 327703 Effect #1
          ""IsMasteryTriggered"": false
        },
        {
          ""Id"": 325013,
          ""Name"": ""Boon of the Ascended"",
          ""ManaCost"": 0.00,
          ""Range"": 0.0,
          ""NumberOfHealingTargets"": 0.0,
          ""NumberOfDamageTargets"": 0.0,
          ""BaseCastTime"": 1.5,
          ""IsCastTimeHasted"": true,
          ""BaseCooldown"": 180.0,
          ""IsCooldownHasted"": false,
          ""Duration"": 10.0,
          ""Gcd"": 1.5,
          ""Coeff1"": 3 // Additional % healing per stack of boon.
        },
        {
          ""Id"": 325020,
          ""Name"": ""Ascended Nova"",
          ""ManaCost"": 0.00,
          ""Range"": 8.0,
          ""NumberOfHealingTargets"": 5.0,
          ""NumberOfDamageTargets"": 1.0,
          ""BaseCastTime"": 0.0,
          ""IsCastTimeHasted"": true,
          ""BaseCooldown"": 0.0,
          ""IsCooldownHasted"": false,
          ""Duration"": 0.0,
          ""Gcd"": 1.0,
          ""Coeff1"": 0.69, // Damage component from Effect #1
          ""Coeff2"": 0.38, // Healing component
          ""Coeff3"": 6, // Target cap of healing component
          ""IsMasteryTriggered"": true
        },
        {
          ""Id"": 325283,
          ""Name"": ""Ascended Blast"",
          ""ManaCost"": 0.00,
          ""Range"": 40.0,
          ""NumberOfHealingTargets"": 1.0,
          ""NumberOfDamageTargets"": 1.0,
          ""BaseCastTime"": 0.0,
          ""IsCastTimeHasted"": true,
          ""BaseCooldown"": 3.0,
          ""IsCooldownHasted"": true,
          ""Duration"": 0.0,
          ""Gcd"": 1.0,
          ""Coeff1"": 2.6, // Damage component from Effect #1
          ""Coeff2"": 100, // Healing component from the damage. Effect #2. 100 = 100%.
          ""IsMasteryTriggered"": true
        },
        {
          ""Id"": 325326,
          ""Name"": ""Ascended Eruption"",
          ""ManaCost"": 0.00,
          ""Range"": 15.0,
          ""NumberOfHealingTargets"": 5.0,
          ""NumberOfDamageTargets"": 1.0,
          ""BaseCastTime"": 0.0,
          ""IsCastTimeHasted"": true,
          ""BaseCooldown"": 0.0,
          ""IsCooldownHasted"": false,
          ""Duration"": 0.0,
          ""Gcd"": 0.0,
          ""Coeff1"": 3.52, // Damage component from Effect #1
          ""Coeff2"": 2.22000, // Healing component from Effect #2.
          ""IsMasteryTriggered"": true
        },
        {
          ""Id"": 324724,
          ""Name"": ""Unholy Nova"",
          ""ManaCost"": 5.00,
          ""Range"": 15.0, // From 325203 Effect #1
          ""NumberOfHealingTargets"": 6.0,
          ""NumberOfDamageTargets"": 1.0,
          ""BaseCastTime"": 0.0,
          ""IsCastTimeHasted"": true,
          ""BaseCooldown"": 60.0,
          ""IsCooldownHasted"": false,
          ""Duration"": 0.0,
          ""Gcd"": 1.5,
          ""Coeff1"": 1.19000, // Initial heal. 324724 #1
          ""IsMasteryTriggered"": true
        },
        {
          ""Id"": 325118,
          ""Name"": ""Unholy Transfusion"",
          ""ManaCost"": 0.00,
          ""Range"": 100.0, // From 325203 Effect #1
          ""NumberOfHealingTargets"": 1.0,
          ""NumberOfDamageTargets"": 1.0,
          ""BaseCastTime"": 0.0,
          ""IsCastTimeHasted"": true,
          ""BaseCooldown"": 0.0,
          ""IsCooldownHasted"": false,
          ""Duration"": 15.0,
          ""Gcd"": 0.0,
          ""Coeff1"": 0.04, // Heal from DoT component. 325118 #1
          ""Coeff2"": 0.5, // DoT component. 325203 #1
          ""IsMasteryTriggered"": true
        }
      ]
    }
  ],
  ""SharedSpells"": [

  ]
}";
            return json;
        }
    }
}
