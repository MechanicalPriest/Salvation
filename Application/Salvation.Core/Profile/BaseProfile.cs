using Salvation.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Profile
{
    public class BaseProfile
    {
        public Spec SpecId { get; set; }
        public int Intellect { get; set; }
        public int MasteryRating { get; set; }
        public int VersatilityRating { get; set; }
        public int HasteRating { get; set; }
        public int CritRating { get; set; }
        /// <summary>
        /// Data containing cast effiency and overheal
        /// </summary>
        public List<CastProfile> Casts { get; set; }

        // Talents
        public int T15Talent { get; set; }
        public int T25Talent { get; set; }
        public int T30Talent { get; set; }
        public int T35Talent { get; set; }
        public int T40Talent { get; set; }
        public int T45Talent { get; set; }
        public int T50Talent { get; set; }

        // Misc info
        public int FightLengthSeconds { get; set; }

        public BaseProfile()
        {
            Casts = new List<CastProfile>();
        }
    }
}
