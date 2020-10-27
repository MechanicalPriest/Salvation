using System.Collections.Generic;

namespace Salvation.Core.Profile
{
    public class Item
    {
        public uint ItemId { get; set; }
        public string Name { get; set; }
        public int ItemLevel { get; set; }

        public List<ItemMod> Mods { get; set; }
        public List<ItemGem> Gems { get; set; }
        public List<ItemEffect> Effects { get; set; }

        public Item()
        {
            Mods = new List<ItemMod>();
            Gems = new List<ItemGem>();
            Effects = new List<ItemEffect>();
        }
    }
}
