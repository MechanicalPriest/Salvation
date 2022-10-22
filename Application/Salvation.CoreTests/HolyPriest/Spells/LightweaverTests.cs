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
    public class LightweaverTests : BaseTest
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
            var spellService = new Heal(_gameStateService, null, null);

            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("LightweaverAverageBuffedCasts", 0.9));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.Lightweaver, 0);
            var resultDefault = spellService.GetAverageRawHealing(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.Lightweaver, 1);
            var resultRank1 = spellService.GetAverageRawHealing(_gameState, null);

            // Assert
            Assert.AreEqual(10745.970166028488d, resultDefault);
            Assert.AreEqual(12196.676138442333d, resultRank1);
        }

        [Test]
        public void GetLightweaverHealingModifier_Calculates_Ranks()
        {
            // Arrange
            var spellService = new Heal(_gameStateService, null, null);

            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("LightweaverAverageBuffedCasts", 0.9));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.Lightweaver, 0);
            var resultDefault = spellService.GetLightweaverHealingModifier(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.Lightweaver, 1);
            var resultRank1 = spellService.GetLightweaverHealingModifier(_gameState);

            // Assert
            Assert.AreEqual(1.0d, resultDefault);
            Assert.AreEqual(1.135d, resultRank1);
        }

        [Test]
        public void GetHastedCastTime_Calculates_Ranks()
        {
            // Arrange
            var spellService = new Heal(_gameStateService, null, null);

            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("LightweaverAverageBuffedCasts", 0.9));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.Lightweaver, 0);
            var resultDefault = spellService.GetHastedCastTime(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.Lightweaver, 1);
            var resultRank1 = spellService.GetHastedCastTime(_gameState, null);

            // Assert
            Assert.AreEqual(2.4387444769610376d, resultDefault);
            Assert.AreEqual(1.7802834681815574d, resultRank1);
        }

        [Test]
        public void GetLightweaverCastTimeReduction_Calculates_Ranks()
        {
            // Arrange
            var spellService = new Heal(_gameStateService, null, null);

            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("LightweaverAverageBuffedCasts", 0.9));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.Lightweaver, 0);
            var resultDefault = spellService.GetLightweaverCastTimeReduction(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.Lightweaver, 1);
            var resultRank1 = spellService.GetLightweaverCastTimeReduction(_gameState);

            // Assert
            Assert.AreEqual(1.0d, resultDefault);
            Assert.AreEqual(0.72999999999999998d, resultRank1);
        }

        [Test]
        public void GetEverlastingLightMultiplier_THrows_No_ResonantWordsPercentageBuffsUsed()
        {
            // Arrange
            var spellService = new Heal(_gameStateService, null, null);

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
