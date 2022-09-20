namespace Salvation.Core.Constants.Data
{
    public enum Spell
    {
        None = 0,
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
        /// <summary>
        /// Priest aura, misc stuff
        /// </summary>
        Priest = 137030,
        LeechHeal = 143924,

        // Base healing spells
        Heal = 2060,
        FlashHeal = 2061,
        PrayerOfHealing = 596,
        HolyNova = 132157,
        HolyNovaRank2 = 322112,
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
        HaloHeal = 120692,
        HaloDamage = 120696,
        DivineStarHeal = 110745,
        DivineStarDamage = 122128,
        EchoOfLight = 77485,
        GuardianSpirit = 47788,

        // Talents
        Enlightenment = 193155, // T15
        TrailOfLight = 200128, // T15
        RenewedFaith = 341997, // T15

        AngelsMercy = 238100, // T25
        BodyAndSoul = 64129, // T25
        AngelicFeather = 121536, // T25

        CosmicRipple = 238136, // T30
        GuardianAngel = 200209, // T30
        Afterlife = 196707, // T30

        PsychicVoice = 196704, // T35
        Censure = 200199, // T35
        ShiningForce = 204263, //T35

        SurgeOfLight = 109186, // T40
        BindingHeal = 32546, // T40
        PrayerCircle = 321377, // T40

        Benediction = 193157, // T45
        DivineStar = 110744, // T45
        Halo = 120517, // T45

        LightOfTheNaaru = 196985, // T50
        Apotheosis = 200183, // T50
        HolyWordSalvation = 265202, // T50

        // Cov abilities
        Mindgames = 323673,
        MindgamesHeal = 323706,

        // Legendaries
        HarmoniousApparatus = 336314,
        DivineImage = 336400,
        DivineImageHealingLight = 196809,
        DivineImageDazzlingLight = 196810,
        DivineImageSearingLight = 196810,
        DivineImageLightEruption = 196812,
        DivineImageBlessedLight = 196813,
        DivineImageTranquilLight = 196816,

        // DPS
        Smite = 585,
        HolyWordChastise = 88625,
        ShadowWordPain = 589,
        ShadowWordDeath = 32379,
        HolyFire = 14914,

        // Consumables

        // Items

        // Traits
    }
}
