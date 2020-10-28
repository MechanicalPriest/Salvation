using Salvation.Core.Constants.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Salvation.Core.Profile
{
    public class CovenantProfile
    {
        public Covenant Covenant { get; set; }
        public int Renown { get; set; }
        /// <summary>
        /// List of availably conduits and their ranks (0-indexed)
        /// </summary>
        public Dictionary<Conduit, int> AvailableConduits { get; set; }
        public List<SoulbindProfile> Soulbinds { get; set; }

        public CovenantProfile()
        {
            AvailableConduits = new Dictionary<Conduit, int>();
            Soulbinds = new List<SoulbindProfile>();
            Covenant = Covenant.None;
            Renown = 0;
        }
    }
}
