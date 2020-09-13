using Salvation.Core.Constants;
using Salvation.Core.Models.Common;
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
            CosmicRipple = 238136,

            // Cov abilities
            MindGames = 323673,
            FaeGuardians = 327661,
            BoonOfTheAscended = 325013,
            AscendedNova = 325020,
            AscendedBlast = 325283,
            AscendedEruption = 325326,
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
            Spells.Add(new MindGames(this));
            Spells.Add(new FaeGuardians(this));
            Spells.Add(new AscendedEruption(this));
            Spells.Add(new BoonOfTheAscended(this));
        }

        public override BaseModelResults GetResults()
        {
            BaseModelResults results = base.GetResults();

            // Total mana regenerated is specific to Holy Priest
            decimal totalRegenPerSecond;

            var regenCoeff = Profile.T15Talent == (int)HolyPriestModel.SpellIds.Enlightenment ? 1.1m : 1m;
            totalRegenPerSecond = RawMana * 0.04m * regenCoeff / 5m;

            var totalNegativeManaPerSecond = results.TotalMPS - totalRegenPerSecond;

            results.TimeToOom = RawMana / totalNegativeManaPerSecond;

            return results;
        }
    }
}
