using NUnit.Framework;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common.Traits;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.Common.Traits
{
    [TestFixture]
    class UltimateFormTests : BaseTest
    {
        private ISpellService _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService gameStateService = new GameStateService();
            _spell = new UltimateForm(gameStateService, new Fleshcraft(gameStateService));
            _gameState = GetGameState();
        }

        [Test]
        public void GetAverageRawHealing()
        {
            // Arrange
            
            // Act
            var value = _spell.GetAverageRawHealing(_gameState, null);

            // Assert
            Assert.AreEqual(5154.6144000000004d, value);
        }

        [Test]
        public void GetMaximumHealTargets()
        {
            // Arrange

            // Act
            var value = _spell.GetMaximumHealTargets(_gameState, null);

            // Assert
            Assert.AreEqual(1, value);
        }

        [Test]
        public void GetMinimumHealTargets()
        {
            // Arrange

            // Act
            var value = _spell.GetMinimumHealTargets(_gameState, null);

            // Assert
            Assert.AreEqual(1, value);
        }

        [Test]
        public void GetActualCastsPerMinute()
        {
            // Arrange

            // Act
            var value = _spell.GetActualCastsPerMinute(_gameState, null);

            // Assert
            Assert.AreEqual(0.64030530858324164d, value);
        }
    }
}
