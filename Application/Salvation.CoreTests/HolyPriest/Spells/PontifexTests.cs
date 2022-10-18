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
    public class PontifexTests : BaseTest
    {
        private GameState _gameState;
        private GameStateService _gameStateService;
        private HolyWordSerenity _holyWordSerenity;
        private HolyWordSanctify _holyWordSanctify;
        private HolyWordSalvation _holyWordSalvation;

        [SetUp]
        public void Init()
        {
            _gameState = GetGameState();
            _gameStateService = new GameStateService();

            _holyWordSerenity = new HolyWordSerenity(_gameStateService,
                new FlashHeal(_gameStateService, null, null),
                new Heal(_gameStateService, null, null),
                new PrayerOfMending(_gameStateService, null));
            _holyWordSanctify = new HolyWordSanctify(_gameStateService,
                new PrayerOfHealing(_gameStateService, new Renew(_gameStateService)),
                new Renew(_gameStateService),
                new CircleOfHealing(_gameStateService));

            // Add salv so it'll resolve.
            _holyWordSalvation = new HolyWordSalvation(_gameStateService,
                new HolyWordSerenity(_gameStateService,
                    new FlashHeal(_gameStateService, null, null),
                    new Heal(_gameStateService, null, null),
                    new PrayerOfMending(_gameStateService, null)),
                new HolyWordSanctify(_gameStateService,
                    new PrayerOfHealing(_gameStateService, new Renew(_gameStateService)),
                    new Renew(_gameStateService),
                    new CircleOfHealing(_gameStateService)),
                new Renew(_gameStateService),
                new PrayerOfMending(_gameStateService, null));

            _gameState.RegisteredSpells.AddRange(
                new List<RegisteredSpell>()
                {
                    new RegisteredSpell()
                    {
                        Spell = Spell.HolyWordSalvation,
                        SpellService = _holyWordSalvation,
                        SpellData = _gameStateService.GetSpellData(_gameState, Spell.HolyWordSalvation)
                    },
                    new RegisteredSpell()
                    {
                        Spell = Spell.FlashHeal,
                        SpellService = new FlashHeal(_gameStateService, null, null),
                        SpellData = _gameStateService.GetSpellData(_gameState, Spell.HolyWordSalvation)
                    },
                    new RegisteredSpell()
                    {
                        Spell = Spell.Heal,
                        SpellService = new Heal(_gameStateService, null, null),
                        SpellData = _gameStateService.GetSpellData(_gameState, Spell.HolyWordSalvation)
                    },
                }
            );
        }

        [Test]
        public void Serenity_GetAverageRawHealing_Calculates_Ranks()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("PontifexPercentUsageSerenity", 0.30));
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("PontifexAverageSalvationStacks", 0.5));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.Pontifex, 0);
            var resultDefault = _holyWordSerenity.GetAverageRawHealing(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.Pontifex, 1);
            var resultRank1 = _holyWordSerenity.GetAverageRawHealing(_gameState, null);

            // Assert
            Assert.AreEqual(25498.912258372686d, resultDefault);
            Assert.AreEqual(25577.22242887651d, resultRank1);
        }

        [Test]
        public void Serenity_GetPontifexMultiplier_Calculates_Ranks()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("PontifexPercentUsageSerenity", 0.30));
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("PontifexAverageSalvationStacks", 0.5));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.Pontifex, 0);
            var resultDefault = _holyWordSerenity.GetPontifexMultiplier(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.Pontifex, 1);
            var resultRank1 = _holyWordSerenity.GetPontifexMultiplier(_gameState);

            // Assert
            Assert.AreEqual(1.0d, resultDefault);
            Assert.AreEqual(1.0030711180818355d, resultRank1);
        }

        [Test]
        public void Serenity_GetPontifexMultiplier_Throws_No_PontifexPercentUsageSerenity()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("PontifexAverageSalvationStacks", 0.5));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.Pontifex, 1);
            var methodCall = new TestDelegate(
                () => _holyWordSerenity.GetPontifexMultiplier(_gameState));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("PontifexPercentUsageSerenity needs to be set. (Parameter 'PontifexPercentUsageSerenity')"));
            Assert.That(ex.ParamName, Is.EqualTo("PontifexPercentUsageSerenity"));
        }

        [Test]
        public void Serenity_GetPontifexMultiplier_Throws_No_PontifexAverageSalvationStacks()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("PontifexPercentUsageSerenity", 0.3));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.Pontifex, 1);
            var methodCall = new TestDelegate(
                () => _holyWordSerenity.GetPontifexMultiplier(_gameState));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("PontifexAverageSalvationStacks needs to be set. (Parameter 'PontifexAverageSalvationStacks')"));
            Assert.That(ex.ParamName, Is.EqualTo("PontifexAverageSalvationStacks"));
        }

        [Test]
        public void Sanctify_GetAverageRawHealing_Calculates_Ranks()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("PontifexPercentUsageSerenity", 0.30));
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("PontifexAverageSalvationStacks", 0.5));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.Pontifex, 0);
            var resultDefault = _holyWordSanctify.GetAverageRawHealing(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.Pontifex, 1);
            var resultRank1 = _holyWordSanctify.GetAverageRawHealing(_gameState, null);

            // Assert
            Assert.AreEqual(53547.71574258263d, resultDefault);
            Assert.AreEqual(53794.438402115811d, resultRank1);
        }

        [Test]
        public void Sanctify_GetPontifexMultiplier_Calculates_Ranks()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("PontifexPercentUsageSerenity", 0.30));
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("PontifexAverageSalvationStacks", 0.5));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.Pontifex, 0);
            var resultDefault = _holyWordSanctify.GetPontifexMultiplier(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.Pontifex, 1);
            var resultRank1 = _holyWordSanctify.GetPontifexMultiplier(_gameState);

            // Assert
            Assert.AreEqual(1.0d, resultDefault);
            Assert.AreEqual(1.0046075291188747d, resultRank1);
        }

        [Test]
        public void Sanctify_GetPontifexMultiplier_Throws_No_PontifexPercentUsageSerenity()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("PontifexAverageSalvationStacks", 0.5));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.Pontifex, 1);
            var methodCall = new TestDelegate(
                () => _holyWordSanctify.GetPontifexMultiplier(_gameState));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("PontifexPercentUsageSerenity needs to be set. (Parameter 'PontifexPercentUsageSerenity')"));
            Assert.That(ex.ParamName, Is.EqualTo("PontifexPercentUsageSerenity"));
        }

        [Test]
        public void Sanctify_GetPontifexMultiplier_Throws_No_PontifexAverageSalvationStacks()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("PontifexPercentUsageSerenity", 0.3));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.Pontifex, 1);
            var methodCall = new TestDelegate(
                () => _holyWordSanctify.GetPontifexMultiplier(_gameState));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("PontifexAverageSalvationStacks needs to be set. (Parameter 'PontifexAverageSalvationStacks')"));
            Assert.That(ex.ParamName, Is.EqualTo("PontifexAverageSalvationStacks"));
        }

        [Test]
        public void Salvation_GetAverageRawHealing_Calculates_Ranks()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("PontifexAverageSalvationStacks", 0.5));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.Pontifex, 0);
            var resultDefault = _holyWordSalvation.GetAverageRawHealing(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.Pontifex, 1);
            var resultRank1 = _holyWordSalvation.GetAverageRawHealing(_gameState, null);

            // Assert
            Assert.AreEqual(80139.438526314159d, resultDefault);
            Assert.AreEqual(84146.410452629862d, resultRank1);
        }

        [Test]
        public void Salvation_GetPontifexMultiplier_Calculates_Ranks()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("PontifexAverageSalvationStacks", 0.5));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.Pontifex, 0);
            var resultDefault = _holyWordSalvation.GetPontifexMultiplier(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.Pontifex, 1);
            var resultRank1 = _holyWordSalvation.GetPontifexMultiplier(_gameState);

            // Assert
            Assert.AreEqual(1.0d, resultDefault);
            Assert.AreEqual(1.05d, resultRank1);
        }

        [Test]
        public void Salvation_GetPontifexMultiplier_Throws_No_PontifexAverageSalvationStacks()
        {
            // Arrange

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.Pontifex, 1);
            var methodCall = new TestDelegate(
                () => _holyWordSalvation.GetPontifexMultiplier(_gameState));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("PontifexAverageSalvationStacks needs to be set. (Parameter 'PontifexAverageSalvationStacks')"));
            Assert.That(ex.ParamName, Is.EqualTo("PontifexAverageSalvationStacks"));
        }

        [Test]
        public void GetPontifexSalvStacksConsumedPerMinute_Throws_No_PontifexAverageSalvationStacks()
        {
            // Arrange

            // Act
            var methodCall = new TestDelegate(
                () => Pontifex.GetPontifexSalvStacksConsumedPerMinute(_gameState, _gameStateService));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("PontifexAverageSalvationStacks needs to be set. (Parameter 'PontifexAverageSalvationStacks')"));
            Assert.That(ex.ParamName, Is.EqualTo("PontifexAverageSalvationStacks"));
        }

        [Test]
        public void GetPontifexSalvStacksConsumedPerMinute_Calculates()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("PontifexAverageSalvationStacks", 0.5));

            // Act
            var result = Pontifex.GetPontifexSalvStacksConsumedPerMinute(_gameState, _gameStateService);

            // Assert
            Assert.AreEqual(0.15136655482636452d, result);
        }
    }
}
