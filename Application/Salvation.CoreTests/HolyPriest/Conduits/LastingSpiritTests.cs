using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.CoreTests.HolyPriest.Conduits
{
    [TestFixture]
    class LastingSpiritTests : BaseTest
    {
        GameState _gameState;

        [OneTimeSetUp]
        public void InitOnce()
        {
            _gameState = GetGameState();
        }

        [Test]
        public void LS_Increases_GetAverageHealingBonus()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var profileService = new ProfileService();
            var spellService = new GuardianSpirit(gameStateService);
            var gamestate1 = gameStateService.CloneGameState(_gameState);
            var gamestate2 = gameStateService.CloneGameState(_gameState);

            profileService.AddActiveConduit(gamestate1.Profile, Conduit.LastingSpirit, 0);

            // Act
            var resultWith = spellService.GetAverageHealingBonus(gamestate1, null);
            var resultWithout = spellService.GetAverageHealingBonus(gamestate2, null);

            // Assert
            Assert.AreEqual(0.019378673383711166d, resultWithout);
            Assert.AreEqual(0.028292863140218307d, resultWith);
        }

        [Test]
        public void LS_Increases_GetDuration()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var profileService = new ProfileService();
            var spellService = new GuardianSpirit(gameStateService);
            var gamestate1 = gameStateService.CloneGameState(_gameState);
            var gamestate2 = gameStateService.CloneGameState(_gameState);

            profileService.AddActiveConduit(gamestate1.Profile, Conduit.LastingSpirit, 0);

            // Act
            var resultWith = spellService.GetDuration(gamestate1, null);
            var resultWithout = spellService.GetDuration(gamestate2, null);

            // Assert
            Assert.AreEqual(10.0d, resultWithout);
            Assert.AreEqual(12.0d, resultWith);
        }

        [Test]
        public void LS_Increases_GetUptime()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var profileService = new ProfileService();
            var spellService = new GuardianSpirit(gameStateService);
            var gamestate1 = gameStateService.CloneGameState(_gameState);
            var gamestate2 = gameStateService.CloneGameState(_gameState);

            profileService.AddActiveConduit(gamestate1.Profile, Conduit.LastingSpirit, 0);

            // Act
            var resultWith = spellService.GetUptime(gamestate1, null);
            var resultWithout = spellService.GetUptime(gamestate2, null);

            // Assert
            Assert.AreEqual(0.032297788972851946d, resultWithout);
            Assert.AreEqual(0.038757346767422339d, resultWith);
        }
    }
}
