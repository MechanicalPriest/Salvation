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
            Spells.Add(new FlashHeal(gameStateService, null, null));
            Spells.Add(new Halo(gameStateService));
            Spells.Add(new Heal(gameStateService, null, null));
            Spells.Add(new HolyNova(gameStateService));
            Spells.Add(new HolyWordSalvation(gameStateService,
                new HolyWordSerenity(gameStateService,
                    new FlashHeal(gameStateService, null, null),
                    new Heal(gameStateService, null, null),
                    new PrayerOfMending(gameStateService, null, null)),
                new HolyWordSanctify(gameStateService,
                    new PrayerOfHealing(gameStateService, new Renew(gameStateService, null)),
                    new Renew(gameStateService, null),
                    new CircleOfHealing(gameStateService)),
                new Renew(gameStateService, null),
                new PrayerOfMending(gameStateService, null, null)));
            Spells.Add(new HolyWordSanctify(gameStateService,
                new PrayerOfHealing(gameStateService, new Renew(gameStateService, null)),
                new Renew(gameStateService, null),
                new CircleOfHealing(gameStateService)));
            Spells.Add(new HolyWordSerenity(gameStateService,
                new FlashHeal(gameStateService, null, null),
                new Heal(gameStateService, null, null),
                new PrayerOfMending(gameStateService, null, null)));
            Spells.Add(new Mindgames(gameStateService));
            Spells.Add(new PowerWordShield(gameStateService));
            Spells.Add(new PrayerOfHealing(gameStateService, new Renew(gameStateService, null)));
            Spells.Add(new PrayerOfMending(gameStateService, null, null));
            Spells.Add(new Renew(gameStateService, null));
            Spells.Add(new SpellService(gameStateService));
            Spells.Add(new Smite(gameStateService));
            Spells.Add(new HolyWordChastise(gameStateService, new Smite(gameStateService), new HolyFire(gameStateService)));
            Spells.Add(new ShadowWordPain(gameStateService));
            Spells.Add(new ShadowWordDeath(gameStateService));
            Spells.Add(new HolyFire(gameStateService));
            Spells.Add(new CosmicRipple(gameStateService,
                new HolyWordSerenity(gameStateService,
                    new FlashHeal(gameStateService, null, null),
                    new Heal(gameStateService, null, null),
                    new PrayerOfMending(gameStateService, null, null)),
                new HolyWordSanctify(gameStateService,
                    new PrayerOfHealing(gameStateService, new Renew(gameStateService, null)),
                    new Renew(gameStateService, null),
                    new CircleOfHealing(gameStateService))));
            Spells.Add(new Lightwell(gameStateService));

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

        [TestCaseSource(typeof(SpellServiceTestsData), nameof(SpellServiceTestsData.GetRenewUptime))]
        public double GetRenewUptime(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetRenewUptime(_gameState, null);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(SpellServiceTestsData), nameof(SpellServiceTestsData.GetRenewTicksPerMinute))]
        public double GetRenewTicksPerMinute(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();

            // Act
            var result = spellService.GetRenewTicksPerMinute(_gameState, null);

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
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(11000.0d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(5000.0d);
                yield return new TestCaseData(typeof(Renew)).Returns(4500.0d);
                yield return new TestCaseData(typeof(Smite)).Returns(500.0d);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(5000.0d);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(750.0d);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(1250.0d);
                yield return new TestCaseData(typeof(HolyFire)).Returns(2500.0d);
                yield return new TestCaseData(typeof(CosmicRipple)).Returns(0.0d);
                yield return new TestCaseData(typeof(Lightwell)).Returns(9250.0d);
            }
        }

        public static IEnumerable GetAverageDamage
        {
            get
            {
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(0);
                yield return new TestCaseData(typeof(CosmicRipple)).Returns(0.0d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(0.0d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(4647.9260095420559d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(0);
                yield return new TestCaseData(typeof(Halo)).Returns(5984.2047372853976d);
                yield return new TestCaseData(typeof(Heal)).Returns(0);
                yield return new TestCaseData(typeof(HolyFire)).Returns(8751.3172169598965d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(1307.2291901837032d);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(5809.9075119275703d);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(0);
                yield return new TestCaseData(typeof(Lightwell)).Returns(0.0d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(8593.9581080721946d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(0);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(0);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(0);
                yield return new TestCaseData(typeof(Renew)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(3139.0722790247805d);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(5537.8891295554449d);
                yield return new TestCaseData(typeof(Smite)).Returns(2925.7034256492407d);
                yield return new TestCaseData(typeof(SpellService)).Returns(0);
            }
        }

        public static IEnumerable GetAverageHealing
        {
            get
            {
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(14310.628298101959d);
                yield return new TestCaseData(typeof(CosmicRipple)).Returns(5114.0113149176659d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(141175.52362307918d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(15725.482998950622d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(5748.1273762405272d);
                yield return new TestCaseData(typeof(Halo)).Returns(20480.475920758316d);
                yield return new TestCaseData(typeof(Heal)).Returns(6507.5435111026291d);
                yield return new TestCaseData(typeof(HolyFire)).Returns(0);
                yield return new TestCaseData(typeof(HolyNova)).Returns(8950.8887605634118d);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(47915.987974111187d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(31587.089710110911d);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(18862.97426842132d);
                yield return new TestCaseData(typeof(Lightwell)).Returns(82129.226361867593d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(16118.250501073171d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(6003.571095658488d);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(10122.036501579942d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(9379.4760435897879d);
                yield return new TestCaseData(typeof(Renew)).Returns(3532.4196559856459d);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(0);
                yield return new TestCaseData(typeof(Smite)).Returns(0);
            }
        }

        public static IEnumerable GetAverageOverhealing
        {
            get
            {
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(2362.5958637319d);
                yield return new TestCaseData(typeof(CosmicRipple)).Returns(1555.2783498158999d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(64619.700317269497d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(12355.7366420326d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(698.85263300190002d);
                yield return new TestCaseData(typeof(Halo)).Returns(11812.926666372399d);
                yield return new TestCaseData(typeof(Heal)).Returns(2861.2205417373002d);
                yield return new TestCaseData(typeof(HolyFire)).Returns(0);
                yield return new TestCaseData(typeof(HolyNova)).Returns(1579.5686048053001d);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(21952.760894525702d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(15097.9379430238d);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(3367.9912806903999d);
                yield return new TestCaseData(typeof(Lightwell)).Returns(26363.80714126d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(162.81061112200001d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(3679.6080908875001d);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(3772.3169666149001d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(210.0097386306d);
                yield return new TestCaseData(typeof(Renew)).Returns(2024.3204037684d);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(0);
                yield return new TestCaseData(typeof(Smite)).Returns(0);
            }
        }

        public static IEnumerable GetAverageRawHealing
        {
            get
            {
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(16673.224161833808d);
                yield return new TestCaseData(typeof(CosmicRipple)).Returns(6669.2896647335228d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(205795.22394034869d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(28081.21964098325d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(6446.9800092424039d);
                yield return new TestCaseData(typeof(Halo)).Returns(32293.402587130739d);
                yield return new TestCaseData(typeof(Heal)).Returns(9368.7640528399497d);
                yield return new TestCaseData(typeof(HolyFire)).Returns(0);
                yield return new TestCaseData(typeof(HolyNova)).Returns(10530.45736536872d);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(69868.748868636903d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(46685.027653134661d);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(22230.965549111748d);
                yield return new TestCaseData(typeof(Lightwell)).Returns(108493.0335031276d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(16281.061112195122d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(9683.1791865459491d);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(13894.353468194842d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(9589.4857822204158d);
                yield return new TestCaseData(typeof(Renew)).Returns(5556.7400597540445d);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(0);
                yield return new TestCaseData(typeof(Smite)).Returns(0);
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
                // Halo has a 2.150s duration, travel time of the expansion?
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
                yield return new TestCaseData(typeof(CosmicRipple)).Returns(0.0d);
                yield return new TestCaseData(typeof(Lightwell)).Returns(180.0d);
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
                yield return new TestCaseData(typeof(CosmicRipple)).Returns(0.0d);
                yield return new TestCaseData(typeof(Lightwell)).Returns(0.49258229019999999d);
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
                yield return new TestCaseData(typeof(CosmicRipple)).Returns(0.0d);
                yield return new TestCaseData(typeof(Lightwell)).Returns(180.0d);
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
                yield return new TestCaseData(typeof(CosmicRipple)).Returns(3.6506643766d);
                yield return new TestCaseData(typeof(Lightwell)).Returns(0.48446683460000001d);
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
                yield return new TestCaseData(typeof(CosmicRipple)).Returns(0.0d);
                yield return new TestCaseData(typeof(Lightwell)).Returns(0.0d);
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
                yield return new TestCaseData(typeof(CosmicRipple)).Returns(5.0d);
                yield return new TestCaseData(typeof(Lightwell)).Returns(15.0d);
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
                yield return new TestCaseData(typeof(CosmicRipple)).Returns(0.0d);
                yield return new TestCaseData(typeof(Lightwell)).Returns(0.0d);
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
                yield return new TestCaseData(typeof(CosmicRipple)).Returns(1.0d);
                yield return new TestCaseData(typeof(Lightwell)).Returns(0.0d);
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
                yield return new TestCaseData(typeof(CosmicRipple)).Returns(0.0d);
                yield return new TestCaseData(typeof(Lightwell)).Returns(0.0d);
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
                yield return new TestCaseData(typeof(CosmicRipple)).Returns(5.0d);
                yield return new TestCaseData(typeof(Lightwell)).Returns(15.0d);
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
                yield return new TestCaseData(typeof(CosmicRipple)).Returns(true);
                yield return new TestCaseData(typeof(Lightwell)).Returns(false);
            }
        }
        
        public static IEnumerable GetRenewUptime
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
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(0.075590865517687705d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(0);
                yield return new TestCaseData(typeof(Mindgames)).Returns(0);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(0);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(0);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(0);
                yield return new TestCaseData(typeof(Renew)).Returns(0.018474070588235292d);
                yield return new TestCaseData(typeof(Smite)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(0);
                yield return new TestCaseData(typeof(HolyFire)).Returns(0);
                yield return new TestCaseData(typeof(CosmicRipple)).Returns(0);
                yield return new TestCaseData(typeof(Lightwell)).Returns(0);
            }
        }

        public static IEnumerable GetRenewTicksPerMinute
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
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(42.330884689905119d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(0);
                yield return new TestCaseData(typeof(Mindgames)).Returns(0);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(0);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(0);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(0);
                yield return new TestCaseData(typeof(Renew)).Returns(10.345479529411765d);
                yield return new TestCaseData(typeof(Smite)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(0);
                yield return new TestCaseData(typeof(HolyFire)).Returns(0);
                yield return new TestCaseData(typeof(CosmicRipple)).Returns(0);
                yield return new TestCaseData(typeof(Lightwell)).Returns(0);
            }
        }
    }
}
