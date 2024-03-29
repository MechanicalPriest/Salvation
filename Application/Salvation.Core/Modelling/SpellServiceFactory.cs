﻿using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Modelling.Common.Consumables;
using Salvation.Core.Modelling.Common.Items;
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
                Spell.Heal => typeof(IHealSpellService),
                Spell.FlashHeal => typeof(IFlashHealSpellService),
                Spell.PrayerOfHealing => typeof(IPrayerOfHealingSpellService),
                Spell.HolyNova => typeof(IHolyNovaSpellService),
                Spell.CircleOfHealing => typeof(ICircleOfHealingSpellService),
                Spell.Renew => typeof(IRenewSpellService),
                Spell.PowerWordShield => typeof(IPowerWordShieldSpellService),
                Spell.DivineHymn => typeof(IDivineHymnSpellService),
                Spell.HolyWordSanctify => typeof(IHolyWordSanctifySpellService),
                Spell.HolyWordSerenity => typeof(IHolyWordSerenitySpellService),
                Spell.PrayerOfMending => typeof(IPrayerOfMendingSpellService),
                Spell.Halo => typeof(IHaloSpellService),
                Spell.DivineStar => typeof(IDivineStarSpellService),
                Spell.HolyWordSalvation => typeof(IHolyWordSalvationSpellService),
                Spell.GuardianSpirit => typeof(IGuardianSpiritSpellService),
                // Holy Priest Talent
                //Spell.Enlightenment => typeof(IEnlightenmentSpellService),
                //Spell.CosmicRipple => typeof(ICosmicRippleSpellService),
                //Spell.Benediction => typeof(IBenedictionSpellService),
                // Holy Priest Covenant
                Spell.Mindgames => typeof(IMindgamesSpellService),
                // Holy Priest Damage
                Spell.Smite => typeof(ISmiteSpellService),
                Spell.HolyWordChastise => typeof(IHolyWordChastiseSpellService),
                Spell.ShadowWordPain => typeof(IShadowWordPainSpellService),
                Spell.ShadowWordDeath => typeof(IShadowWordDeathSpellService),
                Spell.HolyFire => typeof(IHolyFireSpellService),
                // Holy Priest Legendary Power
                Spell.EchoOfEonar => typeof(IEchoOfEonarSpellService),
                Spell.CauterizingShadows => typeof(ICauterizingShadowsSpellService),
                Spell.DivineImage => typeof(IDivineImageSpellService),
                // Consumables
                Spell.SpectralFlaskOfPower => typeof(ISpectralFlaskOfPowerSpellService),
                Spell.SpiritualManaPotion => typeof(ISpiritualManaPotionSpellService),
                // Items
                Spell.UnboundChangeling => typeof(IUnboundChangelingSpellService),
                Spell.CabalistsHymnal => typeof(ICabalistsHymnalSpellService),
                Spell.SoullettingRuby => typeof(ISoullettingRubySpellService),
                Spell.ManaboundMirror => typeof(IManaboundMirrorSpellService),
                Spell.MacabreSheetMusic => typeof(IMacabreSheetMusicSpellService),
                Spell.TuftOfSmolderingPlumage => typeof(ITuftOfSmolderingPlumageSpellService),
                Spell.ConsumptiveInfusion => typeof(IConsumptiveInfusionSpellService),
                Spell.DarkmoonDeckRepose => typeof(IDarkmoonDeckReposeSpellService),
                Spell.VialOfSpectralEssence => typeof(IVialOfSpectralEssenceSpellService),
                Spell.OverflowingAnimaCage => typeof(IOverflowingAnimaCageSpellService),
                Spell.SiphoningPhylacteryShard => typeof(ISiphoningPhylacteryShardSpellService),
                _ => null
            };

            if (type == null)
                return null;

            var spellType = typeof(ISpellService<>).MakeGenericType(type);

            return _spellFactory(spellType);
        }
    }
}
