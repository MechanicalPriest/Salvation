using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Salvation.Core.Models.Common
{
    public class AveragedSpellCastResult
    {
        public decimal NumberOfHealingTargets { get; set; }
        public decimal NumberOfDamageTargets { get; set; }
        public decimal CastsPerMinute { get; set; }
        public decimal MaximumCastsPerMinute { get; set; }

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

        /// <summary>
        /// Additional spell cast results
        /// </summary>
        public List<AveragedSpellCastResult> AdditionalCasts { get; set; }

        public AveragedSpellCastResult()
        {
            AdditionalCasts = new List<AveragedSpellCastResult>();
        }

        #region Calculated Fields

        public decimal RawHPCT { get => calcRawHPCT(); }
        public decimal RawHPM { get => calcRawHPM(); }
        public decimal RawHPS { get => calcRawHPS(); }
        public decimal HPCT { get => calcHPCT(); }
        public decimal HPM { get => calcHPM(); }
        public decimal HPS { get => calcHPS(); }
        public decimal MPS { get => calcMPS(); }
        public decimal DPS { get => calcDPS(); }
        public decimal DPM { get => calcDPM(); }

        private decimal calcRawHPCT()
        {
            if(CastTime > 0)
            {
                return RawHealing / CastTime;
            }

            if (Gcd > 0)
            {
                return RawHealing / Gcd;
            }
            return 0;
        }

        private decimal calcRawHPM()
        {
            if(ManaCost > 0)
                return RawHealing / ManaCost;
            return 0;
        }

        private decimal calcRawHPS()
        {
            return RawHealing * CastsPerMinute / 60;
        }

        private decimal calcHPCT()
        {
            if (CastTime > 0)
            {
                return Healing / CastTime;
            }

            if (Gcd > 0)
            {
                return Healing / Gcd;
            }
            return 0;
        }

        private decimal calcHPM()
        {
            if (ManaCost > 0)
                return Healing / ManaCost;
            return 0;
        }

        private decimal calcHPS()
        {
            return Healing * CastsPerMinute / 60;
        }

        private decimal calcMPS()
        {
            return ManaCost * CastsPerMinute / 60;
        }

        private decimal calcDPS()
        {
            return Damage * CastsPerMinute / 60;
        }

        private decimal calcDPM()
        {
            if (ManaCost > 0)
                return Damage / ManaCost;
            return 0;
        }


        #endregion

        public void MakeSpellHaveNoCasts()
        {
            CastsPerMinute = 0;
            MaximumCastsPerMinute = 0;
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
