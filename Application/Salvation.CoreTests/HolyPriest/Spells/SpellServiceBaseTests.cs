using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.HolyPriest.Spells
{
    [TestFixture]
    public class SpellServiceBaseTests : BaseTest
    {
        private GameState _gameState;

        [OneTimeSetUp]
        public void InitOnce()
        {
            _gameState = GetGameState();
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

        [Test]
        public void GetAverageHealing_Throws_NoSpelldata()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellService(gameStateService);

            // Act
            var methodCall = new TestDelegate(
                () => spellService.GetAverageHealing(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void GetAverageIntellect_Defaults_Zero()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellService(gameStateService);

            // Act
            var result = spellService.GetAverageIntellect(_gameState, null);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void GetAverageCriticalStrike_Defaults_Zero()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellService(gameStateService);

            // Act
            var result = spellService.GetAverageCriticalStrike(_gameState, null);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void GetAverageHaste_Defaults_Zero()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellService(gameStateService);

            // Act
            var result = spellService.GetAverageHaste(_gameState, null);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void GetAverageMastery_Defaults_Zero()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellService(gameStateService);

            // Act
            var result = spellService.GetAverageMastery(_gameState, null);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void GetAverageVersatility_Defaults_Zero()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellService(gameStateService);

            // Act
            var result = spellService.GetAverageVersatility(_gameState, null);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void GetUptime_Defaults_Zero()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellService(gameStateService);

            // Act
            var result = spellService.GetUptime(_gameState, null);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void GetAverageMp5_Defaults_Zero()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellService(gameStateService);

            // Act
            var result = spellService.GetAverageMp5(_gameState, null);

            // Assert
            Assert.AreEqual(0, result);
        }
    }

    public class SpellServiceWithSpell : SpellService
    {
        public SpellServiceWithSpell(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.FlashHeal;
        }
    }
}
