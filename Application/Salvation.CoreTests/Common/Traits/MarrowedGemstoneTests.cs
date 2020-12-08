using NUnit.Framework;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common.Traits;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.Common.Traits
{
    [TestFixture]
    class MarrowedGemstoneTests : BaseTest
    {
        private ISpellService _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService gameStateService = new GameStateService();
            _spell = new MarrowedGemstone(gameStateService);
            _gameState = GetGameState();
        }

        [Test]
        public void GetAverageCriticalStrikePercent_Throws_Without_PlaystyleEntries()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();

            // Act
            var methodCall = new TestDelegate(
                () => _spell.GetAverageCriticalStrikePercent(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void GetAverageCriticalStrikePercent()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var gamestate = gameStateService.CloneGameState(_gameState);
            gameStateService.OverridePlaystyle(gamestate, new Core.Profile.Model.PlaystyleEntry("MarrowedGemstoneEventsPerMinute", 200));
            
            // Act
            var value = _spell.GetAverageCriticalStrikePercent(gamestate, null);

            // Assert
            Assert.AreEqual(0.021071428571428578d, value);
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

        [Test]
        public void GetUptime()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var gamestate = gameStateService.CloneGameState(_gameState);
            gameStateService.OverridePlaystyle(gamestate, new Core.Profile.Model.PlaystyleEntry("MarrowedGemstoneEventsPerMinute", 200));

            // Act
            var value = _spell.GetUptime(gamestate, null);

            // Assert
            Assert.AreEqual(0.11706349206349211d, value);
        }

        [Test]
        public void GetActualCastsPerMinute()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var gamestate = gameStateService.CloneGameState(_gameState);
            gameStateService.OverridePlaystyle(gamestate, new Core.Profile.Model.PlaystyleEntry("MarrowedGemstoneEventsPerMinute", 200));

            // Act
            var value = _spell.GetActualCastsPerMinute(gamestate, null);

            // Assert
            Assert.AreEqual(0.70238095238095266d, value);
        }
    }
}
