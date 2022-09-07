using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System.Linq;

namespace Salvation.CoreTests.HolyPriest.Conduits
{
    [TestFixture]
    class FaeFermataTests : BaseTest
    {
        private GameState _gameState;

        [OneTimeSetUp]
        public void InitOnce()
        {
            _gameState = GetGameState();
        }

        [Test]
        public void FF_Present_Increases_GetAverageRawHealing()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var profileService = new ProfileService();
            var spellService = new FaeGuardians(gameStateService, null);
            var gamestate1 = gameStateService.CloneGameState(_gameState);
            var gamestate2 = gameStateService.CloneGameState(_gameState);

            profileService.AddActiveConduit(gamestate1.Profile, Conduit.FaeFermata, 0);

            // Act
            var resultWith = spellService.GetAverageRawHealing(gamestate1, null);
            var resultWithout = spellService.GetAverageRawHealing(gamestate2, null);

            // Assert
            Assert.AreEqual(16000.0d, resultWithout);
            Assert.AreEqual(18432.0d, resultWith);
        }

        [Test]
        public void FF_Present_Increases_Bonus()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var profileService = new ProfileService();
            var spellService = new FaeGuardians(gameStateService, null);
            var gamestateR1 = gameStateService.CloneGameState(_gameState);
            var gamestateR2 = gameStateService.CloneGameState(_gameState);

            profileService.AddActiveConduit(gamestateR1.Profile, Conduit.FaeFermata, 0);
            profileService.AddActiveConduit(gamestateR2.Profile, Conduit.FaeFermata, 1);

            // Act
            var resultR1 = spellService.GetFaeFermataBonus(gamestateR1);
            var resultR2 = spellService.GetFaeFermataBonus(gamestateR2);

            // Assert
            Assert.AreEqual(3.04d, resultR1);
            Assert.AreEqual(3.3439999999999999d, resultR2);
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
            var profileService = new ProfileService();
            var spellService = new FaeGuardians(gameStateService, new DivineHymn(gameStateService));
            var gamestateWithout = gameStateService.CloneGameState(_gameState);
            var gamestateWith = gameStateService.CloneGameState(_gameState);

            profileService.AddActiveConduit(gamestateWith.Profile, Conduit.FaeFermata, 0);

            // Act
            var resultWithout = spellService.GetCastResults(gamestateWithout);
            var dhResultWithout = resultWithout.AdditionalCasts.Where(c => c.SpellId == (int)Spell.DivineHymn).FirstOrDefault();

            var resultWith = spellService.GetCastResults(gamestateWith);
            var dhResultWith = resultWith.AdditionalCasts.Where(c => c.SpellId == (int)Spell.DivineHymn).FirstOrDefault();

            // Assert
            Assert.IsNotNull(resultWithout);
            Assert.IsNotNull(resultWith);
            Assert.IsNotNull(dhResultWithout);
            Assert.IsNotNull(dhResultWith);
            Assert.AreEqual(12832.846312104002d, dhResultWithout.RawHealing);
            Assert.AreEqual(18684.624230423429d, dhResultWith.RawHealing);
        }
    }
}
