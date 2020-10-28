using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.State;

namespace Salvation.CoreTests.HolyPriest.Conduits
{
    [TestFixture]
    class CourageousAscensionTests : BaseTest
    {
        private GameState _gameState;

        [OneTimeSetUp]
        public void InitOnce()
        {
            _gameState = GetGameState();
        }

        [Test]
        public void CA_Increases_GetAverageDamage()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new AscendedBlast(gameStateService);
            var gamestate1 = gameStateService.CloneGameState(_gameState);
            var gamestate2 = gameStateService.CloneGameState(_gameState);

            gamestate1.Profile.Conduits.Add(Conduit.CourageousAscension, 0);

            // Act
            var resultWith = spellService.GetAverageDamage(gamestate1, null);
            var resultWithout = spellService.GetAverageDamage(gamestate2, null);

            // Assert
            Assert.AreEqual(3794.7708990750002d, resultWithout);
            Assert.AreEqual(4743.4636238437506d, resultWith);
        }

        [Test]
        public void CA_Increases_AE_Damage()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new AscendedEruption(gameStateService);
            var gamestate1 = gameStateService.CloneGameState(_gameState);
            var gamestate2 = gameStateService.CloneGameState(_gameState);

            gamestate1.Profile.Conduits.Add(Conduit.CourageousAscension, 0);

            // Act
            var resultWith = spellService.GetAverageDamage(gamestate1, null);
            var resultWithout = spellService.GetAverageDamage(gamestate2, null);

            // Assert
            Assert.AreEqual(9360.1633738978235d, resultWithout);
            Assert.AreEqual(9451.0387464599389d, resultWith);
        }

        [Test]
        public void CA_Increases_AE_Healing()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new AscendedEruption(gameStateService);
            var gamestate1 = gameStateService.CloneGameState(_gameState);
            var gamestate2 = gameStateService.CloneGameState(_gameState);

            gamestate1.Profile.Conduits.Add(Conduit.CourageousAscension, 0);

            // Act
            var resultWith = spellService.GetAverageRawHealing(gamestate1, null);
            var resultWithout = spellService.GetAverageRawHealing(gamestate2, null);

            // Assert
            Assert.AreEqual(7441.3298822487704d, resultWithout);
            Assert.AreEqual(7513.5758034356513d, resultWith);
        }

        [Test]
        public void CA_Increases_Boon_Bonus_Per_Stack()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new AscendedEruption(gameStateService);
            var gamestate1 = gameStateService.CloneGameState(_gameState);
            var gamestate2 = gameStateService.CloneGameState(_gameState);

            gamestate1.Profile.Conduits.Add(Conduit.CourageousAscension, 0);

            // Act
            var resultWith = spellService.GetBoonBonusPerStack(gamestate1);
            var resultWithout = spellService.GetBoonBonusPerStack(gamestate2);

            // Assert
            Assert.AreEqual(3, resultWithout);
            Assert.AreEqual(4, resultWith);
        }
    }
}
