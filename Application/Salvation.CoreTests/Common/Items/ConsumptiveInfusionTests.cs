using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common.Items;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.Common.Items
{
    [TestFixture]
    class ConsumptiveInfusionTests : BaseTest
    {
        private ConsumptiveInfusion _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService gameStateService = new GameStateService();

            _spell = new ConsumptiveInfusion(gameStateService);
            _gameState = GetGameState();
        }

        [Test]
        public void GetAverageLeech_Throws_Without_Overrides()
        {
            // Arrange

            // Act
            var methodCall = new TestDelegate(
                () => _spell.GetAverageLeech(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void GetAverageLeech_Adds()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellData = gameStateService.GetSpellData(_gameState, Spell.ConsumptiveInfusion);
            var buffSpellData = gameStateService.GetSpellData(_gameState, Spell.ConsumptiveInfusionBuff);
            // 155 is scale budget for ilvl 226 (testing)
            spellData.Overrides.Add(Core.Constants.Override.ItemLevel, 226);
            buffSpellData.ScaleValues.Add(226, 203.03347229957581);
            gameStateService.OverrideSpellData(_gameState, buffSpellData);

            // Act
            var value = _spell.GetAverageLeech(_gameState, spellData);

            // Assert
            Assert.AreEqual(120.10682863867854d, value);
        }

        [Test]
        public void GetDuration()
        {
            // Arrange

            // Act
            var value = _spell.GetDuration(_gameState, null);

            // Assert
            Assert.AreEqual(10.0d, value);
        }

        [Test]
        public void GetUptime()
        {
            // Arrange

            // Act
            var value = _spell.GetUptime(_gameState, null);

            // Assert
            Assert.AreEqual(0.35852225020990763d, value);
        }

        [Test]
        public void GetMaximumCastsPerMinute()
        {
            // Arrange

            // Act
            var value = _spell.GetMaximumCastsPerMinute(_gameState, null);

            // Assert
            Assert.AreEqual(2.1511335012594457d, value);
        }

        [Test]
        public void GetHastedCooldown()
        {
            // Arrange

            // Act
            var value = _spell.GetHastedCooldown(_gameState, null);

            // Assert
            Assert.AreEqual(30.0d, value);
        }
    }
}
