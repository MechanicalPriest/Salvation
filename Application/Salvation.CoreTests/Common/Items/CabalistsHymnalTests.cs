using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common.Items;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.Common.Items
{
    [TestFixture]
    class CabalistsHymnalTests : BaseTest
    {
        private CabalistsHymnal _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService gameStateService = new GameStateService();

            _spell = new CabalistsHymnal(gameStateService);
            _gameState = GetGameState();
        }

        [Test]
        public void GetAverageCriticalStrike_Throws_Without_Overrides()
        {
            // Arrange

            // Act
            var methodCall = new TestDelegate(
                () => _spell.GetAverageCriticalStrike(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void GetAverageCriticalStrike_Adds_Average_Crit()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellData = gameStateService.GetSpellData(_gameState, Spell.CabalistsHymnal);
            var buffSpellData = gameStateService.GetSpellData(_gameState, Spell.CabalistsHymnalBuff);
            // 155 is scale budget for ilvl 226 (testing)
            spellData.Overrides.Add(Core.Constants.Override.ItemLevel, 226);
            buffSpellData.ScaleValues.Add(226, 203.03347229957581);
            gameStateService.OverrideSpellData(_gameState, buffSpellData);

            // Act
            var value = _spell.GetAverageCriticalStrike(_gameState, spellData);

            // Assert
            Assert.AreEqual(94.999970789388428d, value);
        }

        [Test]
        public void GetDuration()
        {
            // Arrange

            // Act
            var value = _spell.GetDuration(_gameState, null);

            // Assert
            Assert.AreEqual(30.0d, value);
        }

        [Test]
        public void GetUptime()
        {
            // Arrange

            // Act
            var value = _spell.GetUptime(_gameState, null);

            // Assert
            Assert.AreEqual(0.5d, value);
        }
    }
}
