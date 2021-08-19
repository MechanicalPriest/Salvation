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
    class SoullettingRubyTests : BaseTest
    {
        private SpellService _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService gameStateService = new GameStateService();

            _spell = new SoullettingRuby(gameStateService);
            _gameState = GetGameState();
        }

        [Test]
        public void GetAverageRawHealing_Throws_Without_Overrides()
        {
            // Arrange

            // Act
            var methodCall = new TestDelegate(
                () => _spell.GetAverageRawHealing(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
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
        public void GetAverageRawHealing()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellData = gameStateService.GetSpellData(_gameState, Spell.SoullettingRuby);
            var buffSpellData = gameStateService.GetSpellData(_gameState, Spell.SoullettingRubyHeal);
            // 58 is scale budget for ilvl 226 healing effect (testing)
            spellData.Overrides.Add(Core.Constants.Override.ItemLevel, 226);
            buffSpellData.GetEffect(871957).ScaleValues.Add(226, 58);
            gameStateService.OverrideSpellData(_gameState, buffSpellData);

            // Act
            var value = _spell.GetAverageRawHealing(_gameState, spellData);

            // Assert
            Assert.AreEqual(4028.4062745981601d, value);
        }

        [Test]
        public void GetAverageHealing_Includes_Overheal()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellData = gameStateService.GetSpellData(_gameState, Spell.SoullettingRuby);
            var buffSpellData = gameStateService.GetSpellData(_gameState, Spell.SoullettingRubyHeal);
            // 58 is scale budget for ilvl 226 healing effect (testing)
            spellData.Overrides.Add(Core.Constants.Override.ItemLevel, 226);
            buffSpellData.GetEffect(871957).ScaleValues.Add(226, 58);
            gameStateService.OverrideSpellData(_gameState, buffSpellData);

            // Act
            var value = _spell.GetAverageHealing(_gameState, spellData);

            // Assert
            Assert.AreEqual(2819.884392218712d, value);
        }

        [Test]
        public void GetAverageCriticalStrike()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellData = gameStateService.GetSpellData(_gameState, Spell.SoullettingRuby);
            var buffSpellData = gameStateService.GetSpellData(_gameState, Spell.SoullettingRubyTrigger);
            // 203.03347229957581 is scale budget for ilvl 226 crit effect (testing)
            spellData.Overrides.Add(Core.Constants.Override.ItemLevel, 226);
            buffSpellData.GetEffect(871958).ScaleValues.Add(226, 203.03347229957581);
            buffSpellData.GetEffect(871962).ScaleValues.Add(226, 203.03347229957581);
            gameStateService.OverrideSpellData(_gameState, buffSpellData);
            gameStateService.OverridePlaystyle(_gameState, new Core.Profile.Model.PlaystyleEntry("SoullettingRubyAverageEnemyHP", .5));

            // Act
            var value = _spell.GetAverageCriticalStrike(_gameState, spellData);

            // Assert
            Assert.AreEqual(118.0166534359139d, value);
        }

        [Test]
        public void GetMaximumCastsPerMinute()
        {
            // Arrange

            // Act
            var value = _spell.GetMaximumCastsPerMinute(_gameState, null);

            // Assert
            Assert.AreEqual(0.65113350125944591d, value);
        }

        [Test]
        public void GetActualCastsPerMinute()
        {
            // Arrange

            // Act
            var value = _spell.GetActualCastsPerMinute(_gameState, null);

            // Assert
            Assert.AreEqual(0.58602015113350137d, value);
        }

        [Test]
        public void GetDuration()
        {
            // Arrange

            // Act
            var value = _spell.GetDuration(_gameState, null);

            // Assert
            Assert.AreEqual(16, value);
        }

        [Test]
        public void GetUptime()
        {
            // Arrange

            // Act
            var value = _spell.GetUptime(_gameState, null);

            // Assert
            Assert.AreEqual(0.15627204030226702d, value);
        }

        [Test]
        public void TriggersMastery()
        {
            // Arrange

            // Act
            var value = _spell.TriggersMastery(_gameState, null);

            // Assert
            Assert.IsTrue(value);
        }
    }
}
