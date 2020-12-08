using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common.Items;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.Common.Items
{
    [TestFixture]
    class CauterizingShadowsTests : BaseTest
    {
        private CauterizingShadows _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService gameStateService = new GameStateService();

            _spell = new CauterizingShadows(gameStateService, new ShadowWordPain(gameStateService));
            _gameState = GetGameState();
        }

        [Test]
        public void GetActualCastsPerMinute_Throws_Without_Playstyle()
        {
            // Arrange

            // Act
            var methodCall = new TestDelegate(
                () => _spell.GetActualCastsPerMinute(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void GetActualCastsPerMinute()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var gamestate = gameStateService.CloneGameState(_gameState);
            gameStateService.OverridePlaystyle(gamestate, new Core.Profile.Model.PlaystyleEntry("CauterizingShadowsSwpExpiryPercent", .9));

            // Act
            var value = _spell.GetActualCastsPerMinute(gamestate, null);

            // Assert
            Assert.AreEqual(40.658181818181824d, value);
        }

        [Test]
        public void GetMaximumCastsPerMinute()
        {
            // Arrange
            
            // Act
            var value = _spell.GetMaximumCastsPerMinute(_gameState, null);

            // Assert
            Assert.AreEqual(45.175757575757579d, value);
        }

        [Test]
        public void TriggersMastery()
        {
            // Arrange

            // Act
            var value = _spell.TriggersMastery(_gameState, null);

            // Assert
            Assert.AreEqual(true, value);
        }
    }
}
