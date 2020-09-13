using Salvation.Core.Constants;
using Salvation.Core.Models.Common;
using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models
{
    public class BaseSpell
    {
        protected BaseModel model;

        private BaseSpellData spellData;
        protected BaseSpellData SpellData
        {
            get { return spellData; }
            set
            {
                spellData = value;
                postSpellDataSetup();
            }
        }

        // calculated fields

        protected virtual decimal HastedCastTime { get { return getHastedCastTime(); } }
        protected virtual decimal HastedGcd { get { return getHastedGcd(); } }
        protected virtual decimal HastedCooldown { get { return getHastedCooldown(); } }
        protected virtual decimal ActualManaCost { get { return getActualManaCost(); } }
        protected virtual decimal CastsPerMinute { get { return calcCastsPerMinute(); } }
        protected virtual decimal MaximumCastsPerMinute { get { return calcMaximumCastsPerMinute(); } }
        protected virtual CastProfile CastProfile { get; private set; }
        // 'static' fields
        public int SpellId { get; set; }
        protected string Name { get; set; }

        public BaseSpell (BaseModel baseModel, BaseSpellData spellData)
        {
            model = baseModel;
            if(spellData != null)
                SpellData = spellData;
        }

        /// <summary>
        /// This is run after SpellData is populated and can be used to do any additional setup
        /// </summary>
        protected virtual void postSpellDataSetup()
        {
            SpellId = spellData.Id;
            Name = spellData.Name;

            CastProfile = model.GetCastProfile(SpellId);
        }


        public virtual AveragedSpellCastResult CastAverageSpell()
        {
            AveragedSpellCastResult result = new AveragedSpellCastResult();

            result.CastsPerMinute = CastsPerMinute;
            result.CastTime = HastedCastTime;
            result.Cooldown = HastedCooldown;
            result.Duration = spellData.Duration;
            result.Gcd = HastedGcd;
            result.ManaCost = ActualManaCost;
            result.SpellId = SpellId;
            result.SpellName = Name;
            result.NumberOfHealingTargets = SpellData.NumberOfHealingTargets;
            result.NumberOfDamageTargets = SpellData.NumberOfDamageTargets;
            result.MaximumCastsPerMinute = MaximumCastsPerMinute;

            return result;
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

        protected virtual decimal getHastedCooldown()
        {
            return SpellData.IsCooldownHasted 
                ? SpellData.BaseCooldown / model.GetHasteMultiplier(model.RawHaste) 
                : SpellData.BaseCooldown;
        }

        protected virtual decimal getActualManaCost()
        {
            return model.RawMana * SpellData.ManaCost;
        }

        /// <summary>
        /// Override this to calculate the Casts per minute of the spell
        /// </summary>
        /// <returns></returns>
        protected virtual decimal calcCastsPerMinute()
        {
            return 0m;
        }

        // <summary>
        /// Override this to calculate the Maximum Casts per minute of the spell
        /// </summary>
        /// <returns></returns>
        protected virtual decimal calcMaximumCastsPerMinute()
        {
            return 0m;
        }
    }
}
