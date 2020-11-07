using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;

namespace Salvation.Core.Profile.Model
{
    public class RegisteredSpell
    {
        public Spell Spell { get; set; }
        public ISpellService SpellService { get; set; }
        /// <summary>
        /// Used to store the scale value for item-scaled spell effects
        /// </summary>
        public double ScaleValue { get; set; }
        public int ItemLevel { get; set; }
    }
}