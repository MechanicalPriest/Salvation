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
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(250.0d);
                yield return new TestCaseData(typeof(HolyFire)).Returns(2500.0d);
            }
        }

        public static IEnumerable GetAverageDamage
        {
            get
            {
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(0);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(0.0d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(5120.6893720423341d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(0);
                yield return new TestCaseData(typeof(Heal)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(0);
                yield return new TestCaseData(typeof(Mindgames)).Returns(12650.230473983998d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(0);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(0);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(0);
                yield return new TestCaseData(typeof(Renew)).Returns(0);
                yield return new TestCaseData(typeof(SpellService)).Returns(0);
                yield return new TestCaseData(typeof(Halo)).Returns(6592.8875665045034d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(1440.1938858869064d);
                yield return new TestCaseData(typeof(Smite)).Returns(3223.2910779373606d);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(6400.8617150529153d);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(6086.3880958888785d);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(1215.6549347767811d);
                yield return new TestCaseData(typeof(HolyFire)).Returns(9614.2118628573062d);
            }
        }

        public static IEnumerable GetAverageHealing
        {
            get
            {
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(16596.034238632044d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(163721.24094762345d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(17324.999042076561d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(6666.1027564218493d);
                yield return new TestCaseData(typeof(Halo)).Returns(22563.645627424463d);
                yield return new TestCaseData(typeof(Heal)).Returns(7546.7975737984298d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(9861.3275797534006d);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(55568.166570415451d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(36631.544849590631d);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(21875.389415479109d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(17794.441843199998d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(5511.8531435177883d);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(11738.524741585539d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(10987.24995053529d);
                yield return new TestCaseData(typeof(Renew)).Returns(4540.9735040252608d);
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
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(2739.902192257d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(74939.460142206604d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(13612.499247345901d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(810.45933018860001d);
                yield return new TestCaseData(typeof(Halo)).Returns(13014.4774054113d);
                yield return new TestCaseData(typeof(Heal)).Returns(3318.1571826058998d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(1740.2342787800001d);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(25458.614663786098d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(17509.077156898598d);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(3905.8591590395999d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(179.74183679999999d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(3378.2325718335001d);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(4374.7556174886004d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(246.0083569336d);
                yield return new TestCaseData(typeof(Renew)).Returns(2602.2914071361001d);
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
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(19335.936430889018d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(238660.70108983011d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(30937.498289422427d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(7476.5620866104191d);
                yield return new TestCaseData(typeof(Halo)).Returns(35578.1230328358d);
                yield return new TestCaseData(typeof(Heal)).Returns(10864.954756404304d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(11601.561858533412d);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(81026.781234201597d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(54140.622006489255d);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(25781.24857451869d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(17974.183679999998d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(8890.0857153512716d);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(16113.280359074181d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(11233.258307468857d);
                yield return new TestCaseData(typeof(Renew)).Returns(7143.2649111613355d);
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
                yield return new TestCaseData(typeof(FlashHeal)).Returns(1.4819345126000001d);
                yield return new TestCaseData(typeof(Halo)).Returns(1.4819345126000001d);
                yield return new TestCaseData(typeof(Heal)).Returns(2.4698908543d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(2.4698908543d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(0);
                yield return new TestCaseData(typeof(Mindgames)).Returns(1.4819345126000001d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(0);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(1.9759126835d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(0);
                yield return new TestCaseData(typeof(Renew)).Returns(0);
                yield return new TestCaseData(typeof(Smite)).Returns(1.4819345126000001d);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(0);
                yield return new TestCaseData(typeof(HolyFire)).Returns(1.4819345126000001d);
            }
        }

        public static IEnumerable GetHastedCooldown
        {
            get
            {
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(14.8193451261d);
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
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(7.409672563d);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(0.0d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(11.855476100900001d);
                yield return new TestCaseData(typeof(Renew)).Returns(0);
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
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(4.1998954060000004d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(0.48446683460000001d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(4.1511335013000004d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(40.487619047599999d);
                yield return new TestCaseData(typeof(Halo)).Returns(1.5975462760000001d);
                yield return new TestCaseData(typeof(Heal)).Returns(24.292571428599999d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(40.487619047599999d);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(0.34583299560000003d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(2.0902775648d);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(1.5565765184d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(1.4844668346d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(8.2486573108000005d);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(30.365714285700001d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(5.0609523809999999d);
                yield return new TestCaseData(typeof(Renew)).Returns(40.487619047599999d);
                yield return new TestCaseData(typeof(Smite)).Returns(40.487619047599999d);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(3.8503081043999998d);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(40.487619047599999d);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(3.2327272727d);
                yield return new TestCaseData(typeof(HolyFire)).Returns(5.2256002622000004d);
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
