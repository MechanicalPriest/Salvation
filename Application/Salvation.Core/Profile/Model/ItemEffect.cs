using Salvation.Core.Constants;

namespace Salvation.Core.Profile.Model
{
    public class ItemEffect
    {
        public uint EffectId { get; set; }
        public int Type { get; set; }
        public int CooldownGroup { get; set; }
        public int CooldownDuration { get; set; }
        public int CooldownGroupDuration { get; set; }
        public BaseSpellData Spell { get; set; }
    }
}