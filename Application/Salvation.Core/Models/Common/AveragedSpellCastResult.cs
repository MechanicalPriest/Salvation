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
            return RawHealing / Gcd;
        }

        private decimal calcRawHPM()
        {
            return RawHealing / ManaCost;
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
            return Healing / Gcd;
        }

        private decimal calcHPM()
        {
            return Healing / ManaCost;
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
    }
}
