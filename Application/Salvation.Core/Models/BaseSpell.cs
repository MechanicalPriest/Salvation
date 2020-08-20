using Salvation.Core.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models
{
    public class BaseSpell
    {
        protected BaseModel model;

        protected BaseSpellData SpellData { get; set; }
        // calculated fields

        protected virtual decimal HastedCastTime { get { return getHastedCastTime(); } }
        protected virtual decimal HastedGcd { get { return getHastedGcd(); } }
        protected virtual decimal ActualManaCost { get { return getActualManaCost(); } }

        public BaseSpell (BaseModel baseModel)
        {
            model = baseModel;
        }

        protected virtual decimal getHastedCastTime()
        {
            return SpellData.IsCastTimeHasted ? SpellData.BaseCastTime / model.GetHasteMultiplier(model.RawHaste) 
                : SpellData.BaseCastTime;
        }

        protected virtual decimal getHastedGcd()
        {
            return SpellData.Gcd / model.GetHasteMultiplier(model.RawHaste);
        }

        protected virtual decimal getActualManaCost()
        {
            return model.RawMana * SpellData.ManaCost;
        }
    }
}
