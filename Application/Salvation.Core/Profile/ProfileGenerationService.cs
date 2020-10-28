using Newtonsoft.Json;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Profile;
using System;
using System.Collections.Generic;

namespace Salvation.Core.Profile
{
    public class ProfileGenerationService : IProfileGenerationService
    {
        public ProfileGenerationService()
        {

        }

        public PlayerProfile GetDefaultProfile(Spec spec)
        {
            PlayerProfile profile = spec switch
            {
                Spec.HolyPriest => GenerateHolyPriestProfile(),
                _ => throw new ArgumentOutOfRangeException("Spec", "Spec must be a valid supported spec."),
            };

            return profile;
        }

        public void AddConduit(PlayerProfile profile, Conduit conduit, uint rank)
        {
            if (profile.Conduits.ContainsKey(conduit))
            {
                profile.Conduits[conduit] = rank;
            }

            profile.Conduits.Add(conduit, rank);
        }

        public void RemoveConduit(PlayerProfile profile, Conduit conduit)
        {
            if (profile.Conduits.ContainsKey(conduit))
            {
                profile.Conduits.Remove(conduit);
            }
        }

        public void AddTalent(PlayerProfile profile, Talent talent)
        {
            if (!profile.Talents.Contains(talent))
                profile.Talents.Add(talent);
        }

        public void RemoveTalent(PlayerProfile profile, Talent talent)
        {
            if (profile.Talents.Contains(talent))
                profile.Talents.Remove(talent);
        }

        /// <summary>
        /// Swap the profiles covenant. This includes logic to 
        /// </summary>
        /// <param name="cleanupCovenantData">Override to false to not touch soulbinds, conduits etc</param>
        public void SetCovenant(PlayerProfile profile, Covenant covenant, bool cleanupCovenantData = true)
        {
            // Wipe the existing covenant data if we're setting a new covenant
            if (cleanupCovenantData)
                RemoveCovenantData(profile);

            profile.Covenant = new CovenantProfile() { Covenant = covenant };
        }

        public void RemoveCovenantData(PlayerProfile profile)
        {
            profile.Covenant = new CovenantProfile();

            // Wipe soulbinds
            profile.Soulbinds = new List<Soulbind>();

            // Wipe conduits
            profile.Conduits = new Dictionary<Conduit, uint>();
        }

        public void SetSpellCastProfile(PlayerProfile profile, CastProfile castProfile)
        {
            profile.Casts.RemoveAll(c => c.SpellId == castProfile.SpellId);

            profile.Casts.Add(castProfile);
        }

        public void SetProfileName(PlayerProfile profile, string profileName)
        {
            profile.Name = profileName;
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
                    new CastProfile((int)Spell.BindingHeal, 0d, 0d, 3, 0), // BindingHeal
                    new CastProfile((int)Spell.CircleOfHealing, 0.8653d, 0.1417d, 5, 0), // CircleOfHealing
                    new CastProfile((int)Spell.DivineStar, 0d, 0d, 6, 1), // DivineStar
                    new CastProfile((int)Spell.Halo, 0.7596d, 0.3658d, 6, 1), // Halo
                    new CastProfile((int)Spell.HolyWordSalvation, 0.874d, 0.3142d, 20, 0), // HolyWordSalvation
                    new CastProfile((int)Spell.CosmicRipple, 0d, 0.2332d, 5, 0), // CosmicRipple
                    new CastProfile((int)Spell.PowerWordShield, 0d, 0.0d, 1, 0), // PowerWordShield
                    new CastProfile((int)Spell.EchoOfLight, 0d, 0.4224d, 1, 0), // Echo
                    new CastProfile((int)Spell.Smite, 1d, 0, 0, 1), // Smite
                    new CastProfile((int)Spell.ShadowWordPain, 1d, 0, 0, 1), // Shadow Word: Pain
                    new CastProfile((int)Spell.ShadowWordDeath, 1d, 0, 0, 1),
                    new CastProfile((int)Spell.HolyWordChastise, 1d, 0, 0, 1),
                    new CastProfile((int)Spell.HolyFire, 1d, 0, 0, 1),

                    // Covenants (SpellId, Efficiency, Overheal)
                    new CastProfile((int)Spell.Mindgames, 1.0d, 0.0d, 1, 1), // Mindgames
                    new CastProfile((int)Spell.FaeGuardians, 1.0d, 0.0d, 1, 0), // Fae Guardians
                    new CastProfile((int)Spell.BoonOfTheAscended, 1.0d, 0.0d, 0, 0), // Boon of the Ascended
                    new CastProfile((int)Spell.AscendedNova, 1.0d, 0.0d, 5, 1), // Ascended Nova
                    new CastProfile((int)Spell.AscendedBlast, 1.0d, 0.0d, 1, 1), // Ascended Blast
                    new CastProfile((int)Spell.AscendedEruption, 1.0d, 0.0d, 5, 1), // Ascended Eruption
                    new CastProfile((int)Spell.UnholyNova, 1.0d, 0.0d, 6, 1), // Unholy Nova
                    new CastProfile((int)Spell.UnholyTransfusion, 1.0d, 0.0d, 1, 1), // Unholy Transfusion
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
    }
}
