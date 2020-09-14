using Salvation.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Salvation.Core.Profile
{
    public enum Talents
    {
        Enlightenment = 193155, // T15
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
        public List<Talents> Talents { get; set; }

        // Misc info
        public int FightLengthSeconds { get; set; }

        public BaseProfile()
        {
            Casts = new List<CastProfile>();
            Talents = new List<Talents>();
        }

        public bool IsTalentActive(Talents talent)
        {
            var exists = Talents.Contains(talent);

            return exists;
        }
    }
}
