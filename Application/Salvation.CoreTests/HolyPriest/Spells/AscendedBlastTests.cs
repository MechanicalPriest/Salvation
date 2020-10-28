using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.HolyPriest.Spells
{
    [TestFixture]
    public class AscendedBlastTests : BaseTest
    {
        private GameState _gameState;

        [OneTimeSetUp]
        public void InitOnce()
        {
            _gameState = GetGameState();
        }

        [Test]
        public void AB_GetMaximumCastsPerMinute_Throws_No_Overrides()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new AscendedBlast(gameStateService);

            // Act
            var methodCall = new TestDelegate(
                () => spellService.GetMaximumCastsPerMinute(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void AB_GetMaximumCastsPerMinute_Throws_No_CPM_Override()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new AscendedBlast(gameStateService);

            // Act
            var spellData = gameStateService.GetSpellData(_gameState, Spell.AscendedBlast);
            spellData.Overrides.Add(Override.AllowedDuration, 1);
            var methodCall = new TestDelegate(
                () => spellService.GetMaximumCastsPerMinute(_gameState, spellData));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void AB_GetMaximumCastsPerMinute_Throws_No_AD_Override()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new AscendedBlast(gameStateService);

            // Act
            var spellData = gameStateService.GetSpellData(_gameState, Spell.AscendedBlast);
            spellData.Overrides.Add(Override.CastsPerMinute, 1);
            var methodCall = new TestDelegate(
                () => spellService.GetMaximumCastsPerMinute(_gameState, spellData));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void AB_GetMaximumCastsPerMinute_Calculates()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new AscendedBlast(gameStateService);

            // Act
            var spellData = gameStateService.GetSpellData(_gameState, Spell.AscendedBlast);
            spellData.Overrides.Add(Override.CastsPerMinute, 0.4819088140d);
            spellData.Overrides.Add(Override.AllowedDuration, 10);

            var result = spellService.GetMaximumCastsPerMinute(_gameState, spellData);

            // Assert
            Assert.AreEqual(1.9460392505960107d, result);
        }
    }
}
