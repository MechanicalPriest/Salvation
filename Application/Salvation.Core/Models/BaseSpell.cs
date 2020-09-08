using Salvation.Core.Constants;
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

        // Used to temporarily store the number of targets this spell should hit
        private decimal requestedNumberOfTargetsHit;

        // calculated fields

        public virtual decimal HastedCastTime { get { return getHastedCastTime(); } }
        public virtual decimal HastedGcd { get { return getHastedGcd(); } }
        public virtual decimal HastedCooldown { get { return getHastedCooldown(); } }
        public virtual decimal ActualManaCost { get { return getActualManaCost(); } }
        public virtual decimal NumberOfTargets { get; protected set; }
        public virtual decimal CastsPerMinute { get { return calcCastsPerMinute(); } }
        public virtual CastProfile CastProfile { get; private set; }
        // 'static' fields
        public int SpellId { get; set; }
        public string Name { get; set; }

        public BaseSpell (BaseModel baseModel, decimal numberOfTargetsHit)
        {
            model = baseModel;

            requestedNumberOfTargetsHit = numberOfTargetsHit;
        }

        /// <summary>
        /// This is run after SpellData is populated and can be used to do any additional setup
        /// </summary>
        protected virtual void postSpellDataSetup()
        {
            SpellId = spellData.Id;
            Name = spellData.Name;

            // If a request to override number of targets is present, override it.
            NumberOfTargets = requestedNumberOfTargetsHit > 0 ?
                requestedNumberOfTargetsHit :
                 SpellData.DefaultNumberOfTargets;

            CastProfile = model.GetCastProfile(SpellId);
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
    }
}
