using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.State;

namespace Salvation.CoreTests.HolyPriest.Spells
{
    [TestFixture]
    public class RapidRecoveryTests : BaseTest
    {
        private GameState _gameState;
        [OneTimeSetUp]
        public void InitOnce()
        {
            _gameState = GetGameState();
        }

        [Test]
        public void RapidRecovery_GetAverageRawHealing_Calculates_Ranks()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new Renew(gameStateService, null);

            // Act
            gameStateService.SetTalentRank(_gameState, Spell.RapidRecovery, 0);
            var resultDefault = spellService.GetAverageRawHealing(_gameState, null);

            gameStateService.SetTalentRank(_gameState, Spell.RapidRecovery, 1);
            var resultRank1 = spellService.GetAverageRawHealing(_gameState, null);

            // Assert
            Assert.AreEqual(6410.2771481277778d, resultDefault);
            Assert.AreEqual(7205.6476669305775d, resultRank1);
        }

        [Test]
        public void RapidRecovery_GetRapidRecoveryHealingMultiplier_Calculates_Ranks()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new Renew(gameStateService, null);

            // Act
            gameStateService.SetTalentRank(_gameState, Spell.RapidRecovery, 0);
            var resultDefault = spellService.GetRapidRecoveryHealingMultiplier(_gameState);

            gameStateService.SetTalentRank(_gameState, Spell.RapidRecovery, 1);
            var resultRank1 = spellService.GetRapidRecoveryHealingMultiplier(_gameState);

            // Assert
            Assert.AreEqual(1.0d, resultDefault);
            Assert.AreEqual(1.3500000000000001d, resultRank1);
        }

        [Test]
        public void RapidRecovery_GetRapidRecoveryDurationModifier_Calculates_Ranks()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new Renew(gameStateService, null);

            // Act
            gameStateService.SetTalentRank(_gameState, Spell.RapidRecovery, 0);
            var resultDefault = spellService.GetRapidRecoveryDurationModifier(_gameState);

            gameStateService.SetTalentRank(_gameState, Spell.RapidRecovery, 1);
            var resultRank1 = spellService.GetRapidRecoveryDurationModifier(_gameState);

            // Assert
            Assert.AreEqual(0.0d, resultDefault);
            Assert.AreEqual(-3000.0d, resultRank1);
        }
    }
}
