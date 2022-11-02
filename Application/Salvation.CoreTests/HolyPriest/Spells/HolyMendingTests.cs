using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile.Model;
using Salvation.Core.State;
using System;
using System.Linq;

namespace Salvation.CoreTests.HolyPriest.Spells
{
    [TestFixture]
    public class HolyMendingTests : BaseTest
    {
        private GameState _gameState;
        private HolyMending _holyMendingSpellService;
        IGameStateService _gameStateService;

        [SetUp]
        public void Init()
        {
            _gameStateService = new GameStateService();
            _holyMendingSpellService = new HolyMending(_gameStateService);

            _gameState = GetGameState();
        }

        [Test]
        public void TriggersMastery_Uses_HealSpellData()
        {
            // Arrange

            // Act
            var result = _holyMendingSpellService.TriggersMastery(_gameState, null);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void GetAverageRawHealing_Calculates_Ranks()
        {
            // Arrange

            // Act
            var resultRank0 = _holyMendingSpellService.GetAverageRawHealing(_gameState, null);

            // Assert
            Assert.That(resultRank0, Is.EqualTo(1434.3138145334638d));
        }

        [Test]
        public void GetNumberOfHealingTargets_Calculates()
        {
            // Arrange

            // Act
            var result = _holyMendingSpellService.GetNumberOfHealingTargets(_gameState, null);

            // Assert
            Assert.That(result, Is.EqualTo(1.0d));
        }

        [Test]
        public void GetActualCastsPerMinute_Calculates()
        {
            // Arrange
            var cpm = 12345d;
            var spellData = _gameStateService.GetSpellData(_gameState, Spell.HolyMending);
            spellData.Overrides[Override.CastsPerMinute] = cpm;

            // Act
            var result = _holyMendingSpellService.GetActualCastsPerMinute(_gameState, spellData);

            // Assert
            Assert.That(result, Is.EqualTo(cpm));
        }

        [Test]
        public void Renew_Calculates_Ranks()
        {
            // Arrange
            var prayerOfMendingSpellService = new PrayerOfMending(_gameStateService, 
                null,
                _holyMendingSpellService);

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.HolyMending, 0);
            var resultRank0 = prayerOfMendingSpellService.GetCastResults(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.HolyMending, 1);
            var resultRank1 = prayerOfMendingSpellService.GetCastResults(_gameState);

            // Assert
            Assert.That(resultRank0.AdditionalCasts.Count, Is.EqualTo(1));

            Assert.That(resultRank1.AdditionalCasts.Count, Is.EqualTo(2));
            Assert.That(resultRank1.RawHealing, Is.EqualTo(10971.640092892278d));
            Assert.That(resultRank1.AdditionalCasts.Where(c => c.SpellId == (int)Spell.HolyMending).Any(), Is.True);

            var holyMendingCast1 = resultRank1.AdditionalCasts.Where(c => c.SpellId == (int)Spell.HolyMending).First();
            Assert.That(holyMendingCast1.AdditionalCasts.Count, Is.EqualTo(1));
            Assert.That(holyMendingCast1.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EchoOfLight).Any(), Is.True);
            Assert.That(holyMendingCast1.RawHealing, Is.EqualTo(1434.3138145334638d));
        }
    }
}
