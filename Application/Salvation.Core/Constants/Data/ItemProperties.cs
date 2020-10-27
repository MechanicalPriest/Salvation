using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Constants.Data
{
    // From SimcProfileParser.Model.RawData & simc/data_enums.hh
    public enum InventorySlot
    {
        INVTYPE_NON_EQUIP = 0,
        INVTYPE_HEAD = 1,
        INVTYPE_NECK = 2,
        INVTYPE_SHOULDERS = 3,
        INVTYPE_BODY = 4,
        INVTYPE_CHEST = 5,
        INVTYPE_WAIST = 6,
        INVTYPE_LEGS = 7,
        INVTYPE_FEET = 8,
        INVTYPE_WRISTS = 9,
        INVTYPE_HANDS = 10,
        INVTYPE_FINGER = 11,
        INVTYPE_TRINKET = 12,
        INVTYPE_WEAPON = 13,
        INVTYPE_SHIELD = 14,
        INVTYPE_RANGED = 15,
        INVTYPE_CLOAK = 16,
        INVTYPE_2HWEAPON = 17,
        INVTYPE_BAG = 18,
        INVTYPE_TABARD = 19,
        INVTYPE_ROBE = 20,
        INVTYPE_WEAPONMAINHAND = 21,
        INVTYPE_WEAPONOFFHAND = 22,
        INVTYPE_HOLDABLE = 23,
        INVTYPE_AMMO = 24,
        INVTYPE_THROWN = 25,
        INVTYPE_RANGEDRIGHT = 26,
        INVTYPE_QUIVER = 27,
        INVTYPE_RELIC = 28,
        INVTYPE_MAX = 29
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
