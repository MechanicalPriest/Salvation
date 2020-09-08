using Salvation.Core.Constants;
using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    public class HolyPriestModel
        : BaseModel
    {
        internal enum SpellIds
        {
            // Base healing spells
            Heal = 2060,
            FlashHeal = 2061,
            PrayerOfHealing = 596,
            HolyNova = 132157,
            CircleOfHealing = 204883,
            Renew = 139,
            PowerWordShield = 17,
            DivineHymn = 64843,
            HolyWordSanctify = 34861,
            HolyWordSerenity = 2050,
            PrayerOfMending = 33076,
            BindingHeal = 32546,
            Halo = 120517,
            DivineStar = 110744,
            HolyWordSalvation = 265202,
            EchoOfLight = 77485,

            // Talents
            Enlightenment = 193155,
            CosmicRipple = 238136
        }

        public HolyPriestModel(GlobalConstants constants, BaseProfile profile)
            : base(constants, profile, Spec.HolyPriest)
        {
            Spells.Add(new Heal(this));
            Spells.Add(new FlashHeal(this));
            Spells.Add(new PrayerOfHealing(this));
            Spells.Add(new HolyNova(this));
            Spells.Add(new CircleOfHealing(this));
            Spells.Add(new Renew(this));
            Spells.Add(new PowerWordShield(this));
            Spells.Add(new DivineHymn(this));
            Spells.Add(new HolyWordSanctify(this));
            Spells.Add(new HolyWordSerenity(this));
            Spells.Add(new PrayerOfMending(this));
            Spells.Add(new BindingHeal(this));
            Spells.Add(new Halo(this));
            Spells.Add(new DivineStar(this));
            Spells.Add(new HolyWordSalvation(this));
        }

        public object GetResults()
        {
            /// So what we want to do for this is calculate results for each spell.
            /// Get back result values such as raw direct/periodic healing, raw mastery healing, raw total healing
            /// number of targets hit, mana cost, cast time
            /// inputs that change the results are the profile, and number of targets hit by non-capped aoe spells

            // Total healing
            decimal totalHealing = 0;

            foreach(var spell in Spells)
            {
                if(spell is BaseHolyPriestHealingSpell holySpell)
                {
                    totalHealing += holySpell.CastsPerMinute * holySpell.AverageTotalHeal;
                }
            }

            // Total mana spent
            // TODO: include sources of mana regeneration
            decimal totalManaSpentPerSecond = 0;

            foreach (var spell in Spells)
            {
                if (spell is BaseHolyPriestHealingSpell holySpell)
                {
                    totalManaSpentPerSecond += holySpell.CastsPerMinute * holySpell.ActualManaCost / 60;
                }
            }

            // Total mana regenerated
            decimal totalRegenPerSecond = 0;

            var regenCoeff = Profile.T15Talent == (int)HolyPriestModel.SpellIds.Enlightenment ? 1.1m : 1m;
            totalRegenPerSecond = RawMana * 0.04m * regenCoeff / 5m;

            var totalNegativeManaPerSecond = totalManaSpentPerSecond - totalRegenPerSecond;

            var secondsUntilOutOfMana = RawMana / totalNegativeManaPerSecond;

            return 0;
        }
    }
}
