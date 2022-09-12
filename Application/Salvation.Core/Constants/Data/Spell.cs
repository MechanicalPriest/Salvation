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
        Halo = 120517,
        HaloHeal = 120692,
        HaloDamage = 120696,
        DivineStar = 110744,
        DivineStarHeal = 110745,
        DivineStarDamage = 122128,
        HolyWordSalvation = 265202,
        EchoOfLight = 77485,
        GuardianSpirit = 47788,

        // Talents
        Enlightenment = 193155,
        CosmicRipple = 238136,
        Benediction = 193157,

        // Cov abilities
        Mindgames = 323673,
        MindgamesHeal = 323706,
        FaeGuardians = 327661,
        BenevolentFaerie = 327710,
        GuardianFaerie = 327694,
        BoonOfTheAscended = 325013,
        AscendedNova = 325020,
        AscendedBlast = 325283,
        AscendedBlastHeal = 325315,
        AscendedEruption = 325326,
        UnholyNova = 324724,
        UnholyTransfusion = 325118,
        Fleshcraft = 324631,
        /// <summary>
        /// The enemy debuff applied that does damage
        /// </summary>
        UnholyTransfusionDoT = 325203,

        // Legendaries
        HarmoniousApparatus = 336314,
        EchoOfEonar = 338477,
        EchoOfEonarHealingBuffSelf = 347660,
        EchoOfEonarDamageBuffSelf = 338489,
        EchoOfEonarHealingBuffOther = 347663, // TODO: This needs to be whitelisted in simc.
        CauterizingShadows = 336370,
        CauterizingShadowsHeal = 336373,
        FlashConcentration = 336266,
        DivineImage = 336400,
        DivineImageHealingLight = 196809,
        DivineImageDazzlingLight = 196810,
        DivineImageSearingLight = 196810,
        DivineImageLightEruption = 196812,
        DivineImageBlessedLight = 196813,
        DivineImageTranquilLight = 196816,

        // Conduits
        CharitableSoul = 337715,
        CourageousAscension = 337966,
        FesteringTransfusion = 337979,
        FaeFermata = 338305,
        ShatteredPerceptions = 338315,
        HolyOration = 338345,
        FocusedMending = 337914,
        ResonantWords = 337947,
        LastingSpirit = 337811,

        // DPS
        Smite = 585,
        HolyWordChastise = 88625,
        ShadowWordPain = 589,
        ShadowWordPainRank2 = 327820,
        ShadowWordDeath = 32379,
        ShadowWordDeathRank2 = 322107,
        HolyFire = 14914,

        // Consumables
        SpectralFlaskOfPower = 307185,
        SpiritualManaPotion = 307193,

        // Items
        /// <summary>
        /// Trinket 178708
        /// </summary>
        UnboundChangeling = 330733,
        UnboundChangelingBuff = 330747,
        CabalistsHymnal = 344806,
        CabalistsHymnalBuff = 344803,
        SoullettingRuby = 345801,
        SoullettingRubyBuff = 345805,
        SoullettingRubyHeal = 345806,
        SoullettingRubyTrigger = 345807,
        ManaboundMirror = 344243,
        ManaboundMirrorBuff = 344244,
        ManaboundMirrorHeal = 344245,
        MacabreSheetMusic = 345432,
        MacabreSheetMusicTrigger = 345431,
        MacabreSheetMusicBuff = 345439,
        TuftOfSmolderingPlumage = 344916,
        TuftOfSmolderingPlumageBuff = 344915,
        TuftOfSmolderingPlumageHeal = 344917,
        ConsumptiveInfusion = 344221,
        ConsumptiveInfusionBuff = 344227,
        ConsumptiveInfusionDebuff = 344229,
        DarkmoonDeckRepose = 333734,
        DarkmoonDeckReposeAce = 311474,
        DarkmoonDeckReposeEight = 311481,
        VialOfSpectralEssence = 345695,
        OverflowingAnimaCage = 343385,
        OverflowingAnimaCageBuff = 343387,
        SiphoningPhylacteryShard = 345549,
        SiphoningPhylacteryShardBuff = 345551,

        // Traits
        // Kyrian - Pelagos
        CombatMeditation = 328266,
        CombatMeditationBuff = 328908,
        CombatMeditationExtension = 328913,
        LetGoOfThePast = 328257,
        LetGoOfThePastBuff = 328900,
        // Kyrian - Kleia
        PointedCourage = 329778,
        PointedCourageBuff = 330511,
        ValiantStrikes = 329791,
        ValiantStrikesBuff = 330943,
        ResonantAccolades = 329781,
        ResonantAccoladesBuff = 330859,
        // Kyrian - Mikanikos
        BronsCallToAction = 333950,
        // Necro - Marileth
        VolatileSolvent = 323074,
        UltimateForm = 323095,
        UltimateFormHeal = 323524,
        // Necro - Emeni
        LeadByExample = 342156,
        LeadByExampleBuff = 342181,
        // Necro - Bonesmith Heirmir
        ForgeborneReveries = 326514,
        ForgeborneReveriesBuff = 348272,
        MarrowedGemstone = 326572,
        MarrowedGemstoneStacks = 327066,
        MarrowedGemstoneBuff = 327069,
        MarrowedGemstoneCooldown = 327073,
        /// <summary>
        /// Mastery buff
        /// </summary>
        VolatileSolventHumanoid = 323491,
        /// <summary>
        /// Int buff
        /// </summary>
        VolatileSolventBeast = 323498,
        /// <summary>
        /// Crit buff
        /// </summary>
        VolatileSolventDragonkin = 323502,
        /// <summary>
        /// Magic damage
        /// </summary>
        VolatileSolventElemental = 323504,
        /// <summary>
        /// Magic damage taken reduced
        /// </summary>
        VolatileSolventMechanical = 323507,
        // NF - Niya
        GroveInvigoration = 322721,
        GroveInvigorationAnimaBuff = 342814,
        NiyasToolsHerbs = 320662,
        NiyasToolsHerbsBuff = 321510,
        // NF - Dreamweaver
        FieldOfBlossoms = 319191,
        FieldOfBlossomsEffect = 342761,
        FieldOfBlossomsBuff = 342774,
        // Venthyr - Nadjia
        ThrillSeeker = 331586,
        ThrillSeekerStacks = 331939,
        ThrillSeekerBuff = 331937,
        // Venthyr - Theotar
        SoothingShade = 336239,
        SoothingShadeBuff = 336885,
        SoothingShadeEffect = 336808,
    }
}
