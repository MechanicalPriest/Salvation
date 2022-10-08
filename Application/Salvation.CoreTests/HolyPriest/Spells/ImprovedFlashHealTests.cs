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
    public class ImprovedFlashHealTests : BaseTest
    {
        private GameState _gameState;
        [OneTimeSetUp]
        public void InitOnce()
        {
            _gameState = GetGameState();
        }

        [Test]
        public void IFH_GetAverageRawHealing_Calculates_Rank1()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new FlashHeal(gameStateService);

            // Act
            //gameStateService.SetTalentRank(_gameState, Spell.ImprovedFlashHeal, 0);
            var resultDefault = spellService.GetAverageRawHealing(_gameState, null);

            gameStateService.SetTalentRank(_gameState, Spell.ImprovedFlashHeal, 1);
            var resultOverride = spellService.GetAverageRawHealing(_gameState, null);

            // Assert
            Assert.AreEqual(7394.684554928077d, resultDefault);
            Assert.AreEqual(8503.8872381672882d, resultOverride);
        }
    }
}
