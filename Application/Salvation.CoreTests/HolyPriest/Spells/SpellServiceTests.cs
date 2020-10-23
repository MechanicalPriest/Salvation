using Newtonsoft.Json;
using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.Profile;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
            var basePath = @"HolyPriest" + Path.DirectorySeparatorChar + "TestData";
            var constants = constantsService.ParseConstants(
                File.ReadAllText(Path.Combine(basePath, "SpellServiceTests_constants.json")));
            var profile = JsonConvert.DeserializeObject<PlayerProfile>(
                File.ReadAllText(Path.Combine(basePath, "SpellServiceTests_profile.json")));

            Spells.Add(new AscendedBlast(gameStateService));
            Spells.Add(new AscendedEruption(gameStateService));
            Spells.Add(new AscendedNova(gameStateService));
            Spells.Add(new AscendedBlast(gameStateService));
            Spells.Add(new AscendedEruption(gameStateService));
            Spells.Add(new AscendedNova(gameStateService));
            Spells.Add(new BindingHeal(gameStateService));
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
                    new BindingHeal(gameStateService),
                    new PrayerOfMending(gameStateService)),
                new HolyWordSanctify(gameStateService,
                    new PrayerOfHealing(gameStateService),
                    new Renew(gameStateService),
                    new BindingHeal(gameStateService),
                    new CircleOfHealing(gameStateService)),
                new Renew(gameStateService),
                new PrayerOfMending(gameStateService)));
            Spells.Add(new HolyWordSanctify(gameStateService,
                new PrayerOfHealing(gameStateService),
                new Renew(gameStateService),
                new BindingHeal(gameStateService),
                new CircleOfHealing(gameStateService)));
            Spells.Add(new HolyWordSerenity(gameStateService,
                new FlashHeal(gameStateService),
                new Heal(gameStateService),
                new BindingHeal(gameStateService),
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

            _gameState = new GameState(profile, constants);
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
                yield return new TestCaseData(typeof(BindingHeal)).Returns(3);
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
                yield return new TestCaseData(typeof(UnholyNova)).Returns(6);
                yield return new TestCaseData(typeof(UnholyTransfusion)).Returns(1);
            }
        }
        public static IEnumerable GetMinimumHealTargets
        {
            get
            {
                yield return new TestCaseData(typeof(AscendedBlast)).Returns(1);
                yield return new TestCaseData(typeof(AscendedEruption)).Returns(0);
                yield return new TestCaseData(typeof(AscendedNova)).Returns(1);
                yield return new TestCaseData(typeof(BindingHeal)).Returns(2);
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
                yield return new TestCaseData(typeof(UnholyNova)).Returns(1);
                yield return new TestCaseData(typeof(UnholyTransfusion)).Returns(0);
            }
        }
        public static IEnumerable GetNumberOfHealingTargets
        {
            get
            {
                yield return new TestCaseData(typeof(AscendedBlast)).Returns(1);
                yield return new TestCaseData(typeof(AscendedEruption)).Returns(5);
                yield return new TestCaseData(typeof(AscendedNova)).Returns(5);
                yield return new TestCaseData(typeof(BindingHeal)).Returns(3);
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
            }
        }
        public static IEnumerable GetAverageRawHealing
        {
            get
            {
                yield return new TestCaseData(typeof(AscendedBlast)).Returns(3000.7516073400002d);
                yield return new TestCaseData(typeof(AscendedEruption)).Returns(4903.5860132224225d);
                yield return new TestCaseData(typeof(AscendedNova)).Returns(629.7545301885317d);
                yield return new TestCaseData(typeof(BindingHeal)).Returns(3309.2087558040002d);
                yield return new TestCaseData(typeof(BoonOfTheAscended)).Returns(0);
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(6160.76098155d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(76041.392686560022d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(9857.2175704800011d);
                yield return new TestCaseData(typeof(FaeGuardians)).Returns(8000.0d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(2382.1609128660002d);
                yield return new TestCaseData(typeof(Halo)).Returns(10138.852358208d);
                yield return new TestCaseData(typeof(Heal)).Returns(3461.7609324900009d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(2112.2609079599997d);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(25816.522208400005d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(17250.130748340005d);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(8214.3479754000018d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(9045.3104999999996d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(2011.6770552d);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(5133.9674846250009d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(3579.1087607100003d);
                yield return new TestCaseData(typeof(Renew)).Returns(2427.862557965499d);
                yield return new TestCaseData(typeof(UnholyNova)).Returns(8378.6349349079992d);
                yield return new TestCaseData(typeof(UnholyTransfusion)).Returns(234.69565644000002d);
            }
        }
        public static IEnumerable GetAverageHealing
        {
            get
            {
                yield return new TestCaseData(typeof(AscendedBlast)).Returns(2970.7440912666002d);
                yield return new TestCaseData(typeof(AscendedEruption)).Returns(4854.5501530901984d);
                yield return new TestCaseData(typeof(AscendedNova)).Returns(623.45698488664641d);
                yield return new TestCaseData(typeof(BindingHeal)).Returns(2184.07777883064d);
                yield return new TestCaseData(typeof(BoonOfTheAscended)).Returns(0);
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(5287.7811504643651d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(52164.395382980169d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(5520.0418394688013d);
                yield return new TestCaseData(typeof(FaeGuardians)).Returns(7920.0d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(2123.9346699113257d);
                yield return new TestCaseData(typeof(Halo)).Returns(6430.0601655755136d);
                yield return new TestCaseData(typeof(Heal)).Returns(2404.5391437075546d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(1795.4217717659997d);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(17704.970930520722d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(11671.438464326848d);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(6969.8742571269022d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(8954.8573949999991d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(1247.239774224d);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(3740.0953125493129d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(3500.7262788504513d);
                yield return new TestCaseData(typeof(Renew)).Returns(1543.3922280986676d);
                yield return new TestCaseData(typeof(SpellService)).Returns(0);
                yield return new TestCaseData(typeof(UnholyNova)).Returns(8294.8485855589188d);
                yield return new TestCaseData(typeof(UnholyTransfusion)).Returns(232.34869987560003d);
            }
        }
        public static IEnumerable GetDuration
        {
            get
            {
                yield return new TestCaseData(typeof(AscendedBlast)).Returns(0);
                yield return new TestCaseData(typeof(AscendedEruption)).Returns(0);
                yield return new TestCaseData(typeof(AscendedNova)).Returns(0);
                yield return new TestCaseData(typeof(BindingHeal)).Returns(0);
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
            }
        }
        public static IEnumerable GetActualManaCost
        {
            get
            {
                yield return new TestCaseData(typeof(AscendedBlast)).Returns(0d);
                yield return new TestCaseData(typeof(AscendedEruption)).Returns(0d);
                yield return new TestCaseData(typeof(AscendedNova)).Returns(0d);
                yield return new TestCaseData(typeof(BindingHeal)).Returns(1700d);
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
            }
        }
        public static IEnumerable GetHastedCastTime
        {
            get
            {
                yield return new TestCaseData(typeof(AscendedBlast)).Returns(0);
                yield return new TestCaseData(typeof(AscendedEruption)).Returns(0);
                yield return new TestCaseData(typeof(AscendedNova)).Returns(0);
                yield return new TestCaseData(typeof(BindingHeal)).Returns(1.3920134983d);
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
                yield return new TestCaseData(typeof(BindingHeal)).Returns(43.103030302999997d);
                yield return new TestCaseData(typeof(BoonOfTheAscended)).Returns(0.4819088140d);
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(4.46143653160d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(0.48446683460000001d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(4.1511335013000004d);
                yield return new TestCaseData(typeof(FaeGuardians)).Returns(0.81780016789999999d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(43.103030302999997d);
                yield return new TestCaseData(typeof(Halo)).Returns(1.6006884982d);
                yield return new TestCaseData(typeof(Heal)).Returns(25.8618181818d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(43.103030302999997d);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(0.36425417399999999d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(2.4095623901d);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(1.8413854285d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(1.4844668346d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(43.103030302999997d);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(32.327272727299999d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(5.3878787879000001d);
                yield return new TestCaseData(typeof(Renew)).Returns(43.103030302999997d);
                yield return new TestCaseData(typeof(UnholyNova)).Returns(1.1511335012999999d);
                yield return new TestCaseData(typeof(UnholyTransfusion)).Returns(0);
            }
        }
    }
}
