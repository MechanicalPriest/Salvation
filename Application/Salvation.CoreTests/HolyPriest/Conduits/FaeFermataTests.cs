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
    class FaeFermataTests
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
        public void FF_Present_Increases_GetAverageRawHealing()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new FaeGuardians(gameStateService, null);
            var gamestate1 = gameStateService.CloneGameState(_gameState);
            var gamestate2 = gameStateService.CloneGameState(_gameState);

            gamestate1.Profile.Conduits.Add(Conduit.FaeFermata, 0);

            // Act
            var resultWith = spellService.GetAverageRawHealing(gamestate1, null);
            var resultWithout = spellService.GetAverageRawHealing(gamestate2, null);

            // Assert
            Assert.AreEqual(8000.0d, resultWithout);
            Assert.AreEqual(8912.0d, resultWith);
        }

        [Test]
        public void FF_Present_Increases_Bonus()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new FaeGuardians(gameStateService, null);
            var gamestateR1 = gameStateService.CloneGameState(_gameState);
            var gamestateR2 = gameStateService.CloneGameState(_gameState);

            gamestateR1.Profile.Conduits.Add(Conduit.FaeFermata, 0);
            gamestateR2.Profile.Conduits.Add(Conduit.FaeFermata, 1);

            // Act
            var resultR1 = spellService.GetFaeFermataBonus(gamestateR1);
            var resultR2 = spellService.GetFaeFermataBonus(gamestateR2);

            // Assert
            Assert.AreEqual(2.2799999999999998d, resultR1);
            Assert.AreEqual(2.5079999999999996d, resultR2);
        }

        [Test]
        public void FF_Not_Present_No_Bonus()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new FaeGuardians(gameStateService, null);
            var gamestate = gameStateService.CloneGameState(_gameState);

            // Act
            var result = spellService.GetFaeFermataBonus(gamestate);

            // Assert
            Assert.AreEqual(0d, result);
        }

        [Test]
        public void FF_Increases_DH_GetCastResults()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new FaeGuardians(gameStateService, new DivineHymn(gameStateService));
            var gamestateWithout = gameStateService.CloneGameState(_gameState);
            var gamestateWith = gameStateService.CloneGameState(_gameState);

            gamestateWith.Profile.Conduits.Add(Conduit.FaeFermata, 0);

            // Act
            var resultWithout = spellService.GetCastResults(gamestateWithout);
            var dhResultWithout = resultWithout.AdditionalCasts.FirstOrDefault();

            var resultWith = spellService.GetCastResults(gamestateWith);
            var dhResultWith = resultWith.AdditionalCasts.FirstOrDefault();

            // Assert
            Assert.IsNotNull(resultWithout);
            Assert.IsNotNull(resultWith);
            Assert.IsNotNull(dhResultWithout);
            Assert.IsNotNull(dhResultWith);
            Assert.AreEqual(8449.0436318400025d, dhResultWithout.RawHealing);
            Assert.AreEqual(11338.616553929283d, dhResultWith.RawHealing);
        }
    }
}
