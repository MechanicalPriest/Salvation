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
            var spellService = new FlashHeal(gameStateService, null, null);

            // Act
            gameStateService.SetTalentRank(_gameState, Spell.CrisisManagement, 0);
            var resultDefault = spellService.GetAverageRawHealing(_gameState, null);

            gameStateService.SetTalentRank(_gameState, Spell.CrisisManagement, 1);
            var resultRank1 = spellService.GetAverageRawHealing(_gameState, null);

            gameStateService.SetTalentRank(_gameState, Spell.CrisisManagement, 2);
            var resultRank2 = spellService.GetAverageRawHealing(_gameState, null);

            // Assert
            Assert.AreEqual(7376.1978435407573d, resultDefault);
            Assert.AreEqual(7931.2048920671805d, resultRank1);
            Assert.AreEqual(8486.2119405936028d, resultRank2);
        }

        [Test]
        public void CM_GetAverageRawHealing_Calculates_Ranks_Heal()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new Heal(gameStateService, null, null);

            // Act
            gameStateService.SetTalentRank(_gameState, Spell.CrisisManagement, 0);
            var resultDefault = spellService.GetAverageRawHealing(_gameState, null);

            gameStateService.SetTalentRank(_gameState, Spell.CrisisManagement, 1);
            var resultRank1 = spellService.GetAverageRawHealing(_gameState, null);

            gameStateService.SetTalentRank(_gameState, Spell.CrisisManagement, 2);
            var resultRank2 = spellService.GetAverageRawHealing(_gameState, null);

            // Assert
            Assert.AreEqual(10719.105240613419d, resultDefault);
            Assert.AreEqual(11525.642577141965d, resultRank1);
            Assert.AreEqual(12332.17991367051d, resultRank2);
        }
    }
}
