using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using System.Collections.Generic;

namespace Salvation.Core.Profile.Model
{
    public class RegisteredSpell
    {
        public Spell Spell { get; set; }
        public ISpellService SpellService { get; set; }
        /// <summary>
        /// Used to store the scale value for item-scaled spell effects
        /// </summary>
        public BaseSpellData SpellData { get; set; }
        public int ItemLevel { get; set; }

        /// <summary>
        /// Scale multiplier used for scaling item-scaled spells
        /// </summary>
        public Dictionary<int, double> ScaleValues { get; set; }

        public RegisteredSpell()
        {
            ScaleValues = new Dictionary<int, double>();
        }

        public RegisteredSpell(Spell spell)
            : this()
        {
            Spell = spell;
        }

        public override string ToString()
        {
            return $"{Spell} (ilvl: {ItemLevel})";
        }
    }
}