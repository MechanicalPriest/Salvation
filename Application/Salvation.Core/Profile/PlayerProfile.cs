using Newtonsoft.Json;
using Salvation.Core.Constants.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Salvation.Core.Profile
{
    public class PlayerProfile
    {
        public Spec Spec { get; set; }
        public string Name { get; set; }
        public string Server { get; set; }
        public string Region { get; set; }
        public int Level { get; set; }
        public Race Race { get; set; }
        public Class Class { get; set; }

        public int Intellect { get; set; }
        public int MasteryRating { get; set; }
        public int VersatilityRating { get; set; }
        public int HasteRating { get; set; }
        public int CritRating { get; set; }

        /// <summary>
        /// Data containing cast effiency and overheal
        /// </summary>
        // TODO: Rename this to something like SpellProfile as it's used for items/passives too
        public List<CastProfile> Casts { get; set; }
        /// <summary>
        /// Complete list of all items the character has available to it
        /// </summary>
        public List<Item> Items { get; set; }
        /// <summary>
        /// Active talents
        /// </summary>
        public List<Talent> Talents { get; set; }
        /// <summary>
        /// Active conduits. Conduit:rank
        /// </summary>
        // TODO: Delete this
        public Dictionary<Conduit, uint> Conduits { get; set; }
        // TODO: Delete this
        public List<Soulbind> Soulbinds { get; set; }

        public CovenantProfile Covenant { get; set; }
        // TODO: Delete this
        public List<Spell> Legendaries { get; set; }

        // Misc info
        public int FightLengthSeconds { get; set; }
        // TODO: Merge these into Casts associated with SpellIds
        public List<PlaystyleEntry> PlaystyleEntries { get; set; }

        public PlayerProfile()
        {
            Casts = new List<CastProfile>();
            Talents = new List<Talent>();
            Conduits = new Dictionary<Conduit, uint>();
            Soulbinds = new List<Soulbind>();
            Legendaries = new List<Spell>();
            PlaystyleEntries = new List<PlaystyleEntry>();
            Items = new List<Item>();
            Covenant = new CovenantProfile();
            Level = 60;
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
