using System.Collections.Generic;

namespace Salvation.Core.Constants
{
    public class GlobalConstants
    {
        public string GameVersion { get; set; }
        public List<BaseSpec> Specs { get; set; }
        public List<BaseModifier> GlobalModifiers { get; set; }
        public List<BaseSpellData> SharedSpells { get; set; }

        public GlobalConstants()
        {
            Specs = new List<BaseSpec>();
            GlobalModifiers = new List<BaseModifier>();
        }
    }
}
