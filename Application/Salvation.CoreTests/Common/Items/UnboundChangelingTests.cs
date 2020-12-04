using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common.Items;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.Common.Items
{
    [TestFixture]
    class UnboundChangelingTests : BaseTest
    {
        private UnboundChangeling _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService gameStateService = new GameStateService();

            _spell = new UnboundChangeling(gameStateService);
            _gameState = GetGameState();
        }

        [Test]
        public void GetAverageHaste_Throws_Without_Overrides()
        {
            // Arrange

            // Act
            var methodCall = new TestDelegate(
                () => _spell.GetAverageHaste(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void GetAverageHaste_Adds_Average_Haste()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellData = gameStateService.GetSpellData(_gameState, Spell.UnboundChangeling);
            var hasteBuffSpellData = gameStateService.GetSpellData(_gameState, Spell.UnboundChangelingBuff);
            // 155 is scale budget for ilvl 226 (testing)
            spellData.Overrides.Add(Core.Constants.Override.ItemLevel, 226);
            hasteBuffSpellData.ScaleValues.Add(226, 155);
            gameStateService.OverrideSpellData(_gameState, hasteBuffSpellData);

            // Act
            var value = _spell.GetAverageHaste(_gameState, spellData);

            // Assert
            Assert.AreEqual(99.87051502471364d, value);
        }

        [Test]
        public void GetDuration()
        {
            // Arrange

            // Act
            var value = _spell.GetDuration(_gameState, null);

            // Assert
            Assert.AreEqual(12, value);
        }

        [Test]
        public void GetUptime()
        {
            // Arrange

            // Act
            var value = _spell.GetUptime(_gameState, null);

            // Assert
            Assert.AreEqual(0.29287541062965877d, value);
        }
    }
}
