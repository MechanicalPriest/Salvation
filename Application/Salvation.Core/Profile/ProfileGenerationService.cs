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
                    new CastProfile(2060, 0.0603d, 0.1084d, 1, 0), // FH
                    new CastProfile(2061, 0.0664d, 0.3054d, 1, 0), // Heal
                    new CastProfile(139, 0.0364d, 0.3643d, 1 , 0), // Renew
                    new CastProfile(33076, 0.9056d, 0.0219d, 1, 0), // PrayerOfMending
                    new CastProfile(596, 0.2931d, 0.2715d, 5, 0), // PrayerOfHealing
                    new CastProfile(132157, 0.0034d, 0.15d, 20, 1), // HolyNova
                    new CastProfile(2050, 0.677d, 0.1515d, 1, 0), // HolyWordSerenity
                    new CastProfile(34861, 0.7822d, 0.3234d, 6, 0), // HolyWordSanctify
                    new CastProfile(64843, 0.8805d, 0.314d, 20, 0), // DivineHymn
                    new CastProfile(32546, 0d, 0d, 3, 0), // BindingHeal
                    new CastProfile(204883, 0.8653d, 0.1417d, 5, 0), // CircleOfHealing
                    new CastProfile(110744, 0d, 0d, 6, 1), // DivineStar
                    new CastProfile(120517, 0.7596d, 0.3658d, 6, 1), // Halo
                    new CastProfile(265202, 0.874d, 0.3142d, 20, 0), // HolyWordSalvation
                    new CastProfile(238136, 0d, 0.2332d, 5, 0), // CosmicRipple
                    new CastProfile(17, 0d, 0.0d, 1, 0), // PowerWordShield
                    new CastProfile(77485, 0d, 0.4224d, 1, 0), // Echo
                    // Covenants (SpellId, Efficiency, Overheal)
                    new CastProfile(323673, 1.0d, 0.0d, 1, 1), // Mindgames
                    new CastProfile(327661, 1.0d, 0.0d, 0, 0), // Fae Guardians
                    new CastProfile(325013, 1.0d, 0.0d, 0, 0), // Boon of the Ascended
                    new CastProfile(325020, 1.0d, 0.0d, 5, 1), // Ascended Nova
                    new CastProfile(325283, 1.0d, 0.0d, 1, 1), // Ascended Blast
                    new CastProfile(325326, 1.0d, 0.0d, 5, 1), // Ascended Eruption
                    new CastProfile(324724, 1.0d, 0.0d, 6, 1), // Unholy Nova
                    new CastProfile(325118, 1.0d, 0.0d, 1, 1), // Unholy Transfusion
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
