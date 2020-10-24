using Newtonsoft.Json;
using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Salvation.CoreTests.HolyPriest.Spells
{
    [TestFixture]
    public class SpellServiceBaseTests
    {
        private GameState _gameState;
        [OneTimeSetUp]
        public void InitOnce()
        {
            IConstantsService constantsService = new ConstantsService();

            // Load this from somewhere that doesn't change
            var basePath = @"HolyPriest" + Path.DirectorySeparatorChar + "TestData";
            var constants = constantsService.ParseConstants(
                File.ReadAllText(Path.Combine(basePath, "SpellServiceTests_constants.json")));
            var profile = JsonConvert.DeserializeObject<PlayerProfile>(
                File.ReadAllText(Path.Combine(basePath, "SpellServiceTests_profile.json")));

            _gameState = new GameState(profile, constants);
        }

        [Test]
        public void GetNumberOfHealingTargets_Throws_NoSpelldata()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellService(gameStateService);

            // Act
            var methodCall = new TestDelegate(
                () => spellService.GetNumberOfHealingTargets(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void GetNumberOfHealingTargets_Override_Targets()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellServiceWithSpell(gameStateService);

            // Act
            var spellData = gameStateService.GetSpellData(_gameState, Spell.FlashHeal);
            spellData.Overrides.Add(Override.NumberOfHealingTargets, 12345);
            var result = spellService.GetNumberOfHealingTargets(_gameState, spellData);

            // Assert
            Assert.AreEqual(12345, result);
        }

        [Test]
        public void GetDuration_Throws_NoSpelldata()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellService(gameStateService);

            // Act
            var methodCall = new TestDelegate(
                () => spellService.GetDuration(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void GetActualManaCost_Throws_NoSpelldata()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellService(gameStateService);

            // Act
            var methodCall = new TestDelegate(
                () => spellService.GetActualManaCost(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void GetHastedCastTime_Throws_NoSpelldata()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellService(gameStateService);

            // Act
            var methodCall = new TestDelegate(
                () => spellService.GetHastedCastTime(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void GetMaximumCastsPerMinute_Throws()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellService(gameStateService);

            // Act
            var methodCall = new TestDelegate(
                () => spellService.GetMaximumCastsPerMinute(_gameState, null));

            // Assert
            Assert.Throws<NotImplementedException>(methodCall);
        }

        [Test]
        public void GetHastedGcd_Throws_NoSpelldata()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellService(gameStateService);

            // Act
            var methodCall = new TestDelegate(
                () => spellService.GetHastedGcd(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void GetHastedCooldown_Throws_NoSpelldata()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellService(gameStateService);

            // Act
            var methodCall = new TestDelegate(
                () => spellService.GetHastedCooldown(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void GetNumberOfDamageTargets_Throws_NoSpelldata()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellService(gameStateService);

            // Act
            var methodCall = new TestDelegate(
                () => spellService.GetNumberOfDamageTargets(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }


        [Test]
        public void GetNumberOfDamageTargets_Override_Targets()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellServiceWithSpell(gameStateService);

            // Act
            var spellData = gameStateService.GetSpellData(_gameState, Spell.FlashHeal);
            spellData.Overrides.Add(Override.NumberOfDamageTargets, 1234);
            var result = spellService.GetNumberOfDamageTargets(_gameState, spellData);

            // Assert
            Assert.AreEqual(1234, result);
        }

        [Test]
        public void GetAverageOverhealing_Throws_NoSpelldata()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellService(gameStateService);

            // Act
            var methodCall = new TestDelegate(
                () => spellService.GetAverageOverhealing(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }
    }

    public class SpellServiceWithSpell : SpellService
    {
        public SpellServiceWithSpell(IGameStateService gameStateService) 
            : base(gameStateService)
        {
            SpellId = (int)Spell.FlashHeal;
        }
    }
}
