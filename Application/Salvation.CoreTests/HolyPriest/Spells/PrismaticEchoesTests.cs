using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile;
using Salvation.Core.State;

namespace Salvation.CoreTests.HolyPriest.Spells
{
    [TestFixture]
    public class PrismaticEchoesTests : BaseTest
    {
        private GameState _gameState;
        [SetUp]
        public void InitOnce()
        {
            _gameState = GetGameState();
        }

        [Test]
        public void PrismaticEchos_GetAverageMasteryIncreaseMultiplier_Calculates_Ranks()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellService(gameStateService);

            // Act
            gameStateService.SetTalentRank(_gameState, Spell.PrismaticEchoes, 0);
            var resultDefault = spellService.GetPrismaticEchoesMultiplier(_gameState);

            gameStateService.SetTalentRank(_gameState, Spell.PrismaticEchoes, 1);
            var resultRank1 = spellService.GetPrismaticEchoesMultiplier(_gameState);

            gameStateService.SetTalentRank(_gameState, Spell.PrismaticEchoes, 2);
            var resultRank2 = spellService.GetPrismaticEchoesMultiplier(_gameState);

            // Assert
            Assert.AreEqual(1.0d, resultDefault);
            Assert.AreEqual(1.0600000000000001d, resultRank1);
            Assert.AreEqual(1.1200000000000001d, resultRank2);
        }

        [Test]
        public void PrismaticEchos_GetMasteryMultiplier_Calculates_Ranks()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new FlashHeal(gameStateService, null);

            // Act
            gameStateService.SetTalentRank(_gameState, Spell.PrismaticEchoes, 0);
            var resultGamestateDefault = spellService.GetHolyPriestMasteryResult(_gameState, null);

            gameStateService.SetTalentRank(_gameState, Spell.PrismaticEchoes, 1);
            var resultGamestateRank1 = spellService.GetHolyPriestMasteryResult(_gameState, null);

            gameStateService.SetTalentRank(_gameState, Spell.PrismaticEchoes, 2);
            var resultGamestateRank2 = spellService.GetHolyPriestMasteryResult(_gameState, null);

            // Assert
            Assert.AreEqual(790.82043156869759d, resultGamestateDefault.RawHealing);
            Assert.AreEqual(838.26965746281951d, resultGamestateRank1.RawHealing);
            Assert.AreEqual(885.71888335694143d, resultGamestateRank2.RawHealing);
        }
    }
}
