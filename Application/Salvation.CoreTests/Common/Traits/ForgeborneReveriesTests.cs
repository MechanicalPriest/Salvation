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
    class ForgeborneReveriesTests : BaseTest
    {
        private ISpellService _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService gameStateService = new GameStateService();
            _spell = new ForgeborneReveries(gameStateService);
            _gameState = GetGameState();
        }

        [Test]
        public void GetAverageIntellectBonus()
        {
            // Arrange

            // Act
            var value = _spell.GetAverageIntellectBonus(_gameState, null);

            // Assert
            Assert.AreEqual(0.03d, value);
        }

        [Test]
        public void GetUptime()
        {
            // Arrange
            // Act
            var value = _spell.GetUptime(_gameState, null);

            // Assert
            Assert.AreEqual(1.0d, value);
        }
    }
}
