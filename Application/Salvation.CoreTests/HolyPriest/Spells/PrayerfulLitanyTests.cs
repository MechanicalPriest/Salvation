using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.State;

namespace Salvation.CoreTests.HolyPriest.Spells
{
    [TestFixture]
    public class PrayerfulLitanyTests : BaseTest
    {
        private GameState _gameState;
        [OneTimeSetUp]
        public void InitOnce()
        {
            _gameState = GetGameState();
        }

        [Test]
        public void PL_GetAverageRawHealing_Calculates_Ranks()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new PrayerOfHealing(gameStateService, new Renew(gameStateService, null));

            // Act
            gameStateService.SetTalentRank(_gameState, Spell.PrayerfulLitany, 0);
            var resultDefault = spellService.GetAverageRawHealing(_gameState, null);

            gameStateService.SetTalentRank(_gameState, Spell.PrayerfulLitany, 1);
            var resultRank1 = spellService.GetAverageRawHealing(_gameState, null);

            // Assert
            Assert.AreEqual(15936.820161482929d, resultDefault);
            Assert.AreEqual(16893.029371171906d, resultRank1);
        }
    }
}
