using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling;
using Salvation.Core.Modelling.Common.Items;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.Common.Items
{
    [TestFixture]
    class OverflowingAnimaCageTests : BaseTest
    {
        private SpellService _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService gameStateService = new GameStateService();

            _spell = new OverflowingAnimaCage(gameStateService);
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
            var spellData = gameStateService.GetSpellData(_gameState, Spell.OverflowingAnimaCage);
            var buffSpellData = gameStateService.GetSpellData(_gameState, Spell.OverflowingAnimaCageBuff);
            // 155 is scale budget for ilvl 226 (testing)
            spellData.Overrides.Add(Core.Constants.Override.ItemLevel, 226);
            buffSpellData.GetEffect(845125).ScaleValues.Add(226, 203.03347229957581);
            gameStateService.OverrideSpellData(_gameState, buffSpellData);
            gameStateService.OverridePlaystyle(_gameState, new Core.Profile.Model.PlaystyleEntry("OverflowingAnimaCageCountAllyBuffs", 1));
            gameStateService.OverridePlaystyle(_gameState, new Core.Profile.Model.PlaystyleEntry("OverflowingAnimaCageAverageNumberAllies", 5));

            // Act
            var value = _spell.GetAverageCriticalStrike(_gameState, spellData);

            // Assert
            Assert.AreEqual(124.00500627986312d, value);
        }

        [Test]
        public void GetDuration()
        {
            // Arrange

            // Act
            var value = _spell.GetDuration(_gameState, null);

            // Assert
            Assert.AreEqual(15.0d, value);
        }

        [Test]
        public void GetUptime()
        {
            // Arrange

            // Act
            var value = _spell.GetUptime(_gameState, null);

            // Assert
            Assert.AreEqual(0.12400503778337531d, value);
        }

        [Test]
        public void GetActualCastsPerMinute()
        {
            // Arrange

            // Act
            var value = _spell.GetActualCastsPerMinute(_gameState, null);

            // Assert
            Assert.AreEqual(0.49602015113350123d, value);
        }

        [Test]
        public void GetMaximumCastsPerMinute()
        {
            // Arrange

            // Act
            var value = _spell.GetMaximumCastsPerMinute(_gameState, null);

            // Assert
            Assert.AreEqual(0.55113350125944582d, value);
        }
    }
}
