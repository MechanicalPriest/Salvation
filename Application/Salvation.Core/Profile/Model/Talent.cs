using Salvation.Core.Constants.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salvation.Core.Profile.Model
{
    public class Talent
    {
        public Spell Spell { get; set; }
        public int SpellId { get; set; }
        /// <summary>
        /// The rank of the talent (how many points have been spent)
        /// </summary>
        public int Rank { get; set; }

        public Talent() { }

        public Talent (Spell spell, int ranks = 0)
        {
            Spell = spell;
            SpellId = (int)spell;
            Rank = ranks;
        }
    }
}
