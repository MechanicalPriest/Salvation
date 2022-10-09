using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.State;

namespace Salvation.CoreTests.HolyPriest.Spells
{
    [TestFixture]
    public class OrisonTests : BaseTest
    {
        private GameState _gameState;
        [OneTimeSetUp]
        public void InitOnce()
        {
            _gameState = GetGameState();
        }

        [Test]
        public void Orison_GetAverageRawHealing_Calculates_Ranks()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new CircleOfHealing(gameStateService);

            // Act
            gameStateService.SetTalentRank(_gameState, Spell.Orison, 0);
            var resultDefaultHeal = spellService.GetAverageRawHealing(_gameState, null);
            var resultHastedCD = spellService.GetHastedCooldown(_gameState, null);

            gameStateService.SetTalentRank(_gameState, Spell.Orison, 1);
            var resultRank1Heal = spellService.GetAverageRawHealing(_gameState, null);
            var resultRank1HastedCD = spellService.GetHastedCooldown(_gameState, null);

            // Assert
            Assert.AreEqual(19124.184193779514d, resultDefaultHeal);
            Assert.AreEqual(14.632466861766225d, resultHastedCD);

            Assert.AreEqual(22949.021032535416d, resultRank1Heal);
            Assert.AreEqual(11.705973489412981d, resultRank1HastedCD);
        }
    }
}
