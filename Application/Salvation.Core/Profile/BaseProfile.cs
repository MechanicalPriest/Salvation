using Salvation.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Salvation.Core.Profile
{
    public enum Talent
    {
        Enlightenment = 193155, // T15
    }

    public enum Covenant
    {
        None = 0,
        Kyrian = 1,
        Venthyr = 2,
        NightFae = 3,
        Necrolord = 4,
    }

    /// <summary>
    /// TODO: Automatically populate this from covenant_data.inc
    /// You can replace `^.*(\d{6}).+"([^"]{1,})".*$` with `$2 = $1,` to speed the process up for now.
    /// </summary>
    public enum Soulbind
    {
        FieldofBlossoms = 319191,
        SocialButterfly = 319210,
        SoothingVoice = 319211,
        EmpoweredChrysalis = 319213,
        FaerieDust = 319214,
        Somnambulist = 319216,
        Podtender = 319217,
        BuiltforWar = 319973,
        EnduringGloom = 319978,
        MoveAsOne = 319982,
        WastelandPropriety = 319983,
        StayontheMove = 320658,
        NiyasToolsBurrs = 320659,
        NiyasToolsPoison = 320660,
        NiyasToolsHerbs = 320662,
        NaturesSplendor = 320668,
        SwiftPatrol = 320687,
        GroveInvigoration = 322721,
        VolatileSolvent = 323074,
        KevinsKeyring = 323079,
        PlaguebornCleansingSlime = 323081,
        TravelwithBloop = 323089,
        PlagueysPreemptiveStrike = 323090,
        OozsFrictionlessCoating = 323091,
        UltimateForm = 323095,
        SulfuricEmission = 323916,
        GristledToes = 323918,
        GnashingChompers = 323919,
        EmenisMagnificentSkin = 323921,
        CartilaginousLegs = 324440,
        HearthKidneystone = 324441,
        WildHuntsCharge = 325065,
        WildHuntTactics = 325066,
        HornoftheWildHunt = 325067,
        FaceYourFoes = 325068,
        FirstStrike = 325069,
        VorkaiSharpeningTechniques = 325072,
        GetInFormation = 325073,
        HoldtheLine = 325601,
        SerratedSpaulders = 326504,
        ResourcefulFleshcrafting = 326507,
        HeirmirsArsenalRavenousPendant = 326509,
        HeirmirsArsenalGorestompers = 326511,
        RuneforgedSpurs = 326512,
        BonesmithsSatchel = 326513,
        ForgeborneReveries = 326514,
        HeirmirsArsenalMarrowedGemstone = 326572,
        LetGoofthePast = 328257,
        EverForward = 328258,
        FocusingMantra = 328261,
        CleansedVestments = 328263,
        BondofFriendship = 328265,
        CombatMeditation = 328266,
        AscendantPhial = 329776,
        PhialofPatience = 329777,
        PointedCourage = 329778,
        BearersPursuit = 329779,
        ResonantAccolades = 329781,
        CleansingRites = 329784,
        RoadofTrials = 329786,
        ValiantStrikes = 329791,
        AgentofChaos = 331576,
        FancyFootwork = 331577,
        FriendsinLowPlaces = 331579,
        ExactingPreparation = 331580,
        FamiliarPredicaments = 331582,
        DauntlessDuelist = 331584,
        ThrillSeeker = 331586,
        ForgeliteFilter = 331609,
        ChargedAdditive = 331610,
        SoulsteelClamps = 331611,
        SparklingDriftglobeCore = 331612,
        ResilientPlumage = 331725,
        RegeneratingMaterials = 331726,
        SuperiorTactics = 332753,
        HoldYourGround = 332754,
        UnbreakableBody = 332755,
        ExpeditionLeader = 332756,
        HammerofGenesis = 333935,
        BronsCalltoAction = 333950,
        Mentorship = 334066,
        WatchtheShoes = 336140,
        LeisurelyGait = 336147,
        ExquisiteIngredients = 336184,
        SoothingShade = 336239,
        RefinedPalate = 336243,
        TokenofAppreciation = 336245,
        LifeoftheParty = 336247,
        ServiceInStone = 340159,
        EmenisAmbulatoryFlesh = 341650,
        EmbodytheConstruct = 342156,
        RunWithoutTiring = 342270,
    }

    public enum Conduit
    {
        // Endurance
        CharitableSoul = 337715,
        LightsInspiration = 337748,
        TranslucentImage = 337662,
        // Finesse
        ClearMind = 337707,
        MentalRecovery = 337954,
        MoveWithGrace = 337678,
        PowerUntoOthers = 337762,
        // Potency - Covenant
        FaeFermata = 338305,
        CourageousAscension = 337966,
        FesteringTransfusion = 337979,
        ShatteredPerceptions = 338315,
        // Potency - Spec Holy Priest
        FocusedMending = 337914,
        HolyOration = 338345,
        LastingSpirit = 337811,
        ResonantWords = 337947,
    }

    public class BaseProfile
    {
        public Spec SpecId { get; set; }
        public string Name { get; set; }
        public int Intellect { get; set; }
        public int MasteryRating { get; set; }
        public int VersatilityRating { get; set; }
        public int HasteRating { get; set; }
        public int CritRating { get; set; }
        /// <summary>
        /// Data containing cast effiency and overheal
        /// </summary>
        public List<CastProfile> Casts { get; set; }
        /// <summary>
        /// Active talents
        /// </summary>
        public List<Talent> Talents { get; set; }
        /// <summary>
        /// Active conduits. Conduit:rank
        /// </summary>
        public Dictionary<Conduit, int> Conduits { get; set; }

        public Covenant Covenant { get; set; }

        // Misc info
        public int FightLengthSeconds { get; set; }

        public BaseProfile()
        {
            Casts = new List<CastProfile>();
            Talents = new List<Talent>();
            Covenant = Covenant.None;
            Conduits = new Dictionary<Conduit, int>();
        }

        public bool IsTalentActive(Talent talent)
        {
            var exists = Talents.Contains(talent);

            return exists;
        }

        public bool IsConduitActive(Conduit conduit)
        {
            var exists = Conduits.Keys.Contains(conduit);

            return exists;
        }
    }
}
