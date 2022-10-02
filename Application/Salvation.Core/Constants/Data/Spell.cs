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
        PowerWordShield = 17,
        HaloHeal = 120692,
        HaloDamage = 120696,
        DivineStarHeal = 110745,
        DivineStarDamage = 122128,
        EchoOfLight = 77485,

        // Base damage spells
        Smite = 585,
        ShadowWordPain = 589,
        HolyFire = 14914,

        // Talents - Class
        Renew = 139,
        DispelMagic = 528,
        Shadowfiend = 34433,
        /// <summary>
        /// The PoM spell cast
        /// </summary>
        PrayerOfMending = 33076,
        ImprovedFlashHeal = 393870,
        ImprovedPurify = 390632,
        PsychicVoice = 196704,
        ShadowWordDeath = 32379,
        FocusedMending = 372354,
        HolyNova = 132157,
        ProtectiveLight = 193063,
        FromDarknessComesLight = 390615,
        AngelicFeather = 121536,
        Phantasm = 108942,
        DeathAndMadness = 321291,
        SpellWarding = 390667,
        BlessedRecovery = 390767,
        Rhapsody = 390622,
        LeapOfFaith = 73325,
        ShackleUndead = 9484,
        SheerTerror = 390919,
        VoidTendrils = 108920,
        MindControl = 605,
        DominateMind = 205364,
        WordsOfThePious = 377438,
        MassDispel = 32375,
        MoveWithGrace = 390620,
        PowerInfusion = 10060,
        VampiricEmbrace = 15286,
        TitheEvasion = 373223,
        Inspiration = 390676,
        ImprovedMassDispel = 341167,
        BodyAndSoul = 64129,
        TwinsOfTheSunPriestess = 373466,
        VoidShield = 280749,
        Sanlayn = 199855,
        Apathy = 390668,
        UnwaveringWill = 373456,
        TwistOfFate = 390972,
        ThroesOfPain = 377422,
        AngelsMercy = 238100,
        BindingHeals = 368275,
        DivineStar = 110744,
        Halo = 120517,
        TranslucentImage = 373446,
        Mindgames = 375901,
        SurgeOfLight = 109186,
        LightsInspiration = 373450,
        CrystallineReflection = 373457,
        ImprovedFade = 390670,
        Manipulation = 390996,
        PowerWordLife = 373481,
        AngelicBulwark = 108945,
        VoidShift = 108968,
        ShatteredPerceptions = 391112,

        // Talents - Holy
        HolyWordSerenity = 2050,
        PrayerOfHealing = 596,
        GuardianSpirit = 47788,
        HolyWordChastise = 88625,
        HolyWordSanctify = 34861,
        GuardianAngel = 200209,
        GuardiansOfTheLight = 196437,
        Censure = 200199,
        BurningVehemence = 372307,
        CircleOfHealing = 204883,
        RevitalizingPrayers = 391208,
        SanctifiedPrayers = 196489,
        CosmicRipple = 238136,
        Afterlife = 196707,
        RenewedFaith = 341997,
        EmpyrealBlaze = 372616,
        PrayerCircle = 321377,
        HealingChorus = 390881,
        PrayerfulLitany = 391209,
        TrailOfLight = 200128,
        DivineHymn = 64843,
        Enlightenment = 193155,
        Benediction = 193157,
        HolyMending = 391154,
        Orison = 390947,
        EverlastingLight = 391161,
        GalesOfSong = 372370,
        SymbolOfHope = 64901,
        DivineService = 391233,
        CrisisManagement = 390954,
        PrismaticEchoes = 390967,
        PrayersOfTheVirtuous = 390977,
        Pontifex = 390980,
        Apotheosis = 200183,
        HolyWordSalvation = 265202,
        EmpoweredRenew = 391339,
        RapidRecovery = 391368,
        SayYourPrayers = 391186,
        ResonantWords = 372309,
        DesperateTimes = 391381,
        LightOfTheNaaru = 196985,
        HarmoniousApparatus = 390994,
        SearingLight = 372611,
        AnsweredPrayers = 391387,
        Lightweaver = 390992,
        Lightwell = 372835,
        DivineImage = 392988,
        DivineWord = 372760,
        MiracleWorker = 235587,
        Restitution = 391124,

        // Talents - Complimentary Spells
        /// <summary>
        /// The PoM spell that heals people
        /// </summary>
        PrayerOfMendingHeal = 33110,
        /// <summary>
        /// The PoM buff people get when it is cast on/bounces to them
        /// </summary>
        PrayerOfMendingBuff = 41635,
        // TODO: Pull this from simc spelldata
        //MindgamesHeal = 375904, 
        DivineImageHealingLight = 196809,
        DivineImageDazzlingLight = 196810,
        // TODO: Pull this from simc, it's actually 196811
        DivineImageSearingLight = 196810, 
        DivineImageLightEruption = 196812,
        DivineImageBlessedLight = 196813,
        DivineImageTranquilLight = 196816,

        // Consumables

        // Items

        // Traits
    }
}
