using Salvation.Core.Constants.Data;
using System.Collections.Generic;

namespace Salvation.Core.Profile.Model
{
    public class SoulbindProfile
    {
        public string Name { get; set; }
        public int SoulbindId { get; set; }
        public bool IsActive { get; set; }
        public List<SoulbindTrait> ActiveTraits { get; set; }
        public Dictionary<Conduit, uint> ActiveConduits { get; set; }

        public SoulbindProfile()
        {
            ActiveConduits = new Dictionary<Conduit, uint>();
            ActiveTraits = new List<SoulbindTrait>();
        }
    }
}