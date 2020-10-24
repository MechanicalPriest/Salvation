using Newtonsoft.Json;
using Salvation.Core.Constants.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Salvation.Core.Profile
{







    public class PlayerProfile
    {
        public Spec SpecId { get; set; }
        public string Name { get; set; }
        public int Intellect { get; set; }
        public int MasteryRating { get; set; }
        public int VersatilityRating { get; set; }
        public int HasteRating { get; set; }
        public int CritRating { get; set; }
        /// <summary>
        /// Data containing cast effiency and overheal
        /// </summary>
        public List<CastProfile> Casts { get; set; }
        /// <summary>
        /// Active talents
        /// </summary>
        public List<Talent> Talents { get; set; }
        /// <summary>
        /// Active conduits. Conduit:rank
        /// </summary>
        public Dictionary<Conduit, uint> Conduits { get; set; }
        public List<Soulbind> Soulbinds { get; set; }

        public Covenant Covenant { get; set; }
        public List<Spell> Legendaries { get; set; }

        // Misc info
        public int FightLengthSeconds { get; set; }
        public List<PlaystyleEntry> PlaystyleEntries { get; set; }

        public PlayerProfile()
        {
            Casts = new List<CastProfile>();
            Talents = new List<Talent>();
            Covenant = Covenant.None;
            Conduits = new Dictionary<Conduit, uint>();
            Soulbinds = new List<Soulbind>();
            Legendaries = new List<Spell>();
            PlaystyleEntries = new List<PlaystyleEntry>();
        }

        public bool IsTalentActive(Talent talent)
        {
            var exists = Talents.Contains(talent);

            return exists;
        }

        public bool IsSoulbindActive(Soulbind soulbind)
        {
            var exists = Soulbinds.Contains(soulbind);

            return exists;
        }

        public bool IsConduitActive(Conduit conduit)
        {
            var exists = Conduits.Keys.Contains(conduit);

            return exists;
        }

        public static PlayerProfile Clone(PlayerProfile existingProfile)
        {
            return JsonConvert.DeserializeObject<PlayerProfile>(JsonConvert.SerializeObject(existingProfile));
        }
    }
}
