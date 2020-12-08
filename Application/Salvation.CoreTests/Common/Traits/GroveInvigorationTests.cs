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
    class GroveInvigorationTests : BaseTest
    {
        private ISpellService _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService gameStateService = new GameStateService();
            _spell = new GroveInvigoration(gameStateService, new FaeGuardians(gameStateService, new DivineHymn(gameStateService)));
            _gameState = GetGameState();
        }

        [Test]
        public void GetAverageMastery()
        {
            // Arrange

            // Act
            var value = _spell.GetAverageMastery(_gameState, null);

            // Assert
            Assert.AreEqual(137.67008025692695d, value);
        }

        [Test]
        public void GetUptime()
        {
            // Arrange
            // Act
            var value = _spell.GetUptime(_gameState, null);

            // Assert
            Assert.AreEqual(0.6d, value);
        }

        [Test]
        public void GetDuration()
        {
            // Arrange
            // Act
            var value = _spell.GetDuration(_gameState, null);

            // Assert
            Assert.AreEqual(30.0d, value);
        }
    }
}
