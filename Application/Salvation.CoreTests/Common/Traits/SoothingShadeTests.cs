using NUnit.Framework;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common.Traits;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.Common.Traits
{
    [TestFixture]
    class SoothingShadeTests : BaseTest
    {
        private ISpellService _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService gameStateService = new GameStateService();
            _spell = new SoothingShade(gameStateService);
            _gameState = GetGameState();
        }

        [Test]
        public void GetAverageMastery()
        {
            // Arrange

            // Act
            var value = _spell.GetAverageMastery(_gameState, null);

            // Assert
            Assert.AreEqual(82.500004500000003d, value);
        }

        [Test]
        public void GetUptime()
        {
            // Arrange
            // Act
            var value = _spell.GetUptime(_gameState, null);

            // Assert
            Assert.AreEqual(0.14999999999999999d, value);
        }

        [Test]
        public void GetDuration()
        {
            // Arrange
            // Act
            var value = _spell.GetDuration(_gameState, null);

            // Assert
            Assert.AreEqual(12.0d, value);
        }

        [Test]
        public void GetActualCastsPerMinute()
        {
            // Arrange
            // Act
            var value = _spell.GetActualCastsPerMinute(_gameState, null);

            // Assert
            Assert.AreEqual(0.75d, value);
        }
    }
}
