using Newtonsoft.Json;
using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common.Items;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile.Model;
using Salvation.Core.State;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Salvation.CoreTests.Common.Items
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
                File.ReadAllText(Path.Combine("TestData", "SpellServiceTests_profile.json")));

            Spells.Add(new DivineImageHealingLight(_gameStateService,
                new FlashHeal(_gameStateService),
                new Heal(_gameStateService),
                new HolyWordSerenity(_gameStateService, 
                    new FlashHeal(_gameStateService), new Heal(_gameStateService), 
                    new BindingHeal(_gameStateService), new PrayerOfMending(_gameStateService)),
                new BindingHeal(_gameStateService)));

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

        [TestCaseSource(typeof(DivineImageSpellTestsData), nameof(DivineImageSpellTestsData.GetDuration))]
        public double GetDuration(Type t)
        {
            // Arrange
            var spellService = Spells.Where(s => s.GetType() == t).FirstOrDefault();
            var spellData = _gameStateService.GetSpellData(_gameState, (Spell)spellService.SpellId);
            spellData.Overrides[Override.AllowedDuration] = 15;

            // Act
            var result = spellService.GetDuration(_gameState, spellData);

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
                yield return new TestCaseData(typeof(DivineImageHealingLight)).Returns(697.53774037500011d);
            }
        }

        public static IEnumerable GetActualCastsPerMinute
        {
            get
            {
                yield return new TestCaseData(typeof(DivineImageHealingLight)).Returns(2.6838297565063431d);
            }
        }

        public static IEnumerable GetMaximumCastsPerMinute
        {
            get
            {
                yield return new TestCaseData(typeof(DivineImageHealingLight)).Returns(10.735319026025373d);
            }
        }

        public static IEnumerable GetDuration
        {
            get
            {
                yield return new TestCaseData(typeof(DivineImageHealingLight)).Returns(0);
            }
        }

        public static IEnumerable GetActualCastsPerMinute_NoOvveride_Throws
        {
            get
            {
                yield return new TestCaseData(typeof(DivineImageHealingLight)).Returns(true);
            }
        }
    }
}