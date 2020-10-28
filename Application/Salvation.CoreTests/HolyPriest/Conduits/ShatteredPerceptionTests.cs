using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.State;

namespace Salvation.CoreTests.HolyPriest.Conduits
{
    [TestFixture]
    class ShatteredPerceptionTests : BaseTest
    {
        private GameState _gameState;

        [OneTimeSetUp]
        public void InitOnce()
        {
            _gameState = GetGameState();
        }

        [Test]
        public void SP_Increases_GetAverageDamage()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new Mindgames(gameStateService);
            var gamestate1 = gameStateService.CloneGameState(_gameState);
            var gamestate2 = gameStateService.CloneGameState(_gameState);

            gamestate1.Profile.Conduits.Add(Conduit.ShatteredPerceptions, 0);

            // Act
            var resultWith = spellService.GetAverageDamage(gamestate1, null);
            var resultWithout = spellService.GetAverageDamage(gamestate2, null);

            // Assert
            Assert.AreEqual(5688.6862500000007d, resultWithout);
            Assert.AreEqual(6428.2154625000003d, resultWith);
        }

        [Test]
        public void SP_Increases_GetDuration()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new Mindgames(gameStateService);
            var gamestate1 = gameStateService.CloneGameState(_gameState);
            var gamestate2 = gameStateService.CloneGameState(_gameState);

            gamestate1.Profile.Conduits.Add(Conduit.ShatteredPerceptions, 0);

            // Act
            var resultWith = spellService.GetDuration(gamestate1, null);
            var resultWithout = spellService.GetDuration(gamestate2, null);

            // Assert
            Assert.AreEqual(5.0d, resultWithout);
            Assert.AreEqual(7.0d, resultWith);
        }
    }
}
