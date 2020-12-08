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
    class LeadByExampleTests : BaseTest
    {
        private ISpellService _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService gameStateService = new GameStateService();
            _spell = new LeadByExample(gameStateService, new UnholyNova(gameStateService, new UnholyTransfusion(gameStateService)));
            _gameState = GetGameState();
        }

        [Test]
        public void GetAverageIntellectBonus_Throws_Without_PlaystyleEntries()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();

            // Act
            var methodCall = new TestDelegate(
                () => _spell.GetAverageIntellectBonus(_gameState, null));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(methodCall);
        }

        [Test]
        public void LeadByExampleIncludeAllyBuffs_Increases_GetAverageIntellectBonus()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();
            var gamestateWithout = gameStateService.CloneGameState(_gameState);
            gameStateService.OverridePlaystyle(gamestateWithout, new Core.Profile.Model.PlaystyleEntry("LeadByExampleIncludeAllyBuffs", 0));
            gameStateService.OverridePlaystyle(gamestateWithout, new Core.Profile.Model.PlaystyleEntry("LeadByExampleNearbyAllies", 4));

            var gamestateWith = gameStateService.CloneGameState(gamestateWithout); 
            gameStateService.OverridePlaystyle(gamestateWith, new Core.Profile.Model.PlaystyleEntry("LeadByExampleIncludeAllyBuffs", 1));

            // Act
            var valueWithOut = _spell.GetAverageIntellectBonus(gamestateWithout, null);
            var valueWith = _spell.GetAverageIntellectBonus(gamestateWith, null);

            // Assert
            Assert.AreEqual(0.024941225860621332d, valueWithOut);
            Assert.AreEqual(0.040289672544080612d, valueWith);
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
            Assert.AreEqual(0.191855583543241d, value);
        }
    }
}
