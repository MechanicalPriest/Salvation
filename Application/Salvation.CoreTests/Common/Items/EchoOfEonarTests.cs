using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common.Items;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.Common.Items
{
    [TestFixture]
    class EchoOfEonarTests : BaseTest
    {
        private EchoOfEonar _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService gameStateService = new GameStateService();

            _spell = new EchoOfEonar(gameStateService);
            _gameState = GetGameState();
        }

        [Test]
        public void GetAverageHaste_Throws_Without_Playstyle()
        {
            // Arrange

            // Act
            var methodCall = new TestDelegate(
                () => _spell.GetAverageHealingBonus(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void GetAverageHealingBonus_Adds_Healing()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var gamestate = gameStateService.CloneGameState(_gameState);
            gameStateService.OverridePlaystyle(gamestate, new Core.Profile.Model.PlaystyleEntry("EchoOfEonarCountAllyBuffs", 1));
            
            // Act
            var value = _spell.GetAverageHealingBonus(gamestate, null);

            // Assert
            Assert.AreEqual(0.043368912718401507d, value);
        }

        [Test]
        public void GetDuration()
        {
            // Arrange

            // Act
            var value = _spell.GetDuration(_gameState, null);

            // Assert
            Assert.AreEqual(10, value);
        }

        [Test]
        public void GetUptime()
        {
            // Arrange

            // Act
            var value = _spell.GetUptime(_gameState, null);

            // Assert
            Assert.AreEqual(0.17347565087360603d, value);
        }
    }
}
