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
    public class ResonantWordsTests : BaseTest
    {
        private GameState _gameState;
        private GameStateService _gameStateService;

        [SetUp]
        public void Init()
        {
            _gameState = GetGameState();
            _gameStateService = new GameStateService();

            var serenity = new HolyWordSerenity(_gameStateService,
                new FlashHeal(_gameStateService, null, null),
                new Heal(_gameStateService, null, null),
                new PrayerOfMending(_gameStateService, null, null));

            var sanc = new HolyWordSanctify(_gameStateService,
                new PrayerOfHealing(_gameStateService, new Renew(_gameStateService, null)),
                new Renew(_gameStateService, null),
                new CircleOfHealing(_gameStateService));

            var chastise = new HolyWordChastise(_gameStateService, 
                new Smite(_gameStateService), 
                new HolyFire(_gameStateService));

            _gameState.RegisteredSpells.AddRange(
                new List<RegisteredSpell>()
                {
                    new RegisteredSpell()
                    {
                        Spell = Spell.HolyWordSerenity,
                        SpellService = serenity,
                        SpellData = _gameStateService.GetSpellData(_gameState, Spell.HolyWordSerenity)
                    },
                    new RegisteredSpell()
                    {
                        Spell = Spell.HolyWordSanctify,
                        SpellService = sanc,
                        SpellData = _gameStateService.GetSpellData(_gameState, Spell.HolyWordSanctify)
                    },
                    new RegisteredSpell()
                    {
                        Spell = Spell.HolyWordChastise,
                        SpellService = chastise,
                        SpellData = _gameStateService.GetSpellData(_gameState, Spell.HolyWordChastise)
                    }
                }
            );
        }

        [Test]
        public void RW_FH_GetAverageRawHealing_Calculates_Ranks()
        {
            // Arrange
            var spellService = new FlashHeal(_gameStateService, null, null);

            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("ResonantWordsPercentageBuffsUsed", 0.99));

            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("ResonantWordsPercentageBuffsHeal", 0.9));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.ResonantWords, 0);
            var resultDefault = spellService.GetAverageRawHealing(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.ResonantWords, 1);
            var resultRank1 = spellService.GetAverageRawHealing(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.ResonantWords, 2);
            var resultRank2 = spellService.GetAverageRawHealing(_gameState, null);

            // Assert
            Assert.AreEqual(7394.684554928077d, resultDefault);
            Assert.AreEqual(7687.2000190616582d, resultRank1);
            Assert.AreEqual(7979.7154831952384d, resultRank2);
        }

        [Test]
        public void RW_FH_GetResonantWordsMulti_Calculates_Ranks()
        {
            // Arrange
            var spellService = new FlashHeal(_gameStateService, null, null);

            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("ResonantWordsPercentageBuffsUsed", 0.99));

            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("ResonantWordsPercentageBuffsHeal", 0.9));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.ResonantWords, 0);
            var resultDefault = spellService.GetResonantWordsMulti(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.ResonantWords, 1);
            var resultRank1 = spellService.GetResonantWordsMulti(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.ResonantWords, 2);
            var resultRank2 = spellService.GetResonantWordsMulti(_gameState, null);

            // Assert
            Assert.AreEqual(1.0d, resultDefault);
            Assert.AreEqual(1.0395575310834102d, resultRank1);
            Assert.AreEqual(1.0791150621668204d, resultRank2);
        }

        [Test]
        public void RW_FH_THrows_No_ResonantWordsPercentageBuffsUsed()
        {
            // Arrange
            var spellService = new FlashHeal(_gameStateService, null, null);

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.ResonantWords, 1);
            var methodCall = new TestDelegate(
                () => spellService.GetAverageRawHealing(_gameState, null));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("ResonantWordsPercentageBuffsUsed needs to be set. (Parameter 'ResonantWordsPercentageBuffsUsed')"));
            Assert.That(ex.ParamName, Is.EqualTo("ResonantWordsPercentageBuffsUsed"));
        }

        [Test]
        public void RW_FH_THrows_No_ResonantWordsPercentageBuffsHeal()
        {
            // Arrange
            var spellService = new FlashHeal(_gameStateService, null, null);

            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("ResonantWordsPercentageBuffsUsed", 0.99));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.ResonantWords, 1);
            var methodCall = new TestDelegate(
                () => spellService.GetAverageRawHealing(_gameState, null));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("ResonantWordsPercentageBuffsHeal needs to be set. (Parameter 'ResonantWordsPercentageBuffsHeal')"));
            Assert.That(ex.ParamName, Is.EqualTo("ResonantWordsPercentageBuffsHeal"));
        }

        [Test]
        public void RW_Heal_GetAverageRawHealing_Calculates_Ranks()
        {
            // Arrange
            var spellService = new Heal(_gameStateService, null, null);

            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("ResonantWordsPercentageBuffsUsed", 0.99));

            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("ResonantWordsPercentageBuffsHeal", 0.1));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.ResonantWords, 0);
            var resultDefault = spellService.GetAverageRawHealing(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.ResonantWords, 1);
            var resultRank1 = spellService.GetAverageRawHealing(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.ResonantWords, 2);
            var resultRank2 = spellService.GetAverageRawHealing(_gameState, null);

            // Assert
            Assert.AreEqual(10745.970166028488d, resultDefault);
            Assert.AreEqual(11389.357920709801d, resultRank1);
            Assert.AreEqual(12032.745675391114d, resultRank2);
        }

        [Test]
        public void RW_Heal_GetResonantWordsMulti_Calculates_Ranks()
        {
            // Arrange
            var spellService = new Heal(_gameStateService, null, null);

            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("ResonantWordsPercentageBuffsUsed", 0.99));

            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("ResonantWordsPercentageBuffsHeal", 0.1));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.ResonantWords, 0);
            var resultDefault = spellService.GetResonantWordsMulti(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.ResonantWords, 1);
            var resultRank1 = spellService.GetResonantWordsMulti(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.ResonantWords, 2);
            var resultRank2 = spellService.GetResonantWordsMulti(_gameState, null);

            // Assert
            Assert.AreEqual(1.0d, resultDefault);
            Assert.AreEqual(1.0598724679801617d, resultRank1);
            Assert.AreEqual(1.1197449359603233d, resultRank2);
        }

        [Test]
        public void RW_Heal_THrows_No_ResonantWordsPercentageBuffsUsed()
        {
            // Arrange
            var spellService = new Heal(_gameStateService, null, null);

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.ResonantWords, 1);
            var methodCall = new TestDelegate(
                () => spellService.GetAverageRawHealing(_gameState, null));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("ResonantWordsPercentageBuffsUsed needs to be set. (Parameter 'ResonantWordsPercentageBuffsUsed')"));
            Assert.That(ex.ParamName, Is.EqualTo("ResonantWordsPercentageBuffsUsed"));
        }

        [Test]
        public void RW_Heal_THrows_No_ResonantWordsPercentageBuffsHeal()
        {
            // Arrange
            var spellService = new Heal(_gameStateService, null, null);

            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("ResonantWordsPercentageBuffsUsed", 0.99));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.ResonantWords, 1);
            var methodCall = new TestDelegate(
                () => spellService.GetAverageRawHealing(_gameState, null));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("ResonantWordsPercentageBuffsHeal needs to be set. (Parameter 'ResonantWordsPercentageBuffsHeal')"));
            Assert.That(ex.ParamName, Is.EqualTo("ResonantWordsPercentageBuffsHeal"));
        }
    }
}
