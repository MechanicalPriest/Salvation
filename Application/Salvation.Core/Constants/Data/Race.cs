namespace Salvation.Core.Constants.Data
{
    public enum Race
    {
        // From util::race_id in util.cpp & race_e in sc_enums.hpp
        // _race_map in sc_spell_info.cpp contains the races and their IDs 0-indexed
        NoRace = 0,
        Human,
        Orc,
        Dwarf,
        NightElf,
        Undead,
        Tauren,
        Gnome,
        Troll,
        Goblin,
        BloodElf,
        Draenei,
        DarkIronDwarf,
        Vulpera,
        MagharOrc,
        Mechagnome,
        Vrykul,
        Tuskarr,
        ForestTroll,
        Tanuka,
        Skeleton,
        IceTroll,
        Worgen,
        Gilnean,
        Pandaren,
        PandarenAlliance,
        PandarenHorde,
        Nightborne,
        HighmountainTauren,
        VoidElf,
        LightforgedDraenei,
        ZandalariTroll,
        KulTiran,
    }

    // Todo; Move to SimcProfileParser as part of 
    // https://github.com/MechanicalPriest/SimcProfileParser/issues/62
    public static class RaceHelpers
    {
        public static Race ParseRace(string race)
        {
            return (race.ToLower()) switch
            {
                "human" => Race.Human,
                "orc" => Race.Orc,
                "dwarf" => Race.Dwarf,
                "night_elf" => Race.NightElf,
                "undead" => Race.Undead,
                "tauren" => Race.Tauren,
                "gnome" => Race.Gnome,
                "troll" => Race.Troll,
                "goblin" => Race.Goblin,
                "blood_elf" => Race.BloodElf,
                "draenei" => Race.Draenei,
                "dark_iron_dwarf" => Race.DarkIronDwarf,
                "vulpera" => Race.Vulpera,
                "maghar_orc" => Race.MagharOrc,
                "mechagnome" => Race.Mechagnome,
                "worgen" => Race.Worgen,
                "pandaren" => Race.Pandaren,
                "pandaren_alliance" => Race.PandarenAlliance,
                "pandaren_horde" => Race.PandarenHorde,
                "nightborne" => Race.Nightborne,
                "highmountain_tauren" => Race.HighmountainTauren,
                "void_elf" => Race.VoidElf,
                "lightforged_draenei" => Race.LightforgedDraenei,
                "zandalari_troll" => Race.ZandalariTroll,
                "kul_tiran" => Race.KulTiran,
                _ => Race.NoRace,
            };
        }
    }
}
