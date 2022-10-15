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
    public class TrailOfLightTests : BaseTest
    {
        private GameState _gameState;
        private TrailOfLight _trailOfLightSpellService;
        IGameStateService _gameStateService;

        [SetUp]
        public void Init()
        {
            _gameStateService = new GameStateService();
            _trailOfLightSpellService = new TrailOfLight(_gameStateService);

            _gameState = GetGameState();
        }

        [Test]
        public void TriggersMastery_Uses_HealSpellData()
        {
            // Arrange

            // Act
            var result = _trailOfLightSpellService.TriggersMastery(_gameState, null);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void GetAverageRawHealing_Calculates_Ranks()
        {
            // Arrange
            var externalHealAmount = 12345.67d;
            var spellData = _gameStateService.GetSpellData(_gameState, Spell.TrailOfLight);
            spellData.Overrides[Override.ResultMultiplier] = externalHealAmount;

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.TrailOfLight, 0);
            var resultRank0 = _trailOfLightSpellService.GetAverageRawHealing(_gameState, spellData);

            _gameStateService.SetTalentRank(_gameState, Spell.TrailOfLight, 1);
            var resultRank1 = _trailOfLightSpellService.GetAverageRawHealing(_gameState, spellData);

            _gameStateService.SetTalentRank(_gameState, Spell.TrailOfLight, 2);
            var resultRank2 = _trailOfLightSpellService.GetAverageRawHealing(_gameState, spellData);

            // Assert
            Assert.That(resultRank0, Is.EqualTo(0.0d));
            Assert.That(resultRank1, Is.EqualTo(2222.2206000000001d));
            Assert.That(resultRank2, Is.EqualTo(4444.4412000000002d));
        }

        [Test]
        public void GetNumberOfHealingTargets_Calculates()
        {
            // Arrange

            // Act
            var result = _trailOfLightSpellService.GetNumberOfHealingTargets(_gameState, null);

            // Assert
            Assert.That(result, Is.EqualTo(1.0d));
        }

        [Test]
        public void FH_Calculates_Ranks()
        {
            // Arrange
            var fhSpellService = new FlashHeal(_gameStateService, _trailOfLightSpellService, null);

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.TrailOfLight, 0);
            var resultRank0 = fhSpellService.GetCastResults(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.TrailOfLight, 1);
            var resultRank1 = fhSpellService.GetCastResults(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.TrailOfLight, 2);
            var resultRank2 = fhSpellService.GetCastResults(_gameState);

            // Assert
            Assert.That(resultRank0.AdditionalCasts.Count, Is.EqualTo(1));
            Assert.That(resultRank0.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EchoOfLight).Any(), Is.True);

            Assert.That(resultRank1.AdditionalCasts.Count, Is.EqualTo(2));
            Assert.That(resultRank1.RawHealing, Is.EqualTo(7394.684554928077d));
            Assert.That(resultRank1.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EchoOfLight).Any(), Is.True);
            Assert.That(resultRank1.AdditionalCasts.Where(c => c.SpellId == (int)Spell.TrailOfLight).Any(), Is.True);

            var trailCast1 = resultRank1.AdditionalCasts.Where(c => c.SpellId == (int)Spell.TrailOfLight).First();
            Assert.That(trailCast1.AdditionalCasts.Count, Is.EqualTo(1));
            Assert.That(trailCast1.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EchoOfLight).Any(), Is.True);
            Assert.That(trailCast1.RawHealing, Is.EqualTo(1331.0432198870537d));

            Assert.That(resultRank2.AdditionalCasts.Count, Is.EqualTo(2));
            Assert.That(resultRank2.RawHealing, Is.EqualTo(7394.684554928077d));
            Assert.That(resultRank2.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EchoOfLight).Any(), Is.True);
            Assert.That(resultRank2.AdditionalCasts.Where(c => c.SpellId == (int)Spell.TrailOfLight).Any(), Is.True);

            var trailCast2 = resultRank2.AdditionalCasts.Where(c => c.SpellId == (int)Spell.TrailOfLight).First();
            Assert.That(trailCast2.AdditionalCasts.Count, Is.EqualTo(1));
            Assert.That(trailCast2.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EchoOfLight).Any(), Is.True);
            Assert.That(trailCast2.RawHealing, Is.EqualTo(2662.0864397741075d));
        }

        [Test]
        public void Heal_Calculates_Ranks()
        {
            // Arrange
            var healSpellService = new Heal(_gameStateService, _trailOfLightSpellService, null);

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.TrailOfLight, 0);
            var resultRank0 = healSpellService.GetCastResults(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.TrailOfLight, 1);
            var resultRank1 = healSpellService.GetCastResults(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.TrailOfLight, 2);
            var resultRank2 = healSpellService.GetCastResults(_gameState);

            // Assert
            Assert.That(resultRank0.AdditionalCasts.Count, Is.EqualTo(1));
            Assert.That(resultRank0.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EchoOfLight).Any(), Is.True);

            Assert.That(resultRank1.AdditionalCasts.Count, Is.EqualTo(2));
            Assert.That(resultRank1.RawHealing, Is.EqualTo(10745.970166028488d));
            Assert.That(resultRank1.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EchoOfLight).Any(), Is.True);
            Assert.That(resultRank1.AdditionalCasts.Where(c => c.SpellId == (int)Spell.TrailOfLight).Any(), Is.True);

            var trailCast1 = resultRank1.AdditionalCasts.Where(c => c.SpellId == (int)Spell.TrailOfLight).First();
            Assert.That(trailCast1.AdditionalCasts.Count, Is.EqualTo(1));
            Assert.That(trailCast1.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EchoOfLight).Any(), Is.True);
            Assert.That(trailCast1.RawHealing, Is.EqualTo(1934.2746298851278d));

            Assert.That(resultRank2.AdditionalCasts.Count, Is.EqualTo(2));
            Assert.That(resultRank2.RawHealing, Is.EqualTo(10745.970166028488d));
            Assert.That(resultRank2.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EchoOfLight).Any(), Is.True);
            Assert.That(resultRank2.AdditionalCasts.Where(c => c.SpellId == (int)Spell.TrailOfLight).Any(), Is.True);

            var trailCast2 = resultRank2.AdditionalCasts.Where(c => c.SpellId == (int)Spell.TrailOfLight).First();
            Assert.That(trailCast2.AdditionalCasts.Count, Is.EqualTo(1));
            Assert.That(trailCast2.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EchoOfLight).Any(), Is.True);
            Assert.That(trailCast2.RawHealing, Is.EqualTo(3868.5492597702555d));
        }

        [Test]
        public void GetAverageRawHealing_Throws_No_UnwaveringWillUptime()
        {
            // Arrange

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.PrayerCircle, 1);
            var methodCall = new TestDelegate(
                () => _trailOfLightSpellService.GetAverageRawHealing(_gameState, null));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("SpellData Override.ResultMultiplier must be set. (Parameter 'Override.ResultMultiplier')"));
            Assert.That(ex.ParamName, Is.EqualTo("Override.ResultMultiplier"));
        }

        [Test]
        public void GetActualCastsPerMinute_Throws_No_UnwaveringWillUptime()
        {
            // Arrange

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.PrayerCircle, 1);
            var methodCall = new TestDelegate(
                () => _trailOfLightSpellService.GetActualCastsPerMinute(_gameState, null));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("SpellData Override.CastsPerMinute must be set. (Parameter 'Override.CastsPerMinute')"));
            Assert.That(ex.ParamName, Is.EqualTo("Override.CastsPerMinute"));
        }
    }
}
