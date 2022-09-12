using Newtonsoft.Json;
using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile.Model;
using Salvation.Core.State;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Salvation.CoreTests.HolyPriest.Spells
{
    /// <summary>
    /// A mixture of integration / unit tests to ensure the methods return expected results 
    /// regardless of different configuration values
    /// </summary>
    [TestFixture]
    public class SpellServiceTests
    {
        private List<SpellService> Spells { get; set; }

        private GameState _gameState;

        [OneTimeSetUp]
        public void InitOnce()
        {
            Spells = new List<SpellService>();

            IGameStateService gameStateService = new GameStateService();
            IConstantsService constantsService = new ConstantsService();

            // Load this from somewhere that doesn't change
            var constants = constantsService.ParseConstants(
                File.ReadAllText(Path.Combine("TestData", "BaseTests_constants.json")));
            var profile = JsonConvert.DeserializeObject<PlayerProfile>(
                File.ReadAllText(Path.Combine("TestData", "SpellServiceTests_profile.json")));

            Spells.Add(new AscendedBlast(gameStateService));
            Spells.Add(new AscendedEruption(gameStateService));
            Spells.Add(new AscendedNova(gameStateService));
            Spells.Add(new AscendedBlast(gameStateService));
            Spells.Add(new AscendedEruption(gameStateService));
            Spells.Add(new AscendedNova(gameStateService));
            Spells.Add(new BoonOfTheAscended(gameStateService,
                new AscendedBlast(gameStateService),
                new AscendedNova(gameStateService),
                new AscendedEruption(gameStateService)));
            Spells.Add(new CircleOfHealing(gameStateService));
            Spells.Add(new DivineHymn(gameStateService));
            Spells.Add(new DivineStar(gameStateService));
            Spells.Add(new FaeGuardians(gameStateService,
                new DivineHymn(gameStateService)));
            Spells.Add(new FlashHeal(gameStateService));
            Spells.Add(new Halo(gameStateService));
            Spells.Add(new Heal(gameStateService));
            Spells.Add(new HolyNova(gameStateService));
            Spells.Add(new HolyWordSalvation(gameStateService,
                new HolyWordSerenity(gameStateService,
                    new FlashHeal(gameStateService),
                    new Heal(gameStateService),
                    new PrayerOfMending(gameStateService)),
                new HolyWordSanctify(gameStateService,
                    new PrayerOfHealing(gameStateService),
                    new Renew(gameStateService),
                    new CircleOfHealing(gameStateService)),
                new Renew(gameStateService),
                new PrayerOfMending(gameStateService)));
            Spells.Add(new HolyWordSanctify(gameStateService,
                new PrayerOfHealing(gameStateService),
                new Renew(gameStateService),
                new CircleOfHealing(gameStateService)));
            Spells.Add(new HolyWordSerenity(gameStateService,
                new FlashHeal(gameStateService),
                new Heal(gameStateService),
                new PrayerOfMending(gameStateService)));
            Spells.Add(new Mindgames(gameStateService));
            Spells.Add(new PowerWordShield(gameStateService));
            Spells.Add(new PrayerOfHealing(gameStateService));
            Spells.Add(new PrayerOfMending(gameStateService));
            Spells.Add(new Renew(gameStateService));
            Spells.Add(new SpellService(gameStateService));
            Spells.Add(new UnholyNova(gameStateService,
                new UnholyTransfusion(gameStateService)));
            Spells.Add(new UnholyTransfusion(gameStateService));
            Spells.Add(new Smite(gameStateService));
            Spells.Add(new HolyWordChastise(gameStateService, new Smite(gameStateService), new HolyFire(gameStateService)));
            Spells.Add(new ShadowWordPain(gameStateService));
            Spells.Add(new ShadowWordDeath(gameStateService));
            Spells.Add(new HolyFire(gameStateService));

            _gameState = gameStateService.CreateValidatedGameState(profile, constants);
        }

        [TestCaseSource(typeof(SpellServiceTestsData), nameof(SpellServiceTestsData.GetMaximumHealTargets))]
        public double GetMaximumHealTargets(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetMaximumHealTargets(_gameState, null);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellServiceTestsData), nameof(SpellServiceTestsData.GetMinimumHealTargets))]
        public double GetMinimumHealTargets(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetMinimumHealTargets(_gameState, null);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellServiceTestsData), nameof(SpellServiceTestsData.GetAverageRawHealing))]
        public double GetAverageRawHealing(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetAverageRawHealing(_gameState, null);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellServiceTestsData), nameof(SpellServiceTestsData.GetAverageDamage))]
        public double GetAverageDamage(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetAverageDamage(_gameState, null);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellServiceTestsData), nameof(SpellServiceTestsData.GetAverageHealing))]
        public double GetAverageHealing(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetAverageHealing(_gameState, null);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellServiceTestsData), nameof(SpellServiceTestsData.GetNumberOfHealingTargets))]
        public double GetNumberOfHealingTargets(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetNumberOfHealingTargets(_gameState, null);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellServiceTestsData), nameof(SpellServiceTestsData.GetDuration))]
        public double GetDuration(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetDuration(_gameState, null);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellServiceTestsData), nameof(SpellServiceTestsData.GetActualManaCost))]
        public double GetActualManaCost(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetActualManaCost(_gameState, null);
            result = Math.Round(result, 10);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellServiceTestsData), nameof(SpellServiceTestsData.GetHastedCastTime))]
        public double GetHastedCastTime(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetHastedCastTime(_gameState, null);
            result = Math.Round(result, 10);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellServiceTestsData), nameof(SpellServiceTestsData.GetHastedCooldown))]
        public double GetHastedCooldown(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetHastedCooldown(_gameState, null);
            result = Math.Round(result, 10);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellServiceTestsData), nameof(SpellServiceTestsData.GetMaximumCastsPerMinute))]
        public double GetMaximumCastsPerMinute(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetMaximumCastsPerMinute(_gameState, null);
            result = Math.Round(result, 10);

            // Assert
            return result;
        }


        [TestCaseSource(typeof(SpellServiceTestsData), nameof(SpellServiceTestsData.GetMaximumDamageTargets))]
        public double GetMaximumDamageTargets(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetMaximumDamageTargets(_gameState, null);
            result = Math.Round(result, 10);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellServiceTestsData), nameof(SpellServiceTestsData.GetMinimumDamageTargets))]
        public double GetMinimumDamageTargets(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetMinimumDamageTargets(_gameState, null);
            result = Math.Round(result, 10);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellServiceTestsData), nameof(SpellServiceTestsData.GetNumberOfDamageTargets))]
        public double GetNumberOfDamageTargets(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetNumberOfDamageTargets(_gameState, null);
            result = Math.Round(result, 10);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellServiceTestsData), nameof(SpellServiceTestsData.GetAverageOverhealing))]
        public double GetAverageOverhealing(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetAverageOverhealing(_gameState, null);
            result = Math.Round(result, 10);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellServiceTestsData), nameof(SpellServiceTestsData.TriggersMastery))]
        public bool TriggersMastery(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.TriggersMastery(_gameState, null);

            // Assert
            return result;
        }

    }

    public class SpellServiceTestsData
    {
        public static IEnumerable GetMaximumHealTargets
        {
            get
            {
                yield return new TestCaseData(typeof(AscendedBlast)).Returns(1);
                yield return new TestCaseData(typeof(AscendedEruption)).Returns(double.MaxValue);
                yield return new TestCaseData(typeof(AscendedNova)).Returns(6);
                yield return new TestCaseData(typeof(BoonOfTheAscended)).Returns(0);
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(5);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(double.MaxValue);
                yield return new TestCaseData(typeof(DivineStar)).Returns(double.MaxValue);
                yield return new TestCaseData(typeof(FaeGuardians)).Returns(1);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(1);
                yield return new TestCaseData(typeof(Halo)).Returns(double.MaxValue);
                yield return new TestCaseData(typeof(Heal)).Returns(1);
                yield return new TestCaseData(typeof(HolyNova)).Returns(double.MaxValue);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(double.MaxValue);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(6);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(1);
                yield return new TestCaseData(typeof(Mindgames)).Returns(1);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(1);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(5);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(1);
                yield return new TestCaseData(typeof(Renew)).Returns(1);
                yield return new TestCaseData(typeof(SpellService)).Returns(0);
                yield return new TestCaseData(typeof(UnholyNova)).Returns(6);
                yield return new TestCaseData(typeof(UnholyTransfusion)).Returns(1);
                yield return new TestCaseData(typeof(Smite)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(0);
                yield return new TestCaseData(typeof(HolyFire)).Returns(0);
            }
        }
        public static IEnumerable GetMinimumHealTargets
        {
            get
            {
                yield return new TestCaseData(typeof(AscendedBlast)).Returns(1);
                yield return new TestCaseData(typeof(AscendedEruption)).Returns(0);
                yield return new TestCaseData(typeof(AscendedNova)).Returns(1);
                yield return new TestCaseData(typeof(BoonOfTheAscended)).Returns(0);
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(1);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(1);
                yield return new TestCaseData(typeof(DivineStar)).Returns(1);
                yield return new TestCaseData(typeof(FaeGuardians)).Returns(1);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(1);
                yield return new TestCaseData(typeof(Halo)).Returns(1);
                yield return new TestCaseData(typeof(Heal)).Returns(1);
                yield return new TestCaseData(typeof(HolyNova)).Returns(1);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(1);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(1);
                yield return new TestCaseData(typeof(Mindgames)).Returns(0);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(1);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(1);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(1);
                yield return new TestCaseData(typeof(Renew)).Returns(1);
                yield return new TestCaseData(typeof(SpellService)).Returns(0);
                yield return new TestCaseData(typeof(UnholyNova)).Returns(1);
                yield return new TestCaseData(typeof(UnholyTransfusion)).Returns(0);
                yield return new TestCaseData(typeof(Smite)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(0);
                yield return new TestCaseData(typeof(HolyFire)).Returns(0);
            }
        }
        public static IEnumerable GetNumberOfHealingTargets
        {
            get
            {
                yield return new TestCaseData(typeof(AscendedBlast)).Returns(1);
                yield return new TestCaseData(typeof(AscendedEruption)).Returns(5);
                yield return new TestCaseData(typeof(AscendedNova)).Returns(5);
                yield return new TestCaseData(typeof(BoonOfTheAscended)).Returns(0);
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(5);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(20);
                yield return new TestCaseData(typeof(DivineStar)).Returns(6);
                yield return new TestCaseData(typeof(FaeGuardians)).Returns(1);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(1);
                yield return new TestCaseData(typeof(Halo)).Returns(6);
                yield return new TestCaseData(typeof(Heal)).Returns(1);
                yield return new TestCaseData(typeof(HolyNova)).Returns(20);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(20);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(6);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(1);
                yield return new TestCaseData(typeof(Mindgames)).Returns(1);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(1);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(5);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(1);
                yield return new TestCaseData(typeof(Renew)).Returns(1);
                yield return new TestCaseData(typeof(UnholyNova)).Returns(6);
                yield return new TestCaseData(typeof(UnholyTransfusion)).Returns(1);
                yield return new TestCaseData(typeof(Smite)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(0);
                yield return new TestCaseData(typeof(HolyFire)).Returns(0);
            }
        }
        public static IEnumerable GetAverageRawHealing
        {
            get
            {
                yield return new TestCaseData(typeof(AscendedBlast)).Returns(2996.6221326510004d);
                yield return new TestCaseData(typeof(AscendedEruption)).Returns(5364.2172212667338d);
                yield return new TestCaseData(typeof(AscendedNova)).Returns(1406.2360845960002d);
                yield return new TestCaseData(typeof(BoonOfTheAscended)).Returns(0);
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(6152.2828701075014d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(75936.748568184019d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(9843.6525921720022d);
                yield return new TestCaseData(typeof(FaeGuardians)).Returns(16000.0d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(2378.8827097749004d);
                yield return new TestCaseData(typeof(Halo)).Returns(10124.899809091203d);
                yield return new TestCaseData(typeof(Heal)).Returns(3456.9970412985012d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(2214.8218332387005d);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(25780.994884260013d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(17226.392036301004d);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(8203.0438268100024d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(9032.8628250000002d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(1841.4996345900001d);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(5126.9023917562517d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(3574.1833816814997d);
                yield return new TestCaseData(typeof(Renew)).Returns(2424.5214627022438d);
                yield return new TestCaseData(typeof(UnholyNova)).Returns(10546.770634470002d);
                yield return new TestCaseData(typeof(UnholyTransfusion)).Returns(234.37268076600006d);
                yield return new TestCaseData(typeof(Smite)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(0);
                yield return new TestCaseData(typeof(HolyFire)).Returns(0);
            }
        }
        public static IEnumerable GetAverageHealing
        {
            get
            {
                yield return new TestCaseData(typeof(AscendedBlast)).Returns(2966.6559113244903d);
                yield return new TestCaseData(typeof(AscendedEruption)).Returns(5310.5750490540668d);
                yield return new TestCaseData(typeof(AscendedNova)).Returns(1392.1737237500402d);
                yield return new TestCaseData(typeof(BoonOfTheAscended)).Returns(0);
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(5280.5043874132689d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(52092.609517774232d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(5512.4454516163214d);
                yield return new TestCaseData(typeof(FaeGuardians)).Returns(15840.0d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(2121.0118240353008d);
                yield return new TestCaseData(typeof(Halo)).Returns(6421.2114589256407d);
                yield return new TestCaseData(typeof(Heal)).Returns(2401.2301448859389d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(1882.5985582528954d);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(17680.606291625518d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(11655.376851761259d);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(6960.2826870482877d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(8942.5341967500008d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(1141.7297734458d);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(3734.948392394429d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(3495.9087656226748d);
                yield return new TestCaseData(typeof(Renew)).Returns(1541.2682938398161d);
                yield return new TestCaseData(typeof(UnholyNova)).Returns(10441.302928125302d);
                yield return new TestCaseData(typeof(UnholyTransfusion)).Returns(232.02895395834005d);
                yield return new TestCaseData(typeof(Smite)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(0);
                yield return new TestCaseData(typeof(HolyFire)).Returns(0);
            }
        }

        public static IEnumerable GetAverageDamage
        {
            get
            {
                yield return new TestCaseData(typeof(AscendedBlast)).Returns(2497.1851105425003d);
                yield return new TestCaseData(typeof(AscendedEruption)).Returns(3017.5482648622506d);
                yield return new TestCaseData(typeof(AscendedNova)).Returns(1032.3558557550002d);
                yield return new TestCaseData(typeof(BoonOfTheAscended)).Returns(0);
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(0);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(0.0d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(558.03019230000018d);
                yield return new TestCaseData(typeof(FaeGuardians)).Returns(0.0d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(0);
                yield return new TestCaseData(typeof(Heal)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(0);
                yield return new TestCaseData(typeof(Mindgames)).Returns(3763.6928437500005d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(0);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(0);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(0);
                yield return new TestCaseData(typeof(Renew)).Returns(0);
                yield return new TestCaseData(typeof(SpellService)).Returns(0);
                yield return new TestCaseData(typeof(UnholyNova)).Returns(0);
                yield return new TestCaseData(typeof(UnholyTransfusion)).Returns(4209.2386505247287d);
                yield return new TestCaseData(typeof(Halo)).Returns(1436.9277451725002d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(263.66926586175003d);
                yield return new TestCaseData(typeof(Smite)).Returns(983.52821392875012d);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(1569.4599158437502d);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(1980.5562252767982d);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(1055.2560197715113d);
                yield return new TestCaseData(typeof(HolyFire)).Returns(3123.1047487375436d);
            }
        }
        public static IEnumerable GetDuration
        {
            get
            {
                yield return new TestCaseData(typeof(AscendedBlast)).Returns(0);
                yield return new TestCaseData(typeof(AscendedEruption)).Returns(0);
                yield return new TestCaseData(typeof(AscendedNova)).Returns(0);
                yield return new TestCaseData(typeof(BoonOfTheAscended)).Returns(10);
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(0);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(8);
                // For some reason Divstar has a duration of 15?
                yield return new TestCaseData(typeof(DivineStar)).Returns(15);
                yield return new TestCaseData(typeof(FaeGuardians)).Returns(20);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(0);
                // Halo has a 3.2s duration, travel time of the expansion?
                yield return new TestCaseData(typeof(Halo)).Returns(3.2);
                yield return new TestCaseData(typeof(Heal)).Returns(0);
                yield return new TestCaseData(typeof(HolyNova)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(0);
                yield return new TestCaseData(typeof(Mindgames)).Returns(5);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(15);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(0);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(0);
                yield return new TestCaseData(typeof(Renew)).Returns(15);
                yield return new TestCaseData(typeof(UnholyNova)).Returns(0);
                yield return new TestCaseData(typeof(UnholyTransfusion)).Returns(15);
                yield return new TestCaseData(typeof(Smite)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(16.0d);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(0);
                yield return new TestCaseData(typeof(HolyFire)).Returns(7);
            }
        }
        public static IEnumerable GetActualManaCost
        {
            get
            {
                yield return new TestCaseData(typeof(AscendedBlast)).Returns(0d);
                yield return new TestCaseData(typeof(AscendedEruption)).Returns(0d);
                yield return new TestCaseData(typeof(AscendedNova)).Returns(0d);
                yield return new TestCaseData(typeof(BoonOfTheAscended)).Returns(0d);
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(1650d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(2200d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(1000d);
                yield return new TestCaseData(typeof(FaeGuardians)).Returns(1000d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(1800d);
                yield return new TestCaseData(typeof(Halo)).Returns(1350d);
                yield return new TestCaseData(typeof(Heal)).Returns(1200d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(800d);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(3000d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(2500d);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(2000d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(1000d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(1550d);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(2500d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(1000d);
                yield return new TestCaseData(typeof(Renew)).Returns(900d);
                yield return new TestCaseData(typeof(UnholyNova)).Returns(2500d);
                yield return new TestCaseData(typeof(UnholyTransfusion)).Returns(0d);
                yield return new TestCaseData(typeof(Smite)).Returns(100.0d);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(1000.0d);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(150.0d);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(250.0d);
                yield return new TestCaseData(typeof(HolyFire)).Returns(500.0d);
            }
        }
        public static IEnumerable GetHastedCastTime
        {
            get
            {
                yield return new TestCaseData(typeof(AscendedBlast)).Returns(0);
                yield return new TestCaseData(typeof(AscendedEruption)).Returns(0);
                yield return new TestCaseData(typeof(AscendedNova)).Returns(0);
                yield return new TestCaseData(typeof(BoonOfTheAscended)).Returns(1.3920134983d);
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(0);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(0);
                yield return new TestCaseData(typeof(DivineStar)).Returns(0);
                yield return new TestCaseData(typeof(FaeGuardians)).Returns(0);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(1.3920134983d);
                yield return new TestCaseData(typeof(Halo)).Returns(1.3920134983d);
                yield return new TestCaseData(typeof(Heal)).Returns(2.3200224972);
                yield return new TestCaseData(typeof(HolyNova)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(2.3200224972);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(0);
                yield return new TestCaseData(typeof(Mindgames)).Returns(1.3920134983d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(0);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(1.8560179978d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(0);
                yield return new TestCaseData(typeof(Renew)).Returns(0);
                yield return new TestCaseData(typeof(UnholyNova)).Returns(0);
                yield return new TestCaseData(typeof(UnholyTransfusion)).Returns(0);
                yield return new TestCaseData(typeof(Smite)).Returns(1.3920134983000001d);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(0);
                yield return new TestCaseData(typeof(HolyFire)).Returns(1.3920134983000001d);
            }
        }
        public static IEnumerable GetHastedCooldown
        {
            get
            {
                yield return new TestCaseData(typeof(AscendedBlast)).Returns(3.0d);
                yield return new TestCaseData(typeof(AscendedEruption)).Returns(0);
                yield return new TestCaseData(typeof(AscendedNova)).Returns(0);
                yield return new TestCaseData(typeof(BoonOfTheAscended)).Returns(180.0d);
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(13.920134983100001d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(180.0d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(15.0d);
                yield return new TestCaseData(typeof(FaeGuardians)).Returns(90.0d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(0d);
                yield return new TestCaseData(typeof(Halo)).Returns(40.0d);
                yield return new TestCaseData(typeof(Heal)).Returns(0.0d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(720.0d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(60.0d);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(60.0d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(45.0d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(0);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(0.0d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(11.136107986500001d);
                yield return new TestCaseData(typeof(Renew)).Returns(0);
                yield return new TestCaseData(typeof(UnholyNova)).Returns(60.0d);
                yield return new TestCaseData(typeof(UnholyTransfusion)).Returns(0);
                yield return new TestCaseData(typeof(Smite)).Returns(0.0d);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(60.0d);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(18.560179977499999d);
                yield return new TestCaseData(typeof(HolyFire)).Returns(10.0d);
            }
        }
        public static IEnumerable GetMaximumCastsPerMinute
        {
            get
            {
                // This is tested in AscendedBlastTests
                //yield return new TestCaseData(typeof(AscendedBlast)).Returns(0);
                yield return new TestCaseData(typeof(AscendedEruption)).Returns(0);
                // This is tested in AscendedNovaTests
                //yield return new TestCaseData(typeof(AscendedNova)).Returns(0);
                yield return new TestCaseData(typeof(BoonOfTheAscended)).Returns(0.4819088140d);
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(4.46143653160d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(0.48446683460000001d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(4.1511335013000004d);
                yield return new TestCaseData(typeof(FaeGuardians)).Returns(0.81780016789999999d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(43.103030302999997d);
                yield return new TestCaseData(typeof(Halo)).Returns(1.6006884982d);
                yield return new TestCaseData(typeof(Heal)).Returns(25.8618181818d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(43.103030302999997d);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(0.3485806924d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(2.1509442082999999d);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(1.5827672467d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(1.4844668346d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(43.103030302999997d);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(32.327272727299999d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(5.3878787879000001d);
                yield return new TestCaseData(typeof(Renew)).Returns(43.103030302999997d);
                yield return new TestCaseData(typeof(Smite)).Returns(43.103030302999997d);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(4.0246688547999998d);
                yield return new TestCaseData(typeof(UnholyNova)).Returns(1.1511335012999999d);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(43.103030302999997d);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(3.2327272727d);
                yield return new TestCaseData(typeof(HolyFire)).Returns(5.2668476918999998d);
                yield return new TestCaseData(typeof(UnholyTransfusion)).Returns(0);
            }
        }
        public static IEnumerable GetMaximumDamageTargets
        {
            get
            {
                yield return new TestCaseData(typeof(AscendedBlast)).Returns(1);
                yield return new TestCaseData(typeof(AscendedEruption)).Returns(double.MaxValue);
                yield return new TestCaseData(typeof(AscendedNova)).Returns(double.MaxValue);
                yield return new TestCaseData(typeof(BoonOfTheAscended)).Returns(0);
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(0);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(0);
                yield return new TestCaseData(typeof(DivineStar)).Returns(double.MaxValue);
                yield return new TestCaseData(typeof(FaeGuardians)).Returns(1);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(0);
                yield return new TestCaseData(typeof(Halo)).Returns(double.MaxValue);
                yield return new TestCaseData(typeof(Heal)).Returns(0);
                yield return new TestCaseData(typeof(HolyNova)).Returns(double.MaxValue);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(0);
                yield return new TestCaseData(typeof(Mindgames)).Returns(1);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(0);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(0);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(0);
                yield return new TestCaseData(typeof(Renew)).Returns(0);
                yield return new TestCaseData(typeof(SpellService)).Returns(0);
                yield return new TestCaseData(typeof(UnholyNova)).Returns(double.MaxValue);
                yield return new TestCaseData(typeof(UnholyTransfusion)).Returns(1);
                yield return new TestCaseData(typeof(Smite)).Returns(1);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(1);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(1);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(1);
                yield return new TestCaseData(typeof(HolyFire)).Returns(1);
            }
        }
        public static IEnumerable GetMinimumDamageTargets
        {
            get
            {
                yield return new TestCaseData(typeof(AscendedBlast)).Returns(1);
                yield return new TestCaseData(typeof(AscendedEruption)).Returns(0);
                yield return new TestCaseData(typeof(AscendedNova)).Returns(0);
                yield return new TestCaseData(typeof(BoonOfTheAscended)).Returns(0);
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(0);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(0);
                yield return new TestCaseData(typeof(DivineStar)).Returns(0);
                yield return new TestCaseData(typeof(FaeGuardians)).Returns(0);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(0);
                yield return new TestCaseData(typeof(Halo)).Returns(0);
                yield return new TestCaseData(typeof(Heal)).Returns(0);
                yield return new TestCaseData(typeof(HolyNova)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(0);
                yield return new TestCaseData(typeof(Mindgames)).Returns(1);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(0);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(0);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(0);
                yield return new TestCaseData(typeof(Renew)).Returns(0);
                yield return new TestCaseData(typeof(SpellService)).Returns(0);
                yield return new TestCaseData(typeof(UnholyNova)).Returns(0);
                yield return new TestCaseData(typeof(UnholyTransfusion)).Returns(1);
                yield return new TestCaseData(typeof(Smite)).Returns(1);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(1);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(1);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(1);
                yield return new TestCaseData(typeof(HolyFire)).Returns(1);
            }
        }
        public static IEnumerable GetNumberOfDamageTargets
        {
            get
            {
                yield return new TestCaseData(typeof(AscendedBlast)).Returns(1);
                yield return new TestCaseData(typeof(AscendedEruption)).Returns(1);
                yield return new TestCaseData(typeof(AscendedNova)).Returns(1);
                yield return new TestCaseData(typeof(BoonOfTheAscended)).Returns(0);
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(0);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(0);
                yield return new TestCaseData(typeof(DivineStar)).Returns(1);
                yield return new TestCaseData(typeof(FaeGuardians)).Returns(0);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(0);
                yield return new TestCaseData(typeof(Halo)).Returns(1);
                yield return new TestCaseData(typeof(Heal)).Returns(0);
                yield return new TestCaseData(typeof(HolyNova)).Returns(1);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(0);
                yield return new TestCaseData(typeof(Mindgames)).Returns(1);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(0);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(0);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(0);
                yield return new TestCaseData(typeof(Renew)).Returns(0);
                yield return new TestCaseData(typeof(UnholyNova)).Returns(1);
                yield return new TestCaseData(typeof(UnholyTransfusion)).Returns(1);
                yield return new TestCaseData(typeof(Smite)).Returns(1);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(1);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(1);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(1);
                yield return new TestCaseData(typeof(HolyFire)).Returns(1);
            }
        }
        public static IEnumerable GetAverageOverhealing
        {
            get
            {
                yield return new TestCaseData(typeof(AscendedBlast)).Returns(29.966221326500001d);
                yield return new TestCaseData(typeof(AscendedEruption)).Returns(53.642172212699997d);
                yield return new TestCaseData(typeof(AscendedNova)).Returns(14.062360846000001d);
                yield return new TestCaseData(typeof(BoonOfTheAscended)).Returns(0);
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(871.77848269419997d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(23844.139050409802d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(4331.2071405556999d);
                yield return new TestCaseData(typeof(FaeGuardians)).Returns(160.0d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(257.8708857396d);
                yield return new TestCaseData(typeof(Halo)).Returns(3703.6883501655998d);
                yield return new TestCaseData(typeof(Heal)).Returns(1055.7668964126001d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(332.2232749858d);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(8100.3885926345001d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(5571.0151845397004d);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(1242.7611397617d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(90.328628249999994d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(699.76986114420004d);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(1391.9539993618d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(78.274616058800007d);
                yield return new TestCaseData(typeof(Renew)).Returns(883.2531688624d);
                yield return new TestCaseData(typeof(UnholyNova)).Returns(105.46770634470001d);
                yield return new TestCaseData(typeof(UnholyTransfusion)).Returns(2.3437268077d);
                yield return new TestCaseData(typeof(Smite)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(0);
                yield return new TestCaseData(typeof(HolyFire)).Returns(0);
            }
        }

        public static IEnumerable TriggersMastery
        {
            get
            {
                yield return new TestCaseData(typeof(AscendedBlast)).Returns(true); // broken waiting for simcraft spellid 325315
                yield return new TestCaseData(typeof(AscendedEruption)).Returns(true);
                yield return new TestCaseData(typeof(AscendedNova)).Returns(true);
                yield return new TestCaseData(typeof(BoonOfTheAscended)).Returns(false);
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(true);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(true);
                yield return new TestCaseData(typeof(DivineStar)).Returns(true);
                yield return new TestCaseData(typeof(FaeGuardians)).Returns(false);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(true);
                yield return new TestCaseData(typeof(Halo)).Returns(true);
                yield return new TestCaseData(typeof(Heal)).Returns(true);
                yield return new TestCaseData(typeof(HolyNova)).Returns(true);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(true);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(true);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(true);
                yield return new TestCaseData(typeof(Mindgames)).Returns(true);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(false);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(true);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(true);
                yield return new TestCaseData(typeof(Renew)).Returns(false);
                yield return new TestCaseData(typeof(UnholyNova)).Returns(true);
                yield return new TestCaseData(typeof(UnholyTransfusion)).Returns(true);
                yield return new TestCaseData(typeof(Smite)).Returns(false);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(false);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(false);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(false);
                yield return new TestCaseData(typeof(HolyFire)).Returns(false);
            }
        }
    }
}
