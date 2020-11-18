using NUnit.Framework;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common.Traits;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.Common.Traits
{
    [TestFixture]
    class ValiantStrikesTests : BaseTest
    {
        private ISpellService _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService gameStateService = new GameStateService();
            _spell = new ValiantStrikes(gameStateService);
            _gameState = GetGameState();
        }

        [Test]
        public void GetAverageHealing_Throws_Without_PlaystyleEntries()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();

            // Act
            var methodCall = new TestDelegate(
                () => _spell.GetAverageHealing(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void GetAverageHealing_Returns_Healing()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var gamestate = gameStateService.CloneGameState(_gameState);
            gameStateService.OverridePlaystyle(gamestate, new Core.Profile.Model.PlaystyleEntry("ValiantStrikesEventsPerMinute", 120));
            
            // Act
            var value = _spell.GetAverageHealing(gamestate, null);

            // Assert
            Assert.AreEqual(189.60240000000019d, value);
        }
    }
}
