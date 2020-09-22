using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.Common
{
    /// <summary>
    /// Store everything 
    /// </summary>
    public class SpellCastResult
    {

        public int SpellId { get; set; }
        public string SpellName { get; set; }

        /// <summary>
        /// Raw healing done excluding overheal
        /// </summary>
        public decimal RawHealing { get; set; }
        /// <summary>
        /// Healing done accounting for overheal
        /// </summary>
        public decimal Healing { get; set; }
        /// <summary>
        /// Overhealing done
        /// </summary>
        public decimal Overhealing { get; set; }
        /// <summary>
        /// Total damage done
        /// </summary>
        public decimal Damage { get; set; }
        /// <summary>
        /// Haste adjusted cast time. 0 is instant-cast
        /// </summary>
        public decimal CastTime { get; set; }
        /// <summary>
        /// Haste adjusted cooldown at time of cast. 0 no CD.
        /// </summary>
        public decimal Cooldown { get; set; }
        public decimal Duration { get; set; }
        /// <summary>
        /// Haste adjusted GCD. 
        /// </summary>
        public decimal Gcd { get; set; }
        /// <summary>
        /// Actual mana cost value of this spell cast
        /// </summary>
        public decimal ManaCost { get; set; }

        #region Calculated Fields

        #endregion

        public List<SpellCastResult> AdditionalCasts { get; set; }

        public SpellCastResult()
        {
            AdditionalCasts = new List<SpellCastResult>();
        }

        public void MakeCastFree()
        {
            ManaCost = 0;
        }

        public void MakeCastInstant()
        {
            CastTime = 0;
        }

        public void MakeCastHaveNoGcd()
        {
            Gcd = 0;
        }
    }
}
