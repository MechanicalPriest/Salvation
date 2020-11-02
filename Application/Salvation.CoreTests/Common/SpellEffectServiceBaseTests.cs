using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common;
using Salvation.Core.State;

namespace Salvation.CoreTests.Common
{
    [TestFixture]
    public class SpellEffectServiceBaseTests : BaseTest
    {
        private GameState _gameState;

        [OneTimeSetUp]
        public void InitOnce()
        {
            _gameState = GetGameState();
        }

        [Test]
        public void GetAverageIntellect_Defaults_Zero()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellEffectService(gameStateService);

            // Act
            var result = spellService.GetAverageIntellect(_gameState, null);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void GetAverageCriticalStrike_Defaults_Zero()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellEffectService(gameStateService);

            // Act
            var result = spellService.GetAverageCriticalStrike(_gameState, null);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void GetAverageHaste_Defaults_Zero()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellEffectService(gameStateService);

            // Act
            var result = spellService.GetAverageHaste(_gameState, null);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void GetAverageMastery_Defaults_Zero()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellEffectService(gameStateService);

            // Act
            var result = spellService.GetAverageMastery(_gameState, null);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void GetAverageVersatility_Defaults_Zero()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellEffectService(gameStateService);

            // Act
            var result = spellService.GetAverageVersatility(_gameState, null);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void GetUptime_Defaults_Zero()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellEffectService(gameStateService);

            // Act
            var result = spellService.GetUptime(_gameState, null);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void GetAverageMp5_Defaults_Zero()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellEffectService(gameStateService);

            // Act
            var result = spellService.GetAverageMp5(_gameState, null);

            // Assert
            Assert.AreEqual(0, result);
        }
    }
}
