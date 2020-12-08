using NUnit.Framework;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common.Traits;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.Common.Traits
{
    [TestFixture]
    class BronsCallToActionTests : BaseTest
    {
        private ISpellService _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService gameStateService = new GameStateService();
            _spell = new BronsCallToAction(gameStateService);
            _gameState = GetGameState();
        }

        [Test]
        public void GetAverageRawHealing_Throws_Without_PlaystyleEntries()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();

            // Act
            var methodCall = new TestDelegate(
                () => _spell.GetAverageRawHealing(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void GetActualCastsPerMinute_Throws_Without_PlaystyleEntries()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();

            // Act
            var methodCall = new TestDelegate(
                () => _spell.GetActualCastsPerMinute(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void GetAverageRawHealing()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var gamestate = gameStateService.CloneGameState(_gameState);
            gameStateService.OverridePlaystyle(gamestate, new Core.Profile.Model.PlaystyleEntry("BronsCallToActionSpellpower", 1.15));
            
            // Act
            var value = _spell.GetAverageRawHealing(gamestate, null);

            // Assert
            Assert.AreEqual(1952.0864099100004d, value);
        }

        [Test]
        public void GetActualCastsPerMinute()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var gamestate = gameStateService.CloneGameState(_gameState);
            gameStateService.OverridePlaystyle(gamestate, new Core.Profile.Model.PlaystyleEntry("BronsCallToActionCastsPerProc", 5.25));
            gameStateService.OverridePlaystyle(gamestate, new Core.Profile.Model.PlaystyleEntry("BronsCallToActionProcsPerMinute", 0.4));

            // Act
            var value = _spell.GetActualCastsPerMinute(gamestate, null);

            // Assert
            Assert.AreEqual(2.1000000000000001d, value);
        }

        [Test]
        public void TriggersMastery()
        {
            // Arrange
            // Act
            var value = ((BronsCallToAction)_spell).TriggersMastery(_gameState, null);

            // Assert
            Assert.IsFalse(value);
        }
    }
}
