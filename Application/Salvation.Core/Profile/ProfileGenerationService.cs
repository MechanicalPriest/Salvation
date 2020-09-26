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
            PlayerProfile profile;

            switch (spec)
            {
                case Spec.HolyPriest:
                    profile = GenerateHolyPriestProfile();
                    break;

                case Spec.None:
                default:
                    throw new ArgumentOutOfRangeException("Spec", "Spec must be a valid supported spec.");
            }

            return profile;
        }

        public void AddConduit(PlayerProfile profile, Conduit conduit, int rank)
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

            profile.Covenant = covenant;
        }

        public void RemoveCovenantData(PlayerProfile profile)
        {
            profile.Covenant = Covenant.None;

            // Wipe soulbinds
            profile.Soulbinds = new List<Soulbind>();

            // Wipe conduits
            profile.Conduits = new Dictionary<Conduit, int>();
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
                SpecId = Spec.HolyPriest,
                Intellect = 1001,
                MasteryRating = 242,
                VersatilityRating = 139,
                HasteRating = 242,
                CritRating = 268,
                Casts = new List<CastProfile>()
                {
                    // Base Spells (SpellId, Efficiency, Overheal)
                    new CastProfile(2060, 0.0603m, 0.1084m), // FH
                    new CastProfile(2061, 0.0664m, 0.3054m), // Heal
                    new CastProfile(139, 0.0364m, 0.3643m), // Renew
                    new CastProfile(33076, 0.9056m, 0.0219m), // PrayerOfMending
                    new CastProfile(596, 0.2931m, 0.2715m), // PrayerOfHealing
                    new CastProfile(132157, 0.0034m, 0.15m), // HolyNova
                    new CastProfile(2050, 0.677m, 0.1515m), // HolyWordSerenity
                    new CastProfile(34861, 0.7822m, 0.3234m), // HolyWordSanctify
                    new CastProfile(64843, 0.8805m, 0.314m), // DivineHymn
                    new CastProfile(32546, 0m, 0m), // BindingHeal
                    new CastProfile(204883, 0.8653m, 0.1417m), // CircleOfHealing
                    new CastProfile(110744, 0m, 0m), // DivineStar
                    new CastProfile(120517, 0.7596m, 0.3658m), // Halo
                    new CastProfile(265202, 0.874m, 0.3142m), // HolyWordSalvation
                    new CastProfile(238136, 0m, 0.2332m), // CosmicRipple
                    new CastProfile(17, 0m, 0.0m), // PowerWordShield
                    new CastProfile(77485, 0, 0.4224m), // Echo
                    new CastProfile(323673, 1.0m, 0.0m), // Mindgames
                    // Covenants (SpellId, Efficiency, Overheal)
                    new CastProfile(327661, 1.0m, 0.0m), // Fae Guardians
                    new CastProfile(325013, 1.0m, 0.0m), // Boon of the Ascended
                    new CastProfile(325020, 1.0m, 0.0m), // Ascended Nova
                    new CastProfile(325283, 1.0m, 0.0m), // Ascended Blast
                    new CastProfile(325326, 1.0m, 0.0m), // Ascended Eruption
                    new CastProfile(324724, 1.0m, 0.0m), // Unholy Nova
                    new CastProfile(325118, 1.0m, 0.0m), // Unholy Transfusion
                },
                Talents = new List<Talent>()
                {
                    Talent.Enlightenment
                },
                FightLengthSeconds = 397
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
