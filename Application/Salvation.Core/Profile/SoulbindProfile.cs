using Salvation.Core.Constants.Data;
using System.Collections.Generic;

namespace Salvation.Core.Profile
{
    public class SoulbindProfile
    {
        public string Name { get; set; }
        public int SoulbindId { get; set; }
        public bool IsActive { get; set; }
        public List<Soulbind> ActiveSoulbinds { get; set; }
        public Dictionary<Conduit, int> ActiveConduits { get; set; }

        public SoulbindProfile()
        {
            ActiveConduits = new Dictionary<Conduit, int>();
            ActiveSoulbinds = new List<Soulbind>();
        }
    }
}