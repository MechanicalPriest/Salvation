using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.HolyPriest.Spells
{
    [TestFixture]
    public class FleshcraftTests : BaseTest
    {
        private GameState _gameState;
        private ISpellService _spell;

        [OneTimeSetUp]
        public void InitOnce()
        {
            IGameStateService gameStateService = new GameStateService();
            _spell = new Fleshcraft(gameStateService);

            _gameState = GetGameState();
        }

        [Test]
        public void GetAverageRawHealing()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var gamestate = gameStateService.CloneGameState(_gameState);
            gameStateService.OverridePlaystyle(gamestate, new Core.Profile.Model.PlaystyleEntry("FleshCraftShieldMultiplier", 1.5));

            // Act
            var result = _spell.GetAverageRawHealing(gamestate, null);

            // Assert
            Assert.AreEqual(8034.0d, result);
        }

        [Test]
        public void GetAverageRawHealing_Throws_No_Override()
        {
            // Arrange

            // Act
            var methodCall = new TestDelegate(
                () => _spell.GetAverageRawHealing(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void GetMaximumCastsPerMinute()
        {
            // Arrange

            // Act
            var result = _spell.GetMaximumCastsPerMinute(_gameState, null);

            // Assert
            Assert.AreEqual(0.63679938606528375d, result);
        }

        [Test]
        public void GetMinimumHealTargets()
        {
            // Arrange

            // Act
            var result = _spell.GetMinimumHealTargets(_gameState, null);

            // Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void GetMaximumHealTargets()
        {
            // Arrange

            // Act
            var result = _spell.GetMaximumHealTargets(_gameState, null);

            // Assert
            Assert.AreEqual(1, result);
        }
    }
}
