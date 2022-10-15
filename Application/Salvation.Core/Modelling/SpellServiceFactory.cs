using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Modelling.HolyPriest.Spells;
using System;

namespace Salvation.Core.Modelling
{
    public class SpellServiceFactory : ISpellServiceFactory
    {
        private readonly Func<Type, ISpellService> _spellFactory;

        public SpellServiceFactory(Func<Type, ISpellService> spellFactory)
        {
            _spellFactory = spellFactory;
        }

        public ISpellService GetSpellService(Spell spell)
        {
            Type type = spell switch
            {
                // Holy Priest
                Spell.FlashHeal => typeof(IFlashHealSpellService),
                Spell.Heal => typeof(IHealSpellService),
                Spell.HolyFire => typeof(IHolyFireSpellService),
                Spell.PowerWordShield => typeof(IPowerWordShieldSpellService),
                Spell.PrayerOfMending => typeof(IPrayerOfMendingSpellService),
                Spell.Renew => typeof(IRenewSpellService),
                Spell.ShadowWordPain => typeof(IShadowWordPainSpellService),
                Spell.Smite => typeof(ISmiteSpellService),
                // Holy Priest Talent
                Spell.CircleOfHealing => typeof(ICircleOfHealingSpellService),
                Spell.DivineHymn => typeof(IDivineHymnSpellService),
                Spell.DivineImage => typeof(IDivineImageSpellService),
                Spell.GuardianSpirit => typeof(IGuardianSpiritSpellService),
                Spell.HolyWordChastise => typeof(IHolyWordChastiseSpellService),
                Spell.HolyWordSalvation => typeof(IHolyWordSalvationSpellService),
                Spell.HolyWordSanctify => typeof(IHolyWordSanctifySpellService),
                Spell.HolyWordSerenity => typeof(IHolyWordSerenitySpellService),
                Spell.Mindgames => typeof(IMindgamesSpellService),
                Spell.PrayerOfHealing => typeof(IPrayerOfHealingSpellService),
                //Spell.Enlightenment => typeof(IEnlightenmentSpellService),
                Spell.CosmicRipple => typeof(ICosmicRippleSpellService),
                Spell.Lightwell => typeof(ILightwellSpellService),
                //Spell.Benediction => typeof(IBenedictionSpellService),
                // Priest Talent
                Spell.DivineStar => typeof(IDivineStarSpellService),
                Spell.Halo => typeof(IHaloSpellService),
                Spell.HolyNova => typeof(IHolyNovaSpellService),
                Spell.ShadowWordDeath => typeof(IShadowWordDeathSpellService),
                Spell.TwistOfFate => typeof(ITwistOfFateSpellService),
                // Consumables
                // Items
                _ => null
            };

            if (type == null)
                return null;

            var spellType = typeof(ISpellService<>).MakeGenericType(type);

            return _spellFactory(spellType);
        }
    }
}
