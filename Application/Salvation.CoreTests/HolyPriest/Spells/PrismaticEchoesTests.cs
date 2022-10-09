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
            var spellService = new PrismaticEchoes(gameStateService);
            _gameState.RegisteredSpells = new System.Collections.Generic.List<Core.Profile.Model.RegisteredSpell>()
            {
                new Core.Profile.Model.RegisteredSpell(Spell.PrismaticEchoes)
                {
                    SpellService = spellService
                }
            };

            // Act
            gameStateService.SetTalentRank(_gameState, Spell.PrismaticEchoes, 0);
            var resultDefault = spellService.GetAverageMasteryIncreaseMultiplier(_gameState, null);

            gameStateService.SetTalentRank(_gameState, Spell.PrismaticEchoes, 1);
            var resultRank1 = spellService.GetAverageMasteryIncreaseMultiplier(_gameState, null);

            gameStateService.SetTalentRank(_gameState, Spell.PrismaticEchoes, 2);
            var resultRank2 = spellService.GetAverageMasteryIncreaseMultiplier(_gameState, null);

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
            var spellService = new PrismaticEchoes(gameStateService);
            _gameState.RegisteredSpells = new System.Collections.Generic.List<Core.Profile.Model.RegisteredSpell>()
            {
                new Core.Profile.Model.RegisteredSpell(Spell.PrismaticEchoes)
                {
                    SpellService = spellService
                }
            };

            // Act
            gameStateService.SetTalentRank(_gameState, Spell.PrismaticEchoes, 0);
            var resultGamestateDefault = gameStateService.GetMasteryMultiplier(_gameState);

            gameStateService.SetTalentRank(_gameState, Spell.PrismaticEchoes, 1);
            var resultGamestateRank1 = gameStateService.GetMasteryMultiplier(_gameState);

            gameStateService.SetTalentRank(_gameState, Spell.PrismaticEchoes, 2);
            var resultGamestateRank2 = gameStateService.GetMasteryMultiplier(_gameState);

            // Assert
            Assert.AreEqual(1.1069444444444445d, resultGamestateDefault);
            Assert.AreEqual(1.1733611111111113d, resultGamestateRank1);
            Assert.AreEqual(1.2397777777777779d, resultGamestateRank2);
        }
    }
}
