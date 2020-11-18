using NUnit.Framework;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common.Traits;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.Common.Traits
{
    [TestFixture]
    class ResonantAccoladesTests : BaseTest
    {
        private ISpellService _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService gameStateService = new GameStateService();
            _spell = new ResonantAccolades(gameStateService);
            _gameState = GetGameState();
        }

        [Test]
        public void GetAverageHealingBonus_Throws_Without_PlaystyleEntries()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();

            // Act
            var methodCall = new TestDelegate(
                () => _spell.GetAverageHealingBonus(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void GetAverageHealingBonus_Returns_Healing()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var gamestate = gameStateService.CloneGameState(_gameState);
            gameStateService.OverridePlaystyle(gamestate, new Core.Profile.Model.PlaystyleEntry("ResonantAccoladesHealingOver70Percent", 0.6));
            
            // Act
            var value = _spell.GetAverageHealingBonus(gamestate, null);

            // Assert
            Assert.AreEqual(0.0096000000000000009d, value);
        }
    }
}
