using System.Collections.Generic;

namespace Salvation.Core.Modelling.Common
{
    public class AveragedSpellCastResult
    {
        public double NumberOfHealingTargets { get; set; }
        public double NumberOfDamageTargets { get; set; }
        public double CastsPerMinute { get; set; }
        public double MaximumCastsPerMinute { get; set; }

        public int SpellId { get; set; }
        public string SpellName { get; set; }

        /// <summary>
        /// Raw healing done excluding overheal
        /// </summary>
        public double RawHealing { get; set; }
        /// <summary>
        /// Healing done accounting for overheal
        /// </summary>
        public double Healing { get; set; }
        /// <summary>
        /// Overhealing done
        /// </summary>
        public double Overhealing { get; set; }
        /// <summary>
        /// Total damage done
        /// </summary>
        public double Damage { get; set; }
        /// <summary>
        /// Haste adjusted cast time. 0 is instant-cast
        /// </summary>
        public double CastTime { get; set; }
        /// <summary>
        /// Haste adjusted cooldown at time of cast. 0 no CD.
        /// </summary>
        public double Cooldown { get; set; }
        public double Duration { get; set; }
        /// <summary>
        /// Haste adjusted GCD. 
        /// </summary>
        public double Gcd { get; set; }
        /// <summary>
        /// Actual mana cost value of this spell cast
        /// </summary>
        public double ManaCost { get; set; }
        /// <summary>
        /// The positive/negative mp5 averaged over a minute
        /// </summary>
        public double Mp5 { get; set; }

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

        public double RawHPCT { get => CalcRawHPCT(); }
        public double RawHPM { get => CalcRawHPM(); }
        public double RawHPS { get => CalcRawHPS(); }
        public double HPCT { get => CalcHPCT(); }
        public double HPM { get => CalcHPM(); }
        public double HPS { get => CalcHPS(); }
        /// <summary>
        /// Overhealing per second
        /// </summary>
        public double OPS { get => CalcOPS(); }
        public double MPS { get => CalcMPS(); }
        public double DPS { get => CalcDPS(); }
        public double DPM { get => CalcDPM(); }

        private double CalcRawHPCT()
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

        private double CalcRawHPM()
        {
            if (ManaCost > 0)
                return RawHealing / ManaCost;
            return 0;
        }

        private double CalcRawHPS()
        {
            return RawHealing * CastsPerMinute / 60;
        }

        private double CalcHPCT()
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

        private double CalcHPM()
        {
            if (ManaCost > 0)
                return Healing / ManaCost;
            return 0;
        }

        private double CalcHPS()
        {
            return Healing * CastsPerMinute / 60;
        }

        private double CalcMPS()
        {
            return (ManaCost * CastsPerMinute / 60) + (Mp5 / 5);
        }

        private double CalcDPS()
        {
            return Damage * CastsPerMinute / 60;
        }

        private double CalcDPM()
        {
            if (ManaCost > 0)
                return Damage / ManaCost;
            return 0;
        }

        private double CalcOPS()
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
