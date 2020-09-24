using Newtonsoft.Json;
using Salvation.Core.Constants.Data;
using Salvation.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Salvation.Core.Profile
{
    public enum Talent
    {
        Enlightenment = 193155, // T15
    }

    public enum Covenant
    {
        [Description("None")]
        None = 0,
        [Description("Kyrian")]
        Kyrian = 1,
        [Description("Venthyr")]
        Venthyr = 2,
        [Description("Night Fae")]
        NightFae = 3,
        [Description("Necrolord")]
        Necrolord = 4,
    }

    

    

    public class BaseProfile
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
        public Dictionary<Conduit, int> Conduits { get; set; }
        public List<Soulbind> Soulbinds { get; set; }

        public Covenant Covenant { get; set; }

        // Misc info
        public int FightLengthSeconds { get; set; }

        public BaseProfile()
        {
            Casts = new List<CastProfile>();
            Talents = new List<Talent>();
            Covenant = Covenant.None;
            Conduits = new Dictionary<Conduit, int>();
            Soulbinds = new List<Soulbind>();
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

        public static BaseProfile Clone(BaseProfile existingProfile)
        {
            return JsonConvert.DeserializeObject<BaseProfile>(JsonConvert.SerializeObject(existingProfile));
        }
    }
}
