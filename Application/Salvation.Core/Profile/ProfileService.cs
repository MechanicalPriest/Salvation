﻿using Newtonsoft.Json;
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
                // Stats
                // TODO: Remove these, instead use the Get X methods.
                Intellect = profile.Intellect,
                MasteryRating = profile.MasteryRating,
                VersatilityRating = profile.VersatilityRating,
                HasteRating = profile.HasteRating,
                CritRating = profile.CritRating
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
                Intellect = 0,
                MasteryRating = 0,
                VersatilityRating = 0,
                HasteRating = 0,
                CritRating = 0,
                Race = Race.NoRace,
                Class = Class.Priest,
                Casts = new List<CastProfile>()
                {
                    // Base Spells (SpellId, Efficiency, Overheal)
                    new CastProfile((int)Spell.FlashHeal, 0.0603d, 0.1084d, 1, 0), // FH
                    new CastProfile((int)Spell.Heal, 0.0664d, 0.3054d, 1, 0), // Heal
                    new CastProfile((int)Spell.Renew, 0.0364d, 0.3643d, 1 , 0), // Renew
                    new CastProfile((int)Spell.PrayerOfMending, 0.9056d, 0.0219d, 1, 0), // PrayerOfMending
                    new CastProfile((int)Spell.PrayerOfHealing, 0.2931d, 0.2715d, 5, 0), // PrayerOfHealing
                    new CastProfile((int)Spell.HolyNova, 0.0034d, 0.15d, 20, 1), // HolyNova
                    new CastProfile((int)Spell.HolyWordSerenity, 0.677d, 0.1515d, 1, 0), // HolyWordSerenity
                    new CastProfile((int)Spell.HolyWordSanctify, 0.7822d, 0.3234d, 6, 0), // HolyWordSanctify
                    new CastProfile((int)Spell.DivineHymn, 0.8805d, 0.314d, 20, 0), // DivineHymn
                    new CastProfile((int)Spell.BindingHeal, 0.12d, 0.34d, 3, 0), // BindingHeal
                    new CastProfile((int)Spell.CircleOfHealing, 0.8653d, 0.1417d, 5, 0), // CircleOfHealing
                    new CastProfile((int)Spell.DivineStar, 0.81d, 0.44d, 6, 1), // DivineStar
                    new CastProfile((int)Spell.Halo, 0.7596d, 0.3658d, 6, 1), // Halo
                    new CastProfile((int)Spell.HolyWordSalvation, 0.874d, 0.3142d, 20, 0), // HolyWordSalvation
                    new CastProfile((int)Spell.CosmicRipple, 0d, 0.2332d, 5, 0), // CosmicRipple
                    new CastProfile((int)Spell.PowerWordShield, 0.01d, 0.38d, 1, 0), // PowerWordShield
                    new CastProfile((int)Spell.EchoOfLight, 0d, 0.4224d, 1, 0), // Echo
                    new CastProfile((int)Spell.Smite, 1d, 0, 0, 1), // Smite
                    new CastProfile((int)Spell.ShadowWordPain, 1d, 0, 0, 1), // Shadow Word: Pain
                    new CastProfile((int)Spell.ShadowWordDeath, 1d, 0, 0, 1),
                    new CastProfile((int)Spell.HolyWordChastise, 1d, 0, 0, 1),
                    new CastProfile((int)Spell.HolyFire, 1d, 0, 0, 1),

                    // Covenants (SpellId, Efficiency, Overheal)
                    new CastProfile((int)Spell.Mindgames, 1.0d, 0.01d, 1, 1), // Mindgames
                    new CastProfile((int)Spell.FaeGuardians, 1.0d, 0.01d, 1, 0), // Fae Guardians
                    new CastProfile((int)Spell.BoonOfTheAscended, 1.0d, 0.01d, 0, 0), // Boon of the Ascended
                    new CastProfile((int)Spell.AscendedNova, 1.0d, 0.01d, 5, 1), // Ascended Nova
                    new CastProfile((int)Spell.AscendedBlast, 1.0d, 0.01d, 1, 1), // Ascended Blast
                    new CastProfile((int)Spell.AscendedEruption, 1.0d, 0.01d, 5, 1), // Ascended Eruption
                    new CastProfile((int)Spell.UnholyNova, 1.0d, 0.01d, 6, 1), // Unholy Nova
                    new CastProfile((int)Spell.UnholyTransfusion, 1.0d, 0.01d, 1, 1), // Unholy Transfusion
                },
                Talents = new List<Talent>()
                {
                    Talent.Enlightenment
                },
                FightLengthSeconds = 397,
                PlaystyleEntries = new List<PlaystyleEntry>()
                {
                    new PlaystyleEntry("FaeBenevolentFaerieSelfUptime", 1, (uint)Spell.BenevolentFaerie),
                    new PlaystyleEntry("FaeGuardianFaerieDTPS", 4000, (uint)Spell.GuardianFaerie),
                    new PlaystyleEntry("ShadowWordDeathPercentExecute", 0.2, (uint)Spell.ShadowWordDeath),
                    new PlaystyleEntry("HolyNovaPercentOfCastsOnThreeOrMore", 0.1, (uint)Spell.HolyNovaRank2),

                    // The number of times you move the Guardian Faerie around
                    new PlaystyleEntry("FaeFermataNumberDRSwaps", 1, (uint)Spell.FaeFermata),
                    // The number of times you move the Benevolent Faerie around
                    new PlaystyleEntry("FaeFermataNumberCDRSwaps", 3, (uint)Spell.FaeFermata),

                    // Overrides the stat value to be set directly rather than from items/race/class
                    new PlaystyleEntry("OverrideStatIntellect", 0, 0),
                    new PlaystyleEntry("OverrideStatHaste", 0, 0),
                    new PlaystyleEntry("OverrideStatCriticalStrike", 0, 0),
                    new PlaystyleEntry("OverrideStatVersatility", 0, 0),
                    new PlaystyleEntry("OverrideStatMastery", 0, 0),
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
