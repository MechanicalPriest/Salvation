using Salvation.Core.Constants;
using Salvation.Core.Models.HolyPriest;
using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Salvation.Core.Models
{
    /// <summary>
    /// List of each of the spec IDs
    /// </summary>
    public enum Spec
    {
        None = 0,
        HolyPriest = 257
    }

    public enum ModelType
    {
        Average = 0,
        Sequence = 1
    }

    public class BaseModel
    {
        // Store the configuration data for easy access by the model
        internal GlobalConstants Constants { get; private set; }
        protected virtual BaseProfile Profile { get; private set; }
        protected BaseSpec SpecConstants { get; private set; }

        // Set the spec for this model
        public Spec Spec { get; private set; }
        /// <summary>
        /// The type of model. This defines some core behaviour about how results are calculated
        /// </summary>
        public ModelType ModelType { get; private set; }

        public List<BaseSpell> Spells { get; private set; }


        // For each stat we have multiple values we care about:
        // Raw stat (with gear on, no buffs)
        // Buffed stat (with static buffs from other classes / flask etc
        // Average stat (average stat value including all potential buffs/procs averaged out)
        // A way to calculating the multiplier from rating

        /// <summary>
        /// Intellect rating (total)
        /// </summary>
        internal int RawInt { get { return getRawIntellect(); } }
        internal int RawVers { get { return getRawVers(); } }
        internal int RawHaste { get { return getRawHaste(); } }
        internal int RawMastery { get { return getRawMastery(); } }
        internal int RawCrit { get { return getRawCrit(); } }
        internal int RawMana { get { return getRawMana(); } }

        internal int FightLengthSeconds { get { return getFightLengthSeconds(); } }

        protected BaseModel(GlobalConstants constants, BaseProfile profile, Spec spec = Spec.None, ModelType modelType = ModelType.Average)
        {
            Constants = constants;
            Profile = profile;
            Spec = spec;
            ModelType = modelType;

            if (Spec == Spec.None)
                throw new Exception($"Spec must be set for the model.");

            var foundSpec = Constants.Specs.Where(s => s.SpecId == (int)Spec).FirstOrDefault();

            if (foundSpec == null)
                throw new Exception($"Unable to find spec constants for SpecID: {(int)Spec}");
            else
                SpecConstants = foundSpec;

            Spells = new List<BaseSpell>();
        }

        internal Constants.BaseSpellData GetSpellDataById(int spellId)
        {
            Constants.BaseSpellData spell = SpecConstants.Spells.Where(s => s.Id == spellId).FirstOrDefault();

            return spell;
        }

        /// <summary>
        /// Get a BaseSpell from the Spells list based on its SpellIds enum value.
        /// </summary>
        /// <typeparam name="T">The return type for convenience</typeparam>
        /// <param name="spell">The referenced spell to find</param>
        /// <returns>The BaseSpell cast to T or null if not found</returns>
        internal T GetSpell<T>(HolyPriestModel.SpellIds spell)
            where T : BaseSpell
        {
            var spellModel = Spells
                .Where(s => s.SpellId == (int)spell)
                .FirstOrDefault() as T;

            return spellModel;
        }

        /// <summary>
        /// Pull a cast profile from the base profile, based on spell ID
        /// </summary>
        /// <param name="spellId"></param>
        /// <returns></returns>
        internal CastProfile GetCastProfile(int spellId)
        {
            var castProfile = Profile.Casts?
                .Where(c => c.SpellId == spellId)
                .FirstOrDefault();

            return castProfile;
        }

        internal BaseModifier GetModifierbyName(string modifierName)
        {
            BaseModifier modifier = SpecConstants.Modifiers.Where(s => s.Name == modifierName).FirstOrDefault();

            return modifier;
        }

        internal decimal GetVersMultiplier(int versRating)
        {
            return 1 + SpecConstants.VersBase + (versRating / SpecConstants.VersCost / 100);
        }

        internal decimal GetHasteMultiplier(int hasteRating)
        {
            return 1 + SpecConstants.HasteBase + (hasteRating / SpecConstants.HasteCost / 100);
        }

        internal decimal GetMasteryMultiplier(int masteryRating)
        {
            return 1 + SpecConstants.MasteryBase + (masteryRating / SpecConstants.MasteryCost / 100);
        }

        internal decimal GetCritMultiplier(int critRating)
        {
            // TODO: This returns average crit. For models not being averaged...
            // it needs to return 2(?) or 1 depending on if RNG decides it crits or not.
            return 1 + SpecConstants.CritBase + (critRating / SpecConstants.CritCost / 100);

            // Pseudo code for TODO implementation
            //var statWeights = true;
            //var critPercent = SpecConstants.CritBase + (critRating / SpecConstants.CritCost / 100);
            //if (statWeights)
            //{
            //    return 1 + critPercent;
            //}
            //else
            //{
            //    return Random(0, 100) > critPercent ? GetCritMultiplier : 0;
            //}
        }

        private int getRawIntellect()
        {
            return Profile.Intellect;
        }

        private int getRawVers()
        {
            return Profile.VersatilityRating;
        }
        private int getRawHaste()
        {
            return Profile.HasteRating;
        }
        private int getRawMastery()
        {
            return Profile.MasteryRating;
        }
        private int getRawCrit()
        {
            return Profile.CritRating;
        }

        private int getRawMana()
        {
            return SpecConstants.ManaBase;
        }
        private int getFightLengthSeconds()
        {
            return Profile.FightLengthSeconds;
        }


    }
}
