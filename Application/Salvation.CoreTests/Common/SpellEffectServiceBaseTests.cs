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
        public void Int_Returns_Zero()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellService = new SpellEffectService(gameStateService);

            // Act
            var result = spellService.GetAverageIntellect(_gameState, null);

            // Assert
            Assert.AreEqual(0, result);
        }
    }
}
