using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.HolyPriest.Spells
{
    [TestFixture]
    public class GuardianSpiritTests : BaseTest
    {
        private GameState _gameState;
        private ISpellService _spell;

        [OneTimeSetUp]
        public void InitOnce()
        {
            IGameStateService gameStateService = new GameStateService();
            _spell = new GuardianSpirit(gameStateService);

            _gameState = GetGameState();
        }

        [Test]
        public void GetAverageHealingBonus()
        {
            // Arrange

            // Act
            var result = _spell.GetAverageHealingMultiplier(_gameState, null);

            // Assert
            Assert.AreEqual(1.019378673383711166d, result);
        }

        [Test]
        public void GetUptime()
        {
            // Arrange

            // Act
            var result = _spell.GetUptime(_gameState, null);

            // Assert
            Assert.AreEqual(0.032297788972851946d, result);
        }

        [Test]
        public void GetDuration()
        {
            // Arrange

            // Act
            var result = _spell.GetDuration(_gameState, null);

            // Assert
            Assert.AreEqual(10.0d, result);
        }

        [Test]
        public void GetMaximumCastsPerMinute()
        {
            // Arrange

            // Act
            var result = _spell.GetMaximumCastsPerMinute(_gameState, null);

            // Assert
            Assert.AreEqual(0.48446683459277917d, result);
        }

        [Test]
        public void GetActualCastsPerMinute()
        {
            // Arrange

            // Act
            var result = _spell.GetActualCastsPerMinute(_gameState, null);

            // Assert
            Assert.AreEqual(0.19378673383711167d, result);
        }

        [Test]
        public void GetMinimumHealTargets()
        {
            // Arrange

            // Act
            var result = _spell.GetMinimumHealTargets(_gameState, null);

            // Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void GetMaximumHealTargets()
        {
            // Arrange

            // Act
            var result = _spell.GetMaximumHealTargets(_gameState, null);

            // Assert
            Assert.AreEqual(1, result);
        }
    }
}
