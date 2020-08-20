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

    public class BaseModel
    {
        // Store the configuration data for easy access by the model
        internal GlobalConstants Constants { get; private set; }
        protected virtual BaseProfile Profile { get; private set; }
        protected BaseSpec SpecConstants { get; private set; }

        // Set the spec for this model
        protected Spec Spec = Spec.None;

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
        internal int RawMana { get { return getRawMana(); } }

        protected BaseModel(GlobalConstants constants, BaseProfile profile, Spec spec = Spec.None)
        {
            Constants = constants;
            Profile = profile;
            Spec = spec;

            if (Spec == Spec.None)
                throw new Exception($"Spec must be set for the model.");

            var foundSpec = Constants.Specs.Where(s => s.SpecId == (int)Spec).FirstOrDefault();

            if (foundSpec == null)
                throw new Exception($"Unable to find spec constants for SpecID: {(int)Spec}");
            else
                SpecConstants = foundSpec;

            Spells = new List<BaseSpell>();
        }

        internal Constants.BaseSpellData GetSpellById(int spellId)
        {
            Constants.BaseSpellData spell = SpecConstants.Spells.Where(s => s.Id == spellId).FirstOrDefault();

            return spell;
        }

        internal decimal GetVersMultiplier(int versRating)
        {
            return 1 + SpecConstants.VersBase + (versRating / SpecConstants.VersCost / 100);
        }

        internal decimal GetHasteMultiplier(int hasteRating)
        {
            return 1 + SpecConstants.HasteBase + (hasteRating / SpecConstants.HasteCost / 100);
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

        private int getRawMana()
        {
            return SpecConstants.ManaBase;
        }
    }
}
