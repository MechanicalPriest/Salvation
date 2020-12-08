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
    class VialOfSpectralEssenceTests : BaseTest
    {
        private SpellService _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService gameStateService = new GameStateService();

            _spell = new VialOfSpectralEssence(gameStateService);
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
            var spellData = gameStateService.GetSpellData(_gameState, Spell.VialOfSpectralEssence);
            // 58 is scale budget for ilvl 226 healing effect (testing)
            spellData.Overrides.Add(Core.Constants.Override.ItemLevel, 226);
            spellData.ScaleValues.Add(226, 58);
            gameStateService.OverrideSpellData(_gameState, spellData);

            // Act
            var value = _spell.GetAverageRawHealing(_gameState, spellData);

            // Assert
            Assert.AreEqual(12588.77003715958d, value);
        }

        [Test]
        public void GetAverageHealing_Includes_Overheal()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var spellData = gameStateService.GetSpellData(_gameState, Spell.VialOfSpectralEssence);
            // 58 is scale budget for ilvl 226 healing effect (testing)
            spellData.Overrides.Add(Core.Constants.Override.ItemLevel, 226);
            spellData.ScaleValues.Add(226, 58);
            gameStateService.OverrideSpellData(_gameState, spellData);

            // Act
            var value = _spell.GetAverageHealing(_gameState, spellData);

            // Assert
            Assert.AreEqual(11959.331535301601d, value);
        }

        [Test]
        public void GetMaximumCastsPerMinute()
        {
            // Arrange

            // Act
            var value = _spell.GetMaximumCastsPerMinute(_gameState, null);

            // Assert
            Assert.AreEqual(0.81780016792611243d, value);
        }

        [Test]
        public void GetActualCastsPerMinute()
        {
            // Arrange

            // Act
            var value = _spell.GetActualCastsPerMinute(_gameState, null);

            // Assert
            Assert.AreEqual(0.7769101595298068d, value);
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
