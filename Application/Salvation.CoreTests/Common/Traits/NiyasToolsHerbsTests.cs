using NUnit.Framework;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common.Traits;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.Common.Traits
{
    [TestFixture]
    class NiyasToolsHerbsTests : BaseTest
    {
        private ISpellService _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService gameStateService = new GameStateService();
            _spell = new NiyasToolsHerbs(gameStateService);
            _gameState = GetGameState();
        }

        [Test]
        public void GetAverageHastePercent_Throws_Without_PlaystyleEntries()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();

            // Act
            var methodCall = new TestDelegate(
                () => _spell.GetAverageHastePercent(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void GetAverageHastePercent()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var gamestate = gameStateService.CloneGameState(_gameState);
            gameStateService.OverridePlaystyle(gamestate, new Core.Profile.Model.PlaystyleEntry("NiyasToolsHerbsUptime", .775));

            // Act
            var value = _spell.GetAverageHastePercent(gamestate, null);

            // Assert
            Assert.AreEqual(0.038750000000000007d, value);
        }

        [Test]
        public void GetUptime()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var gamestate = gameStateService.CloneGameState(_gameState);
            gameStateService.OverridePlaystyle(gamestate, new Core.Profile.Model.PlaystyleEntry("NiyasToolsHerbsUptime", .775));

            // Act
            var value = _spell.GetUptime(gamestate, null);

            // Assert
            Assert.AreEqual(0.775d, value);
        }

        [Test]
        public void GetDuration()
        {
            // Arrange
            // Act
            var value = _spell.GetDuration(_gameState, null);

            // Assert
            Assert.AreEqual(10.0d, value);
        }
    }
}
