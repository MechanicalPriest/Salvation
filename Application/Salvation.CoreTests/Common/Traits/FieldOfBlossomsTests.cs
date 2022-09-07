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
    class FieldOfBlossomsTests : BaseTest
    {
        private ISpellService _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService gameStateService = new GameStateService();
            _spell = new FieldOfBlossoms(gameStateService, new FaeGuardians(gameStateService, new DivineHymn(gameStateService)));
            _gameState = GetGameState();
        }

        [Test]
        public void GetAverageHastePercent()
        {
            // Arrange

            // Act
            var value = _spell.GetAverageHastePercent(_gameState, null);

            // Assert
            Assert.AreEqual(0.036801007556675057d, value);
        }

        [Test]
        public void GetUptime()
        {
            // Arrange
            // Act
            var value = _spell.GetUptime(_gameState, null);

            // Assert
            Assert.AreEqual(0.24534005037783371d, value);
        }

        [Test]
        public void GetDuration()
        {
            // Arrange
            // Act
            var value = _spell.GetDuration(_gameState, null);

            // Assert
            Assert.AreEqual(18.0d, value);
        }

        [Test]
        public void GetActualCastsPerMinute()
        {
            // Arrange
            // Act
            var value = _spell.GetActualCastsPerMinute(_gameState, null);

            // Assert
            Assert.AreEqual(0.81780016792611243d, value);
        }
    }
}
