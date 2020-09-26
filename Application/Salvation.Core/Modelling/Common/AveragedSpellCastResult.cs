using System.Collections.Generic;

namespace Salvation.Core.Modelling.Common
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

        public override string ToString()
        {
            return $"[{SpellName}(id={SpellId})] RawHPS: {RawHPS:0.##} HPS: {HPS:0.##} CPM: {CastsPerMinute:0.##} MaxCPM: {MaximumCastsPerMinute:0.##}";
        }

        #region Calculated Fields

        public decimal RawHPCT { get => CalcRawHPCT(); }
        public decimal RawHPM { get => CalcRawHPM(); }
        public decimal RawHPS { get => CalcRawHPS(); }
        public decimal HPCT { get => CalcHPCT(); }
        public decimal HPM { get => CalcHPM(); }
        public decimal HPS { get => CalcHPS(); }
        /// <summary>
        /// Overhealing per second
        /// </summary>
        public decimal OPS { get => CalcOPS(); }
        public decimal MPS { get => CalcMPS(); }
        public decimal DPS { get => CalcDPS(); }
        public decimal DPM { get => CalcDPM(); }

        private decimal CalcRawHPCT()
        {
            if (CastTime > 0)
            {
                return RawHealing / CastTime;
            }

            if (Gcd > 0)
            {
                return RawHealing / Gcd;
            }
            return 0;
        }

        private decimal CalcRawHPM()
        {
            if (ManaCost > 0)
                return RawHealing / ManaCost;
            return 0;
        }

        private decimal CalcRawHPS()
        {
            return RawHealing * CastsPerMinute / 60;
        }

        private decimal CalcHPCT()
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

        private decimal CalcHPM()
        {
            if (ManaCost > 0)
                return Healing / ManaCost;
            return 0;
        }

        private decimal CalcHPS()
        {
            return Healing * CastsPerMinute / 60;
        }

        private decimal CalcMPS()
        {
            return ManaCost * CastsPerMinute / 60;
        }

        private decimal CalcDPS()
        {
            return Damage * CastsPerMinute / 60;
        }

        private decimal CalcDPM()
        {
            if (ManaCost > 0)
                return Damage / ManaCost;
            return 0;
        }

        private decimal CalcOPS()
        {
            return Overhealing * CastsPerMinute / 60;
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
