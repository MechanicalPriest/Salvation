using Newtonsoft.Json;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Profile;
using Salvation.Core.Profile.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Salvation.Core.Profile
{
    /// <summary>
    /// This class has methods to interact with a profile at a high level 
    /// i.e. Validating, Cloning the profile. Use GameStateService for general
    /// get/set operations within the profile.
    /// </summary>
    public class ProfileService : IProfileService
    {
        public ProfileService()
        {

        }

        #region Equipment Management

        /// <summary>
        /// Add an item to the profile and optionally equip it (calls EquipItem()).
        /// EquipItem() is automatically called if the item's Equippd property is true.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="item">The item to add</param>
        /// <param name="equip">Set to true to equip it immediately</param>
        public void AddItem(PlayerProfile profile, Item item, bool equip = false)
        {
            // Item can be validated here if needed (effects/spells).
            if (equip || item.Equipped)
                EquipItem(profile, item);

            profile.Items.Add(item);
        }

        /// <summary>
        /// Sets an items state to eqipped
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="item"></param>
        private void EquipItem(PlayerProfile profile, Item item)
        {
            var existingItems = profile.Items
                .Where(i => i.Slot == item.Slot && i.Equipped == true).ToList();

            // If it's not ring/trinket and one already exists, remove
            if (existingItems.Count == 1 && item.Slot != InventorySlot.Finger
                && item.Slot != InventorySlot.Trinket)
            {
                var removeItem = existingItems.First();
                removeItem.Equipped = false;
            }

            // If it is ring or trinket and we already have 2, remove the oldest one
            if (existingItems.Count == 2 &&
                (item.Slot == InventorySlot.Finger || item.Slot == InventorySlot.Trinket))
            {
                var removeItem = existingItems.First();
                removeItem.Equipped = false;
            }

            item.Equipped = true;
        }

        public List<Item> GetEquippedItems(PlayerProfile profile)
        {
            return profile.Items.Where(i => i.Equipped).ToList();
        }

        #endregion Equipment Management

        #region Talent Management

        public void AddTalent(PlayerProfile profile, Talent talent)
        {
            if (!profile.Talents.Contains(talent))
                profile.Talents.Add(talent);
        }

        #endregion Talent Management

        #region Covenant

        public void SetCovenant(PlayerProfile profile, CovenantProfile covenant)
        {
            // Can add logic here to validate soulbinds / active conduits here.
            var newCovenant = new CovenantProfile()
            {
                Covenant = covenant.Covenant,
                Renown = covenant.Renown
            };
            // TODO: Create all 3 soulbinds for this class/covenant first to prepopulate

            profile.Covenant = newCovenant;

            // Apply soulbinds
            foreach (var soulbind in covenant.Soulbinds)
            {
                AddSoulbind(profile, soulbind);
            }

            // Apply available conduits
            foreach (var conduit in covenant.AvailableConduits)
            {
                AddAvailableConduit(profile, conduit.Key, conduit.Value);
            }
        }

        public void AddSoulbind(PlayerProfile profile, SoulbindProfile soulbind)
        {
            // If we already have this soulbind, replace it.
            profile.Covenant.Soulbinds.RemoveAll(s => s.SoulbindId == soulbind.SoulbindId);

            // TODO: More validation logic on soulbind before adding it?
            profile.Covenant.Soulbinds.Add(soulbind);
        }

        public void AddAvailableConduit(PlayerProfile profile, Conduit conduit, int conduitRank)
        {
            if (profile.Covenant.AvailableConduits.ContainsKey(conduit))
                profile.Covenant.AvailableConduits[conduit] = conduitRank;
            else
                profile.Covenant.AvailableConduits.Add(conduit, conduitRank);
        }

        public void AddActiveConduit(PlayerProfile profile, Conduit conduit,
            uint conduitRank, int soulbindId = 0)
        {
            SoulbindProfile soulbind;

            if (soulbindId > 0)
            {
                // If soulbind is set, search for it
                soulbind = profile.Covenant.Soulbinds
                    .Where(s => s.SoulbindId == soulbindId).FirstOrDefault();
            }
            else
            {
                // Otherwise, use the current active soulbind
                soulbind = profile.Covenant.Soulbinds
                    .Where(s => s.IsActive).FirstOrDefault();
            }

            if (soulbind == null)
                return;

            soulbind.ActiveConduits.Add(conduit, conduitRank);
        }

        #endregion

        /// <summary>
        /// Sets the cast profile for the spellid set inside the provided CastProfile.
        /// Will overwrite an existing CastProfile entry if exists.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="castProfile"></param>
        public void SetSpellCastProfile(PlayerProfile profile, CastProfile castProfile)
        {
            profile.Casts.RemoveAll(c => c.SpellId == castProfile.SpellId);

            profile.Casts.Add(castProfile);
        }

        /// <summary>
        /// Sets the playstyle for the name set inside the provided playstyle.
        /// Will overwrite an existing playstyle entry if exists.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="playstyle"></param>
        public void SetPlaystyleEntry(PlayerProfile profile, PlaystyleEntry playstyle)
        {
            profile.PlaystyleEntries.RemoveAll(p => p.Name == playstyle.Name);

            profile.PlaystyleEntries.Add(playstyle);
        }

        public void SetProfileName(PlayerProfile profile, string profileName)
        {
            profile.Name = profileName;
        }

        #region Profile Management

        public PlayerProfile GetDefaultProfile(Spec spec)
        {
            PlayerProfile profile = spec switch
            {
                Spec.HolyPriest => GenerateHolyPriestProfile(),
                _ => throw new ArgumentOutOfRangeException("Spec", "Spec must be a valid supported spec."),
            };

            return profile;
        }

        /// <summary>
        /// Attempts to re-create the profile following standard rules. 
        /// Should be used after a profile is obtain from an unknown source
        /// such as API endpoints or desserialisation. Errors are ignored
        /// and invalid data is silently dropped. TODO: Capture dropped data
        /// </summary>
        /// <param name="profile">The profile from an unknown source</param>
        /// <returns>A parsed and validated profile</returns>
        public PlayerProfile ValidateProfile(PlayerProfile profile)
        {
            PlayerProfile newProfile = new PlayerProfile()
            {
                // First, apply basic stats and settings
                Spec = profile.Spec,
                Name = profile.Name,
                Server = profile.Server,
                Region = profile.Region,
                Race = profile.Race,
                Class = profile.Class,
                Level = profile.Level,
                FightLengthSeconds = profile.FightLengthSeconds,
            };

            // Casts (spell usage).
            foreach (var cast in profile.Casts)
            {
                SetSpellCastProfile(newProfile, cast);
            }

            // Items
            foreach (var item in profile.Items)
            {
                AddItem(newProfile, item);
            }

            // Talents
            foreach (var talent in profile.Talents)
            {
                AddTalent(newProfile, talent);
            }

            // Covenant
            SetCovenant(newProfile, profile.Covenant);

            // Playstyle entries
            foreach (var playstyle in profile.PlaystyleEntries)
            {
                SetPlaystyleEntry(newProfile, playstyle);
            }

            return newProfile;
        }

        private PlayerProfile GenerateHolyPriestProfile()
        {
            var basicProfile = new PlayerProfile()
            {
                Name = "Holy Priest Default Profile",
                Spec = Spec.HolyPriest,
                Race = Race.NoRace,
                Class = Class.Priest,
                Casts = new List<CastProfile>()
                {
                    new CastProfile((int)Spell.LeechHeal, 0.0d, 0.3475d, 1, 0),

                    // Base Spells (SpellId, Efficiency, Overheal, HealTargets, DamageTargets)
                    new CastProfile((int)Spell.FlashHeal, 0.1103d, 0.1084d, 1, 0),
                    new CastProfile((int)Spell.Heal, 0.1564d, 0.3054d, 1, 0),
                    new CastProfile((int)Spell.Renew, 0.0364d, 0.3643d, 1, 0),
                    new CastProfile((int)Spell.PrayerOfMending, 0.9056d, 0.0219d, 1, 0),
                    new CastProfile((int)Spell.PrayerOfHealing, 0.2931d, 0.2715d, 5, 0),
                    new CastProfile((int)Spell.HolyNova, 0.0034d, 0.15d, 20, 1),
                    new CastProfile((int)Spell.HolyWordSerenity, 0.677d, 0.1515d, 1, 0),
                    new CastProfile((int)Spell.HolyWordSanctify, 0.7822d, 0.3234d, 6, 0),
                    new CastProfile((int)Spell.DivineHymn, 0.8005d, 0.314d, 20, 0),
                    new CastProfile((int)Spell.BindingHeal, 0.12d, 0.34d, 3, 0),
                    new CastProfile((int)Spell.CircleOfHealing, 0.8653d, 0.1417d, 5, 0),
                    new CastProfile((int)Spell.DivineStar, 0.81d, 0.44d, 6, 1),
                    new CastProfile((int)Spell.Halo, 0.7596d, 0.3658d, 6, 1),
                    new CastProfile((int)Spell.HolyWordSalvation, 0.804d, 0.3142d, 20, 0),
                    new CastProfile((int)Spell.CosmicRipple, 0d, 0.2332d, 5, 0),
                    new CastProfile((int)Spell.PowerWordShield, 0.01d, 0.38d, 1, 0),
                    new CastProfile((int)Spell.EchoOfLight, 0d, 0.4224d, 1, 0),
                    new CastProfile((int)Spell.GuardianSpirit, 0.36d, 0d, 1, 0),

                    // DPS Spells
                    new CastProfile((int)Spell.Smite, 0.12d, 0, 0, 1),
                    new CastProfile((int)Spell.ShadowWordPain, 0.04d, 0, 0, 1),
                    new CastProfile((int)Spell.ShadowWordDeath, 0.01d, 0, 0, 1),
                    new CastProfile((int)Spell.HolyWordChastise, 0.01d, 0, 0, 1),
                    new CastProfile((int)Spell.HolyFire, 0.01d, 0, 0, 1),

                    // Covenants (SpellId, Efficiency, Overheal, HealTargets, DamageTargets)
                    new CastProfile((int)Spell.Mindgames, 1.0d, 0.01d, 1, 1),
                    new CastProfile((int)Spell.FaeGuardians, 1.0d, 0.01d, 1, 0),
                    new CastProfile((int)Spell.BoonOfTheAscended, 1.0d, 0.01d, 0, 0),
                    new CastProfile((int)Spell.AscendedNova, 1.0d, 0.31d, 6, 1),
                    new CastProfile((int)Spell.AscendedBlast, 1.0d, 0.16d, 1, 1),
                    new CastProfile((int)Spell.AscendedEruption, 1.0d, 0.47d, 10, 1),
                    new CastProfile((int)Spell.UnholyNova, 1.0d, 0.24d, 6, 1),
                    new CastProfile((int)Spell.UnholyTransfusionDoT, 1.0d, 0.46d, 20, 1),
                    new CastProfile((int)Spell.Fleshcraft, 1.0d, 0.01d, 1, 0),

                    // Consumables (SpellId, Efficiency, Overheal, HealTargets, DamageTargets)
                    new CastProfile((int)Spell.SpiritualManaPotion, 0.9d, 0.00d, 0, 0),
                    
                    // Covenant Traits (SpellId, Efficiency, Overheal, HealTargets, DamageTargets)                   
                    new CastProfile((int)Spell.ResonantAccolades, 0.0d, 0.5d, 1, 0),
                    new CastProfile((int)Spell.BronsCallToAction, 0.0d, 0.1d, 1, 0),
                    new CastProfile((int)Spell.ValiantStrikes, 0.0d, 0.01d, 1, 0),
                    new CastProfile((int)Spell.UltimateForm, 0.0d, 0.25d, 1, 0),
                    
                    // Legendaries (SpellId, Efficiency, Overheal, HealTargets, DamageTargets)
                    new CastProfile((int)Spell.DivineImageBlessedLight, 0.0d, 0.10d, 5, 0),
                    new CastProfile((int)Spell.DivineImageDazzlingLight, 0.0d, 0.10d, 5, 0),
                    new CastProfile((int)Spell.DivineImageHealingLight, 0.0d, 0.05d, 1, 0),
                    new CastProfile((int)Spell.DivineImageLightEruption, 0.0d, 0.25d, 1, 0),
                    new CastProfile((int)Spell.DivineImageSearingLight, 0.0d, 0.25d, 1, 0),
                    new CastProfile((int)Spell.DivineImageTranquilLight, 0.0d, 0.20d, 1, 0),

                    // Items  (SpellId, Efficiency, Overheal, HealTargets, DamageTargets)
                    new CastProfile((int)Spell.SoullettingRuby, 0.9d, 0.30d, 1, 0),
                    new CastProfile((int)Spell.ManaboundMirror, 0.9d, 0.15d, 1, 0),
                    new CastProfile((int)Spell.MacabreSheetMusic, 0.9d, 0.0d, 0, 0),
                    new CastProfile((int)Spell.TuftOfSmolderingPlumage, 0.6d, 0.30d, 1, 0),
                    new CastProfile((int)Spell.ConsumptiveInfusion, 0.9d, 0.00d, 0, 0),
                    new CastProfile((int)Spell.DarkmoonDeckRepose, 0.80d, 0.25d, 4, 0),
                    new CastProfile((int)Spell.VialOfSpectralEssence, 0.95d, 0.05d, 1, 0),
                },
                Talents = new List<Talent>()
                {
                    Talent.Enlightenment
                },
                FightLengthSeconds = 397,
                PlaystyleEntries = new List<PlaystyleEntry>()
                {
                    // #### Base Overrides ####
                    // Overrides the stat value to be set directly rather than from items/race/class
                    new PlaystyleEntry("OverrideStatIntellect", 0, (uint)Spell.HolyPriest),
                    new PlaystyleEntry("OverrideStatHaste", 0, (uint)Spell.HolyPriest),
                    new PlaystyleEntry("OverrideStatCriticalStrike", 0, (uint)Spell.HolyPriest),
                    new PlaystyleEntry("OverrideStatVersatility", 0, (uint)Spell.HolyPriest),
                    new PlaystyleEntry("OverrideStatMastery", 0, (uint)Spell.HolyPriest),
                    new PlaystyleEntry("OverrideStatLeech", 0, (uint)Spell.HolyPriest),

                    // Adds additional stats (for use with stat weights)
                    new PlaystyleEntry("GrantAdditionalStatIntellect", 0, (uint)Spell.HolyPriest),
                    new PlaystyleEntry("GrantAdditionalStatMastery", 0, (uint)Spell.HolyPriest),
                    new PlaystyleEntry("GrantAdditionalStatHaste", 0, (uint)Spell.HolyPriest),
                    new PlaystyleEntry("GrantAdditionalStatVersatility", 0, (uint)Spell.HolyPriest),
                    new PlaystyleEntry("GrantAdditionalStatCriticalStrike", 0, (uint)Spell.HolyPriest),
                    new PlaystyleEntry("GrantAdditionalStatLeech", 0, (uint)Spell.HolyPriest),

                    new PlaystyleEntry("LeechSelfHealPercent", 0.20, (uint)Spell.HolyPriest),

                    // Force the cloth armor bonus
                    new PlaystyleEntry("ForceClothBonus", 0, (uint)Spell.HolyPriest),
                    // Amount of average damage taken per second over the course of a fight
                    new PlaystyleEntry("DamageTakenPerSecond", 1500, (uint)Spell.HolyPriest),
                    
                    // #### Holy Priest ####
                    // ## Covenants overrides

                    new PlaystyleEntry("FaeBenevolentFaerieSelfUptime", 1, (uint)Spell.BenevolentFaerie),
                    // The number of times you move the Guardian Faerie around
                    new PlaystyleEntry("FaeFermataNumberDRSwaps", 1, (uint)Spell.FaeFermata),
                    // The number of times you move the Benevolent Faerie around
                    new PlaystyleEntry("FaeFermataNumberCDRSwaps", 3, (uint)Spell.FaeFermata),
                    new PlaystyleEntry("FaeGuardianFaerieDTPS", 4000, (uint)Spell.GuardianFaerie),
                    // Extra shield you get. It can be anywhere from 2.5x at the moment. default to 1 (no mod)
                    new PlaystyleEntry("FleshCraftShieldMultiplier", 1, (uint)Spell.Fleshcraft),

                    // ## Damage & Healing overrides
                    new PlaystyleEntry("ShadowWordDeathPercentExecute", 0.2, (uint)Spell.ShadowWordDeath),
                    new PlaystyleEntry("HolyNovaPercentOfCastsOnThreeOrMore", 0.1, (uint)Spell.HolyNovaRank2),

                    // ## Item overrides
                    new PlaystyleEntry("SoullettingRubyAverageEnemyHP", 0.5, (uint)Spell.SoullettingRuby),
                    // Average amount of the mirror that's filled when it's cast. Very easy to fill playing normally.
                    new PlaystyleEntry("ManaboundMirrorPercentMirrorFilled", 1.0, (uint)Spell.ManaboundMirror),
                    // The average amount of health as a percentage that the target has
                    new PlaystyleEntry("TuftOfSmolderingPlumageAvgAllyHp", .75, (uint)Spell.TuftOfSmolderingPlumage),

                    // ## Legendary overrides
                    new PlaystyleEntry("EchoOfEonarCountAllyBuffs", 1, (uint)Spell.EchoOfEonar),
                    new PlaystyleEntry("CauterizingShadowsSwpExpiryPercent", 0.9, (uint)Spell.CauterizingShadows),
                    new PlaystyleEntry("FlashConcentrationAverageStacks", 5, (uint)Spell.FlashConcentration),

                    // ## Conduit overrides
                    // How often Charitable Soul is cast on an alt. 0.9 = 90% of the time
                    new PlaystyleEntry("CharitableSoulAllyCasts", 0.9, (uint)Spell.CharitableSoul),
                    new PlaystyleEntry("ResonantWordsPercentageBuffsUsed", 0.85, (uint)Spell.ResonantWords),
                    new PlaystyleEntry("ResonantWordsPercentageBuffsHeal", 0.75, (uint)Spell.ResonantWords),

                    // ## Soulbind trait overrides
                    // #### Kyrian
                    // Number of orbs picked up per cast
                    new PlaystyleEntry("CombatMeditationOrbPickups", 1.0, (uint)Spell.LetGoOfThePast),
                    // Number of average stacks while it's up
                    new PlaystyleEntry("LetGoOfThePastAverageStacks", 2.5, (uint)Spell.LetGoOfThePast),
                    // Average uptime as a percentage. 1 = 100%
                    new PlaystyleEntry("LetGoOfThePastAverageUptime", 0.9, (uint)Spell.LetGoOfThePast),
                    // Average number of nearby allies
                    new PlaystyleEntry("PointedCourageAverageNearbyAllies", 4.5, (uint)Spell.PointedCourage),
                    // The rough number of crittable events per minute
                    new PlaystyleEntry("ValiantStrikesEventsPerMinute", 120, (uint)Spell.ValiantStrikes),
                    // Number of times Valiant Strikes procs per minute
                    new PlaystyleEntry("ValiantStrikesProcsPerMinute", 1, (uint)Spell.ValiantStrikes),
                    // Percentage of healing events over 70% for resonant accolades
                    new PlaystyleEntry("ResonantAccoladesHealingOver70Percent", 0.7, (uint)Spell.ResonantAccolades),
                    // Bronns spellpower per cast. It's currently 1.15.
                    new PlaystyleEntry("BronsCallToActionSpellpower", 1.15, (uint)Spell.BronsCallToAction),
                    // The average amount of times Bronn procs per minute
                    new PlaystyleEntry("BronsCallToActionProcsPerMinute", 0.4, (uint)Spell.BronsCallToAction),
                    // Amount of times bron casts healing spells during his duration
                    new PlaystyleEntry("BronsCallToActionCastsPerProc", 5.25, (uint)Spell.BronsCallToAction),

                    // #### Necro
                    // Whether to include allies or not. 1 = yes.
                    new PlaystyleEntry("LeadByExampleIncludeAllyBuffs", 1, (uint)Spell.LeadByExample),
                    // Number of nearby allies when proccing it
                    new PlaystyleEntry("LeadByExampleNearbyAllies", 4, (uint)Spell.LeadByExample),
                    // The rough number of crit-able events per minute
                    new PlaystyleEntry("MarrowedGemstoneEventsPerMinute", 200, (uint)Spell.MarrowedGemstone),

                    // #### Night Fae 
                    // Whether to include allies or not. 1 = yes.
                    new PlaystyleEntry("NiyasToolsHerbsUptime", .775, (uint)Spell.NiyasToolsHerbs),
                },
                Items = new List<Item>()
                {

                }
            };

            return basicProfile;
        }

        /// <summary>
        /// Deep clone a profile by serialising and deserialising it as JSON.
        /// </summary>
        /// <param name="profile">The profile to be cloned</param>
        /// <returns>A fresh instance of the profile</returns>
        public PlayerProfile CloneProfile(PlayerProfile profile)
        {
            var profileString = JsonConvert.SerializeObject(profile);

            var newProfile = JsonConvert.DeserializeObject<PlayerProfile>(profileString);

            return newProfile;
        }

        #endregion
    }
}
