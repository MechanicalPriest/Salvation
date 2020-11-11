using NUnit.Framework;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common.Traits;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.Common.Traits
{
    [TestFixture]
    class LetGoOfThePastTest : BaseTest
    {
        private ISpellService _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService gameStateService = new GameStateService();
            _spell = new LetGoOfThePast(gameStateService);
            _gameState = GetGameState();
        }

        [Test]
        public void GetAverageVersatilityPercent_Throws_Without_PlaystyleEntries()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();

            // Act
            var methodCall = new TestDelegate(
                () => _spell.GetAverageVersatilityPercent(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void GetAverageVersatilityPercent_Adds_Average_VersPercent()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var gamestate = gameStateService.CloneGameState(_gameState);
            gameStateService.OverridePlaystyle(gamestate, new Core.Profile.Model.PlaystyleEntry("LetGoOfThePastAverageStacks", 2.5));
            gameStateService.OverridePlaystyle(gamestate, new Core.Profile.Model.PlaystyleEntry("LetGoOfThePastAverageUptime", 0.9));

            // Act
            var value = _spell.GetAverageVersatilityPercent(gamestate, null);

            // Assert
            Assert.AreEqual(0.0225d, value);
        }

        [Test]
        public void GetDuration()
        {
            // Arrange

            // Act
            var value = _spell.GetDuration(_gameState, null);

            // Assert
            Assert.AreEqual(4.0, value);
        }

        [Test]
        public void GetUptime()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var gamestate = gameStateService.CloneGameState(_gameState);
            gameStateService.OverridePlaystyle(gamestate, new Core.Profile.Model.PlaystyleEntry("LetGoOfThePastAverageUptime", 0.9));

            // Act
            var value = _spell.GetUptime(gamestate, null);

            // Assert
            Assert.AreEqual(54.0d, value);
        }
    }
}
