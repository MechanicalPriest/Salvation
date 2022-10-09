using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.State;

namespace Salvation.CoreTests.HolyPriest.Spells
{
    [TestFixture]
    public class GalesOfSongTests : BaseTest
    {
        private GameState _gameState;
        [SetUp]
        public void InitOnce()
        {
            _gameState = GetGameState();
        }

        [Test]
        public void GalesOfSong_GetAverageRawHealing_Calculates_Ranks()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new DivineHymn(gameStateService);

            // Act
            gameStateService.SetTalentRank(_gameState, Spell.GalesOfSong, 0);
            var resultDefault = spellService.GetAverageRawHealing(_gameState, null);

            gameStateService.SetTalentRank(_gameState, Spell.GalesOfSong, 1);
            var resultRank1 = spellService.GetAverageRawHealing(_gameState, null);

            gameStateService.SetTalentRank(_gameState, Spell.GalesOfSong, 2);
            var resultRank2 = spellService.GetAverageRawHealing(_gameState);

            // Assert
            Assert.AreEqual(236047.07347750716d, resultDefault);
            Assert.AreEqual(259651.78082525788d, resultRank1);
            Assert.AreEqual(283955.88690923818d, resultRank2);
        }

        [Test]
        public void GalesOfSong_GetGalesOfSongStackModifier()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new DivineHymn(gameStateService);

            // Act
            gameStateService.SetTalentRank(_gameState, Spell.GalesOfSong, 0);
            var resultDefault = spellService.GetGalesOfSongStackModifier(_gameState);

            gameStateService.SetTalentRank(_gameState, Spell.GalesOfSong, 1);
            var resultRank1 = spellService.GetGalesOfSongStackModifier(_gameState);

            gameStateService.SetTalentRank(_gameState, Spell.GalesOfSong, 2);
            var resultRank2 = spellService.GetGalesOfSongStackModifier(_gameState);

            // Assert
            Assert.AreEqual(0.0d, resultDefault);
            Assert.AreEqual(0.01d, resultRank1);
            Assert.AreEqual(0.02d, resultRank2);
        }

        [Test]
        public void GalesOfSong_GetGalesOfSongHealMultiplier()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new DivineHymn(gameStateService);

            // Act
            gameStateService.SetTalentRank(_gameState, Spell.GalesOfSong, 0);
            var resultDefault = spellService.GetGalesOfSongHealMultiplier(_gameState);

            gameStateService.SetTalentRank(_gameState, Spell.GalesOfSong, 1);
            var resultRank1 = spellService.GetGalesOfSongHealMultiplier(_gameState);

            gameStateService.SetTalentRank(_gameState, Spell.GalesOfSong, 2);
            var resultRank2 = spellService.GetGalesOfSongHealMultiplier(_gameState);

            // Assert
            Assert.AreEqual(1.0d, resultDefault);
            Assert.AreEqual(1.0800000000000001d, resultRank1);
            Assert.AreEqual(1.1599999999999999d, resultRank2);
        }
    }
}
