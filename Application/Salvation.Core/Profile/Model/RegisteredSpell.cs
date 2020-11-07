using Salvation.Core.Constants.Data;
using Salvation.Core.Modelling;

namespace Salvation.Core.Profile.Model
{
    public class RegisteredSpell
    {
        public Spell Spell { get; set; }
        public SpellService SpellService { get; set; }
    }
}