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
    class DarkmoonDeckReposeTests : BaseTest
    {
        private SpellService _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService gameStateService = new GameStateService();

            _spell = new DarkmoonDeckRepose(gameStateService);
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
            var spellData = gameStateService.GetSpellData(_gameState, Spell.DarkmoonDeckRepose);
            var spellDataLow = gameStateService.GetSpellData(_gameState, Spell.DarkmoonDeckReposeAce);
            var spellDataHigh = gameStateService.GetSpellData(_gameState, Spell.DarkmoonDeckReposeEight);
            // 39 is scale budget for ilvl 200 healing effect (testing)
            spellData.Overrides.Add(Core.Constants.Override.ItemLevel, 200);
            spellDataLow.GetEffect(792442).ScaleValues.Add(200, 39);
            spellDataHigh.GetEffect(792449).ScaleValues.Add(200, 39);
            gameStateService.OverrideSpellData(_gameState, spellDataLow);
            gameStateService.OverrideSpellData(_gameState, spellDataHigh);

            // Act
            var value = _spell.GetAverageRawHealing(_gameState, spellData);

            // Assert
            Assert.AreEqual(16353.643593993384d, value);
        }

        [Test]
        public void GetAverageHealing_Includes_Overheal()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService(); 
            var spellData = gameStateService.GetSpellData(_gameState, Spell.DarkmoonDeckRepose);
            var spellDataLow = gameStateService.GetSpellData(_gameState, Spell.DarkmoonDeckReposeAce);
            var spellDataHigh = gameStateService.GetSpellData(_gameState, Spell.DarkmoonDeckReposeEight);
            // 39 is scale budget for ilvl 200 healing effect (testing)
            spellData.Overrides.Add(Core.Constants.Override.ItemLevel, 200);
            spellDataLow.GetEffect(792442).ScaleValues.Add(200, 39); 
            spellDataHigh.GetEffect(792449).ScaleValues.Add(200, 39); 
            gameStateService.OverrideSpellData(_gameState, spellDataLow);
            gameStateService.OverrideSpellData(_gameState, spellDataHigh);

            // Act
            var value = _spell.GetAverageHealing(_gameState, spellData);

            // Assert
            Assert.AreEqual(13900.597054894375d, value);
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
            Assert.AreEqual(0.69513014273719553d, value);
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
