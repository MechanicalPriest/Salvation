using Newtonsoft.Json;
using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System.IO;
using System.Linq;

namespace Salvation.CoreTests.HolyPriest.Conduits
{
    [TestFixture]
    class CourageousAscensionTests
    {
        private GameState _gameState;

        [OneTimeSetUp]
        public void InitOnce()
        {
            IConstantsService constantsService = new ConstantsService();

            // Load this from somewhere that doesn't change
            var basePath = @"HolyPriest" + Path.DirectorySeparatorChar + "TestData";
            var constants = constantsService.ParseConstants(
                File.ReadAllText(Path.Combine(basePath, "SpellServiceTests_constants.json")));
            var profile = JsonConvert.DeserializeObject<PlayerProfile>(
                File.ReadAllText(Path.Combine(basePath, "SpellServiceTests_profile.json")));

            _gameState = new GameState(profile, constants);
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
            Assert.AreEqual(2500.6263394500002d, resultWithout);
            Assert.AreEqual(3125.7829243125002d, resultWith);
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
            Assert.AreEqual(6168.0327210344913d, resultWithout);
            Assert.AreEqual(6227.9165338600687d, resultWith);
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
            Assert.AreEqual(4903.5860132224225d, resultWithout);
            Assert.AreEqual(4951.1936444187559d, resultWith);
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
