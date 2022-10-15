using Newtonsoft.Json;
using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
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
    [TestFixture]
    class DivineImageSpellTests : BaseTest
    {
        private List<ISpellService> Spells { get; set; }
        private GameState _gameState;
        private IGameStateService _gameStateService;

        [OneTimeSetUp]
        public void InitOnce()
        {
            Spells = new List<ISpellService>();

            _gameStateService = new GameStateService();
            IConstantsService constantsService = new ConstantsService();

            // Load this from somewhere that doesn't change
            var constants = constantsService.ParseConstants(
                File.ReadAllText(Path.Combine("TestData", "BaseTests_constants.json")));
            var profile = JsonConvert.DeserializeObject<PlayerProfile>(
                File.ReadAllText(Path.Combine("TestData", "BaseTests_profile.json")));

            Spells.Add(new DivineImageHealingLight(_gameStateService,
                new FlashHeal(_gameStateService, null, null),
                new Heal(_gameStateService, null, null),
                new HolyWordSerenity(_gameStateService,
                    new FlashHeal(_gameStateService, null, null), new Heal(_gameStateService, null, null),
                    new PrayerOfMending(_gameStateService))));

            Spells.Add(new DivineImageTranquilLight(_gameStateService,
                new Renew(_gameStateService)));

            Spells.Add(new DivineImageDazzlingLights(_gameStateService,
                new PrayerOfHealing(_gameStateService),
                new HolyWordSanctify(_gameStateService,
                    new PrayerOfHealing(_gameStateService), new Renew(_gameStateService),
                    new CircleOfHealing(_gameStateService)),
                new DivineStar(_gameStateService),
                new Halo(_gameStateService),
                new DivineHymn(_gameStateService),
                new CircleOfHealing(_gameStateService)));

            Spells.Add(new DivineImageBlessedLight(_gameStateService,
                new PrayerOfMending(_gameStateService)));

            _gameState = _gameStateService.CreateValidatedGameState(profile, constants);
        }

        [TestCaseSource(typeof(DivineImageSpellTestsData), nameof(DivineImageSpellTestsData.GetAverageRawHealing))]
        public double GetAverageRawHealing(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();
            var spellData = _gameStateService.GetSpellData(_gameState, (Spell)spellService.SpellId);
            spellData.Overrides[Override.AllowedDuration] = 15;

            // Act
            var result = spellService.GetAverageRawHealing(_gameState, spellData);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(DivineImageSpellTestsData), nameof(DivineImageSpellTestsData.GetActualCastsPerMinute))]
        public double GetActualCastsPerMinute(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();
            var spellData = _gameStateService.GetSpellData(_gameState, (Spell)spellService.SpellId);
            spellData.Overrides[Override.AllowedDuration] = 15;

            // Act
            var result = spellService.GetActualCastsPerMinute(_gameState, spellData);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(DivineImageSpellTestsData), nameof(DivineImageSpellTestsData.GetMaximumCastsPerMinute))]
        public double GetMaximumCastsPerMinute(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();
            var spellData = _gameStateService.GetSpellData(_gameState, (Spell)spellService.SpellId);
            spellData.Overrides[Override.AllowedDuration] = 15;

            // Act
            var result = spellService.GetMaximumCastsPerMinute(_gameState, spellData);

            // Assert
            return result;
        }

        [TestCaseSource(typeof(DivineImageSpellTestsData), nameof(DivineImageSpellTestsData.GetActualCastsPerMinute_NoOvveride_Throws))]
        public bool GetActualCastsPerMinute_NoOvveride_Throws(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();
            var spellData = _gameStateService.GetSpellData(_gameState, (Spell)spellService.SpellId);

            // Act
            var methodCall = new TestDelegate(
                () => spellService.GetActualCastsPerMinute(_gameState, spellData));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            return true;
        }
    }

    public class DivineImageSpellTestsData
    {
        public static IEnumerable GetAverageRawHealing
        {
            get
            {
                yield return new TestCaseData(typeof(DivineImageHealingLight)).Returns(3140.2601303414635d);
                yield return new TestCaseData(typeof(DivineImageTranquilLight)).Returns(3766.3892088828825d);
                yield return new TestCaseData(typeof(DivineImageDazzlingLights)).Returns(7654.3840677073185d);
                yield return new TestCaseData(typeof(DivineImageBlessedLight)).Returns(18841.560782048778d);
            }
        }

        public static IEnumerable GetActualCastsPerMinute
        {
            get
            {
                yield return new TestCaseData(typeof(DivineImageHealingLight)).Returns(1.2908797822093376d);
                yield return new TestCaseData(typeof(DivineImageTranquilLight)).Returns(0.37314282352941175d);
                yield return new TestCaseData(typeof(DivineImageDazzlingLights)).Returns(3.6909356396092057d);
                yield return new TestCaseData(typeof(DivineImageBlessedLight)).Returns(1.160433176470588d);
            }
        }

        public static IEnumerable GetMaximumCastsPerMinute
        {
            get
            {
                yield return new TestCaseData(typeof(DivineImageHealingLight)).Returns(5.1635191288373505d);
                yield return new TestCaseData(typeof(DivineImageTranquilLight)).Returns(1.492571294117647d);
                yield return new TestCaseData(typeof(DivineImageDazzlingLights)).Returns(14.763742558436823d);
                yield return new TestCaseData(typeof(DivineImageBlessedLight)).Returns(4.6417327058823519d);
            }
        }

        public static IEnumerable GetActualCastsPerMinute_NoOvveride_Throws
        {
            get
            {
                yield return new TestCaseData(typeof(DivineImageHealingLight)).Returns(true);
                yield return new TestCaseData(typeof(DivineImageTranquilLight)).Returns(true);
                yield return new TestCaseData(typeof(DivineImageDazzlingLights)).Returns(true);
                yield return new TestCaseData(typeof(DivineImageBlessedLight)).Returns(true);
            }
        }
    }
}