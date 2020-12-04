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
    class ManaboundMirrorTests : BaseTest
    {
        private SpellService _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService gameStateService = new GameStateService();

            _spell = new ManaboundMirror(gameStateService);
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
        public void GetAverageRawHealing()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellData = gameStateService.GetSpellData(_gameState, Spell.ManaboundMirror);
            var buffSpellData = gameStateService.GetSpellData(_gameState, Spell.ManaboundMirrorHeal);
            // 58 is scale budget for ilvl 226 healing effect (testing)
            spellData.Overrides.Add(Core.Constants.Override.ItemLevel, 226);
            buffSpellData.ScaleValues.Add(226, 58);
            gameStateService.OverrideSpellData(_gameState, buffSpellData);
            gameStateService.OverridePlaystyle(_gameState, new Core.Profile.Model.PlaystyleEntry("ManaboundMirrorPercentMirrorFilled", 1.0));

            // Act
            var value = _spell.GetAverageRawHealing(_gameState, spellData);

            // Assert
            Assert.AreEqual(16260.493636737641d, value);
        }

        [Test]
        public void GetAverageHealing_Includes_Overheal()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellData = gameStateService.GetSpellData(_gameState, Spell.ManaboundMirror);
            var buffSpellData = gameStateService.GetSpellData(_gameState, Spell.ManaboundMirrorHeal);
            // 58 is scale budget for ilvl 226 healing effect (testing)
            spellData.Overrides.Add(Core.Constants.Override.ItemLevel, 226);
            buffSpellData.ScaleValues.Add(226, 58);
            gameStateService.OverrideSpellData(_gameState, buffSpellData);
            gameStateService.OverridePlaystyle(_gameState, new Core.Profile.Model.PlaystyleEntry("ManaboundMirrorPercentMirrorFilled", 1.0));

            // Act
            var value = _spell.GetAverageHealing(_gameState, spellData);

            // Assert
            Assert.AreEqual(13821.419591226995d, value);
        }

        [Test]
        public void GetMaximumCastsPerMinute()
        {
            // Arrange

            // Act
            var value = _spell.GetMaximumCastsPerMinute(_gameState, null);

            // Assert
            Assert.AreEqual(1.0d, value);
        }

        [Test]
        public void GetActualCastsPerMinute()
        {
            // Arrange

            // Act
            var value = _spell.GetActualCastsPerMinute(_gameState, null);

            // Assert
            Assert.AreEqual(0.90000000000000002d, value);
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
