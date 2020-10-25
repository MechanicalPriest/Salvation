﻿namespace Salvation.Core.Constants.Data
{
    public enum Spell
    {
        /// <summary>
        /// 179715 - Direct heal bonus
        /// 191076 - Periodic heal bonus
        /// 191077 - Direct damage bonus
        /// 191078 - Periodic damage bonus
        /// 191079 - SW:D damage reduction
        /// 869198 - Mana regen bonus
        /// 871790 - SW:P direct damage bonus
        /// 871791 - SW:P periodic damage bonus
        /// </summary>
        HolyPriest = 137031,

        // Base healing spells
        Heal = 2060,
        FlashHeal = 2061,
        PrayerOfHealing = 596,
        HolyNova = 132157,
        CircleOfHealing = 204883,
        Renew = 139,
        PowerWordShield = 17,
        DivineHymn = 64843,
        HolyWordSanctify = 34861,
        HolyWordSerenity = 2050,
        /// <summary>
        /// The PoM spell cast
        /// </summary>
        PrayerOfMending = 33076,
        /// <summary>
        /// The PoM spell that heals people
        /// </summary>
        PrayerOfMendingHeal = 33110,
        /// <summary>
        /// The PoM buff people get when it is cast on/bounces to them
        /// </summary>
        PrayerOfMendingBuff = 41635,
        /// <summary>
        /// Makes PoM instant cast
        /// </summary>
        PrayerOfMendingRank2 = 319912,
        BindingHeal = 32546,
        Halo = 120517,
        HaloHeal = 120692,
        HaloDamage = 120696,
        DivineStar = 110744,
        DivineStarHeal = 110745,
        DivineStarDamage = 122128,
        HolyWordSalvation = 265202,
        EchoOfLight = 77485,

        // Talents
        Enlightenment = 193155,
        CosmicRipple = 238136,
        Benediction = 193157,

        // Cov abilities
        Mindgames = 323673,
        FaeGuardians = 327661,
        BenevolentFaerie = 327710,
        GuardianFaerie = 327694,
        BoonOfTheAscended = 325013,
        AscendedNova = 325020,
        AscendedBlast = 325283,
        AscendedEruption = 325326,
        UnholyNova = 324724,
        UnholyTransfusion = 325118,
        /// <summary>
        /// The enemy debuff applied that does damage
        /// </summary>
        UnholyTransfusionDoT = 325203,

        // Legendaries
        HarmoniousApparatus = 336314,

        // Conduits
        CharitableSoul = 337715,
        CourageousAscension = 337966,
        FesteringTransfusion = 337979,
        FaeFermata = 338305,
        ShatteredPerceptions = 338315,
        HolyOration = 338345,

        // DPS
        Smite = 585,
        SmiteRank2 = 262861,
        Chastise = 88625,
        Pain = 589,
        PainRank2 = 327820,
    }
}
