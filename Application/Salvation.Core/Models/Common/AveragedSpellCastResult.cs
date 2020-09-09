using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Salvation.Core.Models.Common
{
    public class AveragedSpellCastResult
        : SpellCastResult
    {
        public decimal NumberOfTargets { get; set; }
        public decimal CastsPerMinute { get; set; }
        public decimal MaximumCastsPerMinute { get; set; }


        public AveragedSpellCastResult()
            : base()
        {

        }

        #region Calculated Fields

        public decimal RawHPCT { get => calcRawHPCT(); }
        public decimal RawHPM { get => calcRawHPM(); }
        public decimal RawHPS { get => calcRawHPS(); }
        public decimal HPCT { get => calcHPCT(); }
        public decimal HPM { get => calcHPM(); }
        public decimal HPS { get => calcHPS(); }
        public decimal MPS { get => calcMPS(); }

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


        #endregion

        public void MakeSpellHaveNoCasts()
        {
            CastsPerMinute = 0;
            MaximumCastsPerMinute = 0;
        }
    }
}
