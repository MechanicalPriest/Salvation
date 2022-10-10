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
    public class CrisisManagementTests : BaseTest
    {
        private GameState _gameState;
        [OneTimeSetUp]
        public void InitOnce()
        {
            _gameState = GetGameState();
        }

        [Test]
        public void CM_GetAverageRawHealing_Calculates_Ranks_FH()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new FlashHeal(gameStateService);

            // Act
            gameStateService.SetTalentRank(_gameState, Spell.CrisisManagement, 0);
            var resultDefault = spellService.GetAverageRawHealing(_gameState, null);

            gameStateService.SetTalentRank(_gameState, Spell.CrisisManagement, 1);
            var resultRank1 = spellService.GetAverageRawHealing(_gameState, null);

            gameStateService.SetTalentRank(_gameState, Spell.CrisisManagement, 2);
            var resultRank2 = spellService.GetAverageRawHealing(_gameState, null);

            // Assert
            Assert.AreEqual(7394.684554928077d, resultDefault);
            Assert.AreEqual(7951.0825985635893d, resultRank1);
            Assert.AreEqual(8507.4806421990997d, resultRank2);
        }

        [Test]
        public void CM_GetAverageRawHealing_Calculates_Ranks_Heal()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new Heal(gameStateService);

            // Act
            gameStateService.SetTalentRank(_gameState, Spell.CrisisManagement, 0);
            var resultDefault = spellService.GetAverageRawHealing(_gameState, null);

            gameStateService.SetTalentRank(_gameState, Spell.CrisisManagement, 1);
            var resultRank1 = spellService.GetAverageRawHealing(_gameState, null);

            gameStateService.SetTalentRank(_gameState, Spell.CrisisManagement, 2);
            var resultRank2 = spellService.GetAverageRawHealing(_gameState, null);

            // Assert
            Assert.AreEqual(10745.970166028488d, resultDefault);
            Assert.AreEqual(11554.52889939044d, resultRank1);
            Assert.AreEqual(12363.087632752389d, resultRank2);
        }
    }
}
