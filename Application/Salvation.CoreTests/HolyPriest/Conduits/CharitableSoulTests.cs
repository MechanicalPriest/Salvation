using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Linq;

namespace Salvation.CoreTests.HolyPriest.Conduits
{
    [TestFixture]
    class CharitableSoulTests : BaseTest
    {
        private GameState _gameState;

        [OneTimeSetUp]
        public void InitOnce()
        {
            _gameState = GetGameState();
        }

        [Test]
        public void CS_Generates_Cast_Result()
        {
            IGameStateService gameStateService = new GameStateService();
            var profileService = new ProfileService();
            var spellService = new PowerWordShield(gameStateService);
            var gamestate1 = gameStateService.CloneGameState(_gameState);
            var gamestate2 = gameStateService.CloneGameState(_gameState);

            profileService.AddActiveConduit(gamestate1.Profile, Conduit.CharitableSoul, 0);
            gameStateService.OverridePlaystyle(gamestate1,
                new Core.Profile.Model.PlaystyleEntry("CharitableSoulAllyCasts", 0.8));

            // Act
            var resultWith = spellService.GetCastResults(gamestate1, null);
            var resultWithout = spellService.GetCastResults(gamestate2, null);

            // Assert
            Assert.NotNull(resultWithout);
            Assert.NotNull(resultWith);
            Assert.AreEqual(0, resultWithout.AdditionalCasts.Count);
            Assert.AreEqual(197.01474528677767d, resultWith.AdditionalCasts[0].Healing);
            Assert.AreEqual(317.76571820448015d, resultWith.AdditionalCasts[0].RawHealing);
        }

        [Test]
        public void CS_Throws_Without_PlaystyelEntry()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var profileService = new ProfileService();
            var spellService = new PowerWordShield(gameStateService);
            var gamestate1 = gameStateService.CloneGameState(_gameState);

            gamestate1.Profile.PlaystyleEntries.RemoveAll(p => p.Name == "CharitableSoulAllyCasts");
            profileService.AddActiveConduit(gamestate1.Profile, Conduit.CharitableSoul, 0);

            // Act
            var methodCall = new TestDelegate(
                () => spellService.GetCastResults(gamestate1, null));

            // Assert
            Assert.Throws<ArgumentNullException>(methodCall);
        }
    }
}
