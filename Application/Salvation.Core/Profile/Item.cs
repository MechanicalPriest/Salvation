using Salvation.Core.Constants.Data;
using System.Collections.Generic;

namespace Salvation.Core.Profile
{
    public class Item
    {
        public uint ItemId { get; set; }
        public string Name { get; set; }
        public int ItemLevel { get; set; }
        public bool Equipped { get; set; }

        public InventorySlot Slot { get; set; }
        /// <summary>
        /// This contains the type of item it is. Common types: 2 = weapon, 4 = armor
        /// </summary>
        public ItemType ItemType { get; set; }
        /// <summary>
        /// This contains information about what kind of sub-item it is.
        /// Example if the Class is Armor (4) and the SubClass is Cloth (1)
        /// </summary>
        public int ItemSubType { get; set; }

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
