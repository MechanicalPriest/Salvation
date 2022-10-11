using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile.Model;
using Salvation.Core.State;
using System;
using System.Collections.Generic;

namespace Salvation.CoreTests.HolyPriest.Spells
{
    [TestFixture]
    public class EverlastingLightTests : BaseTest
    {
        private GameState _gameState;
        private GameStateService _gameStateService;

        [SetUp]
        public void Init()
        {
            _gameState = GetGameState();
            _gameStateService = new GameStateService();
        }

        [Test]
        public void GetAverageRawHealing_Calculates_Ranks()
        {
            // Arrange
            var spellService = new Heal(_gameStateService);

            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("EverlastingLightAverageMana", 0.45));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.EverlastingLight, 0);
            var resultDefault = spellService.GetAverageRawHealing(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.EverlastingLight, 1);
            var resultRank1 = spellService.GetAverageRawHealing(_gameState, null);

            // Assert
            Assert.AreEqual(10745.970166028488d, resultDefault);
            Assert.AreEqual(11471.323152235411d, resultRank1);
        }

        [Test]
        public void GetEverlastingLightMultiplier_Calculates_Ranks()
        {
            // Arrange
            var spellService = new Heal(_gameStateService);

            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("EverlastingLightAverageMana", 0.45));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.EverlastingLight, 0);
            var resultDefault = spellService.GetEverlastingLightMultiplier(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.EverlastingLight, 1);
            var resultRank1 = spellService.GetEverlastingLightMultiplier(_gameState);

            // Assert
            Assert.AreEqual(1.0d, resultDefault);
            Assert.AreEqual(1.0674999999999999d, resultRank1);
        }

        [Test]
        public void GetEverlastingLightMultiplier_THrows_No_ResonantWordsPercentageBuffsUsed()
        {
            // Arrange
            var spellService = new Heal(_gameStateService);

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.EverlastingLight, 1);
            var methodCall = new TestDelegate(
                () => spellService.GetEverlastingLightMultiplier(_gameState));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("EverlastingLightAverageMana needs to be set. (Parameter 'EverlastingLightAverageMana')"));
            Assert.That(ex.ParamName, Is.EqualTo("EverlastingLightAverageMana"));
        }
    }
}
