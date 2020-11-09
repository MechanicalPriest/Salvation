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
            Assert.AreEqual(3798.0811671075003d, resultWithout);
            Assert.AreEqual(4747.6014588843755d, resultWith);
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
            Assert.AreEqual(9368.3284648663339d, resultWithout);
            Assert.AreEqual(9459.2831101562988d, resultWith);
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
            Assert.AreEqual(7447.8211295687361d, resultWithout);
            Assert.AreEqual(7520.1300725742576d, resultWith);
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
