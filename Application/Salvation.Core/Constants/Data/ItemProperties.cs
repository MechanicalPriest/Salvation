namespace Salvation.Core.Constants.Data
{
    // From SimcProfileParser.Model.RawData & simc/data_enums.hh
    public enum InventorySlot
    {
        NonEquip = 0,
        Head = 1,
        Neck = 2,
        Shoulder = 3,
        Body = 4,
        Chest = 5,
        Waist = 6,
        Legs = 7,
        Feet = 8,
        Wrists = 9,
        Hands = 10,
        Finger = 11,
        Trinket = 12,
        Weapon = 13,
        Shield = 14,
        Ranged = 15,
        Cloak = 16,
        TwoHandedWeapon = 17,
        Bag = 18,
        Tabard = 19,
        Robe = 20,
        WeaponMainHand = 21,
        WeaponOffHand = 22,
        Holdable = 23,
        Ammo = 24,
        Thrown = 25,
        RangedRight = 26,
        Quiver = 27,
        Relic = 28,
        Max = 29
    };

    public enum ItemType
    {
        ITEM_CLASS_CONSUMABLE = 0,
        ITEM_CLASS_CONTAINER = 1,
        ITEM_CLASS_WEAPON = 2,
        ITEM_CLASS_GEM = 3,
        ITEM_CLASS_ARMOR = 4,
        ITEM_CLASS_REAGENT = 5,
        ITEM_CLASS_PROJECTILE = 6,
        ITEM_CLASS_TRADE_GOODS = 7,
        ITEM_CLASS_GENERIC = 8,
        ITEM_CLASS_RECIPE = 9,
        ITEM_CLASS_MONEY = 10,
        ITEM_CLASS_QUIVER = 11,
        ITEM_CLASS_QUEST = 12,
        ITEM_CLASS_KEY = 13,
        ITEM_CLASS_PERMANENT = 14,
        ITEM_CLASS_MISC = 15,
        ITEM_CLASS_GLYPH = 16
    };
}
