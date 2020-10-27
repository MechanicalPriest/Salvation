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
    class FesteringTransfusion : BaseTest
    {
        private GameState _gameState;

        [OneTimeSetUp]
        public void InitOnce()
        {
            _gameState = GetGameState();
        }

        [Test]
        public void FT_Increases_Duration()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new UnholyTransfusion(gameStateService);
            var gamestate1 = gameStateService.CloneGameState(_gameState);
            var gamestate2 = gameStateService.CloneGameState(_gameState);

            gamestate1.Profile.Conduits.Add(Conduit.FesteringTransfusion, 0);

            // Act
            var resultWith = spellService.GetDuration(gamestate1, null);
            var resultWithout = spellService.GetDuration(gamestate2, null);

            // Assert
            Assert.AreEqual(15.0d, resultWithout);
            Assert.AreEqual(17.0d, resultWith);
        }

        [Test]
        public void FT_Increases_Damage()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new UnholyTransfusion(gameStateService);
            var gamestate1 = gameStateService.CloneGameState(_gameState);
            var gamestate2 = gameStateService.CloneGameState(_gameState);

            gamestate1.Profile.Conduits.Add(Conduit.FesteringTransfusion, 0);

            // Act
            var resultWith = spellService.GetAverageDamage(gamestate1, null);
            var resultWithout = spellService.GetAverageDamage(gamestate2, null);

            // Assert
            Assert.AreEqual(5985.7419759829563d, resultWithout);
            Assert.AreEqual(7190.8713604808581d, resultWith);
        }

        [Test]
        public void FT_Increases_Healing()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new UnholyTransfusion(gameStateService);
            var gamestate1 = gameStateService.CloneGameState(_gameState);
            var gamestate2 = gameStateService.CloneGameState(_gameState);

            gamestate1.Profile.Conduits.Add(Conduit.FesteringTransfusion, 0);

            // Act
            var resultWith = spellService.GetAverageRawHealing(gamestate1, null);
            var resultWithout = spellService.GetAverageRawHealing(gamestate2, null);

            // Assert
            Assert.AreEqual(356.15726874000006d, resultWithout);
            Assert.AreEqual(427.86359884632003d, resultWith);
        }
    }
}
