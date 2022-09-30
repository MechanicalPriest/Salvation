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

            Spells.Add(new CircleOfHealing(gameStateService));
            Spells.Add(new DivineHymn(gameStateService));
            Spells.Add(new DivineStar(gameStateService));
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
        public static IEnumerable GetActualManaCost
        {
            get
            {
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(8250.0d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(11000.0d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(5000.0d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(9000.0d);
                yield return new TestCaseData(typeof(Halo)).Returns(6750.0d);
                yield return new TestCaseData(typeof(Heal)).Returns(6000.0d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(4000.0d);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(15000.0d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(8750.0d);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(6250.0d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(5000.0d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(7750.0d);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(10000.0d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(5000.0d);
                yield return new TestCaseData(typeof(Renew)).Returns(4500.0d);
                yield return new TestCaseData(typeof(Smite)).Returns(500.0d);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(5000.0d);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(750.0d);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(1250.0d);
                yield return new TestCaseData(typeof(HolyFire)).Returns(2500.0d);
            }
        }

        public static IEnumerable GetAverageDamage
        {
            get
            {
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(0);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(0.0d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(5135.677705870361d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(0);
                yield return new TestCaseData(typeof(Heal)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(0);
                yield return new TestCaseData(typeof(Mindgames)).Returns(12661.073339004877d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(0);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(0);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(0);
                yield return new TestCaseData(typeof(Renew)).Returns(0);
                yield return new TestCaseData(typeof(SpellService)).Returns(0);
                yield return new TestCaseData(typeof(Halo)).Returns(6612.1850463080882d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(1444.409354776039d);
                yield return new TestCaseData(typeof(Smite)).Returns(3232.725698784468d);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(6419.5971323379499d);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(6119.0332380187547d);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(3468.4854034695795d);
                yield return new TestCaseData(typeof(HolyFire)).Returns(9669.677321857489d);
            }
        }

        public static IEnumerable GetAverageHealing
        {
            get
            {
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(16644.611077279609d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(164200.45545094009d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(17375.709571528052d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(6685.6145381734814d);
                yield return new TestCaseData(typeof(Halo)).Returns(22629.689753218856d);
                yield return new TestCaseData(typeof(Heal)).Returns(7568.8871623577788d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(9890.1918320081568d);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(55730.815419087296d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(36738.765925299624d);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(21939.419005121359d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(17809.693955121951d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(5527.986419513235d);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(11772.883577807093d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(11019.409794287913d);
                yield return new TestCaseData(typeof(Renew)).Returns(4565.0415365617973d);
                yield return new TestCaseData(typeof(Smite)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(0);
                yield return new TestCaseData(typeof(HolyFire)).Returns(0);
            }
        }

        public static IEnumerable GetAverageOverhealing
        {
            get
            {
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(2747.9219266579998d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(75158.809054803496d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(13652.343234772001d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(812.83155668239999d);
                yield return new TestCaseData(typeof(Halo)).Returns(13052.570974026299d);
                yield return new TestCaseData(typeof(Heal)).Returns(3327.8694779500001d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(1745.3279703543999d);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(25533.132406936798d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(17560.326485725502d);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(3917.2916667954d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(179.89589853659999d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(3388.1207087338998d);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(4387.5605921408996d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(246.7284270472d);
                yield return new TestCaseData(typeof(Renew)).Returns(2616.0840518631999d);
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
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(19392.533003937562d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(239359.26450574357d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(31028.052806300089d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(7498.4460948558562d);
                yield return new TestCaseData(typeof(Halo)).Returns(35682.260727245121d);
                yield return new TestCaseData(typeof(Heal)).Returns(10896.756640307773d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(11635.519802362538d);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(81263.947826024058d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(54299.092411025165d);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(25856.710671916746d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(17989.589853658537d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(8916.107128247153d);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(16160.444169947967d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(11266.138221335154d);
                yield return new TestCaseData(typeof(Renew)).Returns(7181.1255884250404d);
                yield return new TestCaseData(typeof(Smite)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(0);
                yield return new TestCaseData(typeof(HolyFire)).Returns(0);
            }
        }

        public static IEnumerable GetDuration
        {
            get
            {
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(0);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(8);
                // For some reason Divstar has a duration of 15?
                yield return new TestCaseData(typeof(DivineStar)).Returns(15);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(0);
                // Halo has a 3.2s duration, travel time of the expansion?
                yield return new TestCaseData(typeof(Halo)).Returns(2.150);
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
                yield return new TestCaseData(typeof(Smite)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(16.0d);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(0);
                yield return new TestCaseData(typeof(HolyFire)).Returns(7);
            }
        }

        public static IEnumerable GetHastedCastTime
        {
            get
            {
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(0);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(0);
                yield return new TestCaseData(typeof(DivineStar)).Returns(0);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(1.4777468707000001d);
                yield return new TestCaseData(typeof(Halo)).Returns(1.4777468707000001d);
                yield return new TestCaseData(typeof(Heal)).Returns(2.4629114511000001d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(2.4629114511000001d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(0);
                yield return new TestCaseData(typeof(Mindgames)).Returns(1.4777468707000001d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(0);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(1.9703291609d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(0);
                yield return new TestCaseData(typeof(Renew)).Returns(0);
                yield return new TestCaseData(typeof(Smite)).Returns(1.4777468707000001d);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(0);
                yield return new TestCaseData(typeof(HolyFire)).Returns(1.4777468707000001d);
            }
        }

        public static IEnumerable GetHastedCooldown
        {
            get
            {
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(14.777468706500001d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(180.0d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(15.0d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(0d);
                yield return new TestCaseData(typeof(Halo)).Returns(40.0d);
                yield return new TestCaseData(typeof(Heal)).Returns(0.0d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(720.0d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(60.0d);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(60.0d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(45.0d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(7.3887343533000003d);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(0.0d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(11.821974965200001d);
                yield return new TestCaseData(typeof(Renew)).Returns(0);
                yield return new TestCaseData(typeof(Smite)).Returns(0.0d);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(60.0d);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(20.0d);
                yield return new TestCaseData(typeof(HolyFire)).Returns(10.0d);
            }
        }

        public static IEnumerable GetMaximumCastsPerMinute
        {
            get
            {
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(4.2113687954000003d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(0.48446683460000001d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(4.1511335013000004d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(40.602352941200003d);
                yield return new TestCaseData(typeof(Halo)).Returns(1.5976923075d);
                yield return new TestCaseData(typeof(Heal)).Returns(24.361411764700001d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(40.602352941200003d);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(0.34595361790000001d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(2.0929389129999998d);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(1.5577254636d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(1.4844668346d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(8.2716040895000003d);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(30.451764705900001d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(5.0752941176000004d);
                yield return new TestCaseData(typeof(Renew)).Returns(40.602352941200003d);
                yield return new TestCaseData(typeof(Smite)).Returns(40.602352941200003d);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(3.8579570307000002d);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(40.602352941200003d);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(3.0d);
                yield return new TestCaseData(typeof(HolyFire)).Returns(5.2275068161d);
            }
        }

        public static IEnumerable GetMaximumDamageTargets
        {
            get
            {
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(0);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(0);
                yield return new TestCaseData(typeof(DivineStar)).Returns(double.MaxValue);
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
                yield return new TestCaseData(typeof(Smite)).Returns(1);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(1);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(1);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(1);
                yield return new TestCaseData(typeof(HolyFire)).Returns(1);
            }
        }

        public static IEnumerable GetMaximumHealTargets
        {
            get
            {
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(5);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(double.MaxValue);
                yield return new TestCaseData(typeof(DivineStar)).Returns(double.MaxValue);
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
                yield return new TestCaseData(typeof(Smite)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(0);
                yield return new TestCaseData(typeof(HolyFire)).Returns(0);
            }
        }

        public static IEnumerable GetMinimumDamageTargets
        {
            get
            {
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(0);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(0);
                yield return new TestCaseData(typeof(DivineStar)).Returns(0);
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
                yield return new TestCaseData(typeof(Smite)).Returns(1);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(1);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(1);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(1);
                yield return new TestCaseData(typeof(HolyFire)).Returns(1);
            }
        }

        public static IEnumerable GetMinimumHealTargets
        {
            get
            {
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(1);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(1);
                yield return new TestCaseData(typeof(DivineStar)).Returns(1);
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
                yield return new TestCaseData(typeof(Smite)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(0);
                yield return new TestCaseData(typeof(HolyFire)).Returns(0);
            }
        }

        public static IEnumerable GetNumberOfDamageTargets
        {
            get
            {
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(0);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(0);
                yield return new TestCaseData(typeof(DivineStar)).Returns(1);
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
                yield return new TestCaseData(typeof(Smite)).Returns(1);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(1);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(1);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(1);
                yield return new TestCaseData(typeof(HolyFire)).Returns(1);
            }
        }

        public static IEnumerable GetNumberOfHealingTargets
        {
            get
            {
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(5);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(20);
                yield return new TestCaseData(typeof(DivineStar)).Returns(6);
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
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(true);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(true);
                yield return new TestCaseData(typeof(DivineStar)).Returns(true);
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
                yield return new TestCaseData(typeof(Smite)).Returns(false);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(false);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(false);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(false);
                yield return new TestCaseData(typeof(HolyFire)).Returns(false);
            }
        }
    }
}
