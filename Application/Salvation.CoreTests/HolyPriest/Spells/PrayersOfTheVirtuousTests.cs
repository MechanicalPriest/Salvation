using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.State;

namespace Salvation.CoreTests.HolyPriest.Spells
{
    [TestFixture]
    public class PrayersOfTheVirtuousTests : BaseTest
    {
        private GameState _gameState;
        [SetUp]
        public void InitOnce()
        {
            _gameState = GetGameState();
        }

        [Test]
        public void PotV_GetAverageRawHealing_Calculates_Ranks()
        {
            // Arrange
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new PrayerOfMending(gameStateService);

            // Act
            gameStateService.SetTalentRank(_gameState, Spell.PrayersOfTheVirtuous, 0);
            var resultDefault = spellService.GetAverageRawHealing(_gameState);

            gameStateService.SetTalentRank(_gameState, Spell.PrayersOfTheVirtuous, 1);
            var resultRank1 = spellService.GetAverageRawHealing(_gameState);

            gameStateService.SetTalentRank(_gameState, Spell.PrayersOfTheVirtuous, 2);
            var resultRank2 = spellService.GetAverageRawHealing(_gameState);

            // Assert
            Assert.AreEqual(10999.137937736617d, resultDefault);
            Assert.AreEqual(13198.965525283937d, resultRank1);
            Assert.AreEqual(15398.793112831259d, resultRank2);
        }

        [Test]
        public void PotV_GetPrayersOfTheVirtuousModifier()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new PrayerOfMending(gameStateService);

            // Act
            gameStateService.SetTalentRank(_gameState, Spell.PrayersOfTheVirtuous, 0);
            var resultDefault = spellService.GetPrayersOfTheVirtuousModifier(_gameState);

            gameStateService.SetTalentRank(_gameState, Spell.PrayersOfTheVirtuous, 1);
            var resultRank1 = spellService.GetPrayersOfTheVirtuousModifier(_gameState);

            gameStateService.SetTalentRank(_gameState, Spell.PrayersOfTheVirtuous, 2);
            var resultRank2 = spellService.GetPrayersOfTheVirtuousModifier(_gameState);

            // Assert
            Assert.AreEqual(0.0d, resultDefault);
            Assert.AreEqual(1.0d, resultRank1);
            Assert.AreEqual(2.0d, resultRank2);
        }
    }
}
