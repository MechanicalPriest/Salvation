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
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(10000.0d);
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
                yield return new TestCaseData(typeof(DivineHymn)).Returns(0.0d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(4426.5961995638636d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(0);
                yield return new TestCaseData(typeof(Heal)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(0);
                yield return new TestCaseData(typeof(Mindgames)).Returns(10912.962676917074d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(0);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(0);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(0);
                yield return new TestCaseData(typeof(Renew)).Returns(0);
                yield return new TestCaseData(typeof(SpellService)).Returns(0);
                yield return new TestCaseData(typeof(Halo)).Returns(5699.2426069384737d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(1244.9801811273364d);
                yield return new TestCaseData(typeof(Smite)).Returns(2786.3842149040388d);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(5533.2452494548288d);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(5274.1801233861388d);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(2989.5926466902674d);
                yield return new TestCaseData(typeof(HolyFire)).Returns(8334.5878256760934d);
                yield return new TestCaseData(typeof(CosmicRipple)).Returns(0.0d);
                yield return new TestCaseData(typeof(Lightwell)).Returns(0.0d);
            }
        }

        public static IEnumerable GetAverageHealing
        {
            get
            {
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(14346.494534438052d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(141529.34699055561d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(14976.650475191071d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(5762.533710516821d);
                yield return new TestCaseData(typeof(Halo)).Returns(19505.21516262697d);
                yield return new TestCaseData(typeof(Heal)).Returns(6523.8531439625349d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(8524.6559624413458d);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(48036.078169535031d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(31666.255348482122d);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(18910.2498931542d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(15350.714762926829d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(5717.6867577699904d);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(10147.405014115229d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(9402.9835023456544d);
                yield return new TestCaseData(typeof(Renew)).Returns(3934.747597867608d);
                yield return new TestCaseData(typeof(Smite)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(0);
                yield return new TestCaseData(typeof(HolyFire)).Returns(0);
                yield return new TestCaseData(typeof(CosmicRipple)).Returns(5126.8283858823706d);
                yield return new TestCaseData(typeof(Lightwell)).Returns(26072.770273608759d);
            }
        }

        public static IEnumerable GetAverageOverhealing
        {
            get
            {
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(2368.5171566233998d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(64781.654453403004d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(11767.3682305073d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(700.60414336029999d);
                yield return new TestCaseData(typeof(Halo)).Returns(11250.406348926101d);
                yield return new TestCaseData(typeof(Heal)).Returns(2868.3915205387002d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(1504.3510521955d);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(22007.780345389201d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(15135.777386489999d);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(3376.4323615943999d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(155.05772487799999d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(3504.3886579881d);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(3781.7713951026999d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(210.53607882770001d);
                yield return new TestCaseData(typeof(Renew)).Returns(2254.8820983218002d);
                yield return new TestCaseData(typeof(Smite)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(0);
                yield return new TestCaseData(typeof(HolyFire)).Returns(0);
                yield return new TestCaseData(typeof(CosmicRipple)).Returns(1559.1762905421999d);
                yield return new TestCaseData(typeof(Lightwell)).Returns(8369.4625845270002d);
            }
        }

        public static IEnumerable GetAverageRawHealing
        {
            get
            {
                yield return new TestCaseData(typeof(CircleOfHealing)).Returns(16715.01169106146d);
                yield return new TestCaseData(typeof(DivineHymn)).Returns(206311.00144395864d);
                yield return new TestCaseData(typeof(DivineStar)).Returns(26744.018705698338d);
                yield return new TestCaseData(typeof(FlashHeal)).Returns(6463.1378538770987d);
                yield return new TestCaseData(typeof(Halo)).Returns(30755.62151155309d);
                yield return new TestCaseData(typeof(Heal)).Returns(9392.2446645012024d);
                yield return new TestCaseData(typeof(HolyNova)).Returns(10029.007014636878d);
                yield return new TestCaseData(typeof(HolyWordSalvation)).Returns(70043.858514924228d);
                yield return new TestCaseData(typeof(HolyWordSanctify)).Returns(46802.032734972097d);
                yield return new TestCaseData(typeof(HolyWordSerenity)).Returns(22286.682254748615d);
                yield return new TestCaseData(typeof(Mindgames)).Returns(15505.772487804878d);
                yield return new TestCaseData(typeof(PowerWordShield)).Returns(9222.0754157580486d);
                yield return new TestCaseData(typeof(PrayerOfHealing)).Returns(13929.176409217886d);
                yield return new TestCaseData(typeof(PrayerOfMending)).Returns(9613.519581173352d);
                yield return new TestCaseData(typeof(Renew)).Returns(6189.6296961894104d);
                yield return new TestCaseData(typeof(Smite)).Returns(0);
                yield return new TestCaseData(typeof(HolyWordChastise)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordPain)).Returns(0);
                yield return new TestCaseData(typeof(ShadowWordDeath)).Returns(0);
                yield return new TestCaseData(typeof(HolyFire)).Returns(0);
                yield return new TestCaseData(typeof(CosmicRipple)).Returns(6686.0046764245835d);
                yield return new TestCaseData(typeof(Lightwell)).Returns(34442.232858135743d);
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
