using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile;
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
            var profileService = new ProfileService();
            var spellService = new AscendedBlast(gameStateService);
            var gamestate1 = gameStateService.CloneGameState(_gameState);
            var gamestate2 = gameStateService.CloneGameState(_gameState);

            profileService.AddActiveConduit(gamestate1.Profile, Conduit.CourageousAscension, 0);

            // Act
            var resultWith = spellService.GetAverageDamage(gamestate1, null);
            var resultWithout = spellService.GetAverageDamage(gamestate2, null);

            // Assert
            Assert.AreEqual(3870.6663170565002d, resultWithout);
            Assert.AreEqual(4838.332896320625d, resultWith);
        }

        [Test]
        public void CA_Increases_AE_Damage()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var profileService = new ProfileService();
            var spellService = new AscendedEruption(gameStateService);
            var gamestate1 = gameStateService.CloneGameState(_gameState);
            var gamestate2 = gameStateService.CloneGameState(_gameState);

            profileService.AddActiveConduit(gamestate1.Profile, Conduit.CourageousAscension, 0);

            // Act
            var resultWith = spellService.GetAverageDamage(gamestate1, null);
            var resultWithout = spellService.GetAverageDamage(gamestate2, null);

            // Assert
            Assert.AreEqual(9547.3666413757801d, resultWithout);
            Assert.AreEqual(9640.0595213891374d, resultWith);
        }

        [Test]
        public void CA_Increases_AE_Healing()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var profileService = new ProfileService();
            var spellService = new AscendedEruption(gameStateService);
            var gamestate1 = gameStateService.CloneGameState(_gameState);
            var gamestate2 = gameStateService.CloneGameState(_gameState);

            profileService.AddActiveConduit(gamestate1.Profile, Conduit.CourageousAscension, 0);

            // Act
            var resultWith = spellService.GetAverageRawHealing(gamestate1, null);
            var resultWithout = spellService.GetAverageRawHealing(gamestate2, null);

            // Assert
            Assert.AreEqual(7590.1564798937452d, resultWithout);
            Assert.AreEqual(7663.8473195043643d, resultWith);
        }

        [Test]
        public void CA_Increases_Boon_Bonus_Per_Stack()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var profileService = new ProfileService();
            var spellService = new AscendedEruption(gameStateService);
            var gamestate1 = gameStateService.CloneGameState(_gameState);
            var gamestate2 = gameStateService.CloneGameState(_gameState);

            profileService.AddActiveConduit(gamestate1.Profile, Conduit.CourageousAscension, 0);

            // Act
            var resultWith = spellService.GetBoonBonusPerStack(gamestate1);
            var resultWithout = spellService.GetBoonBonusPerStack(gamestate2);

            // Assert
            Assert.AreEqual(3, resultWithout);
            Assert.AreEqual(4, resultWith);
        }
    }
}
