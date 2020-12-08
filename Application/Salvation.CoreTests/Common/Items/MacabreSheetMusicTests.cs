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
    class MacabreSheetMusicTests : BaseTest
    {
        private SpellService _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService gameStateService = new GameStateService();

            _spell = new MacabreSheetMusic(gameStateService);
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
            var spellData = gameStateService.GetSpellData(_gameState, Spell.MacabreSheetMusic);
            var buffSpellData = gameStateService.GetSpellData(_gameState, Spell.MacabreSheetMusicTrigger);
            // 155 is scale budget for ilvl 226 (testing)
            spellData.Overrides.Add(Core.Constants.Override.ItemLevel, 226);
            buffSpellData.ScaleValues.Add(226, 203.03347229957581);
            gameStateService.OverrideSpellData(_gameState, buffSpellData);

            // Act
            var value = _spell.GetAverageHaste(_gameState, spellData);

            // Assert
            Assert.AreEqual(96.17329324766618d, value);
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
        public void GetDuration()
        {
            // Arrange

            // Act
            var value = _spell.GetDuration(_gameState, null);

            // Assert
            Assert.AreEqual(20.0D, value);
        }

        [Test]
        public void GetUptime()
        {
            // Arrange

            // Act
            var value = _spell.GetUptime(_gameState, null);

            // Assert
            Assert.AreEqual(0.24534005037783371d, value);
        }
    }
}
