using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile.Model;
using Salvation.Core.State;
using System;
using System.Linq;

namespace Salvation.CoreTests.HolyPriest.Spells
{
    [TestFixture]
    public class AnsweredPrayersTests : BaseTest
    {
        private GameState _gameState;
        private AnsweredPrayers _answeredPrayersSpellService;
        IGameStateService _gameStateService;

        [SetUp]
        public void Init()
        {
            _gameStateService = new GameStateService();
            _answeredPrayersSpellService = new AnsweredPrayers(_gameStateService, 
                new PrayerOfMending(_gameStateService, null, null));

            _gameState = GetGameState();
        }

        [Test]
        public void GetDuration()
        {
            // Arrange

            // Act
            var result = _answeredPrayersSpellService.GetDuration(_gameState, null);

            // Assert
            Assert.That(result, Is.EqualTo(8.0d));
        }

        [Test]
        public void GetMaximumCastsPerMinute()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("PoMPercentageStacksExpired", 0.1));
            _gameStateService.SetTalentRank(_gameState, Spell.AnsweredPrayers, 2);

            // Act
            var result = _answeredPrayersSpellService.GetMaximumCastsPerMinute(_gameState, null);

            // Assert
            Assert.That(result, Is.EqualTo(0.4177559435294117d));
        }

        [Test]
        public void GetUptime()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("PoMPercentageStacksExpired", 0.1));
            _gameStateService.SetTalentRank(_gameState, Spell.AnsweredPrayers, 2);

            // Act
            var result = _answeredPrayersSpellService.GetUptime(_gameState, null);

            // Assert
            Assert.That(result, Is.EqualTo(0.055700792470588227d));
        }

        [Test]
        public void GetUptime_IncreasedBy_SayYourPrayers()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("PoMPercentageStacksExpired", 0.1));
            _gameStateService.SetTalentRank(_gameState, Spell.AnsweredPrayers, 2);

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.SayYourPrayers, 0);
            var result = _answeredPrayersSpellService.GetUptime(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.SayYourPrayers, 1);
            var resultRank1 = _answeredPrayersSpellService.GetUptime(_gameState, null);

            // Assert
            Assert.That(result, Is.EqualTo(0.055700792470588227d));
            Assert.That(resultRank1, Is.EqualTo(0.064055911341176455d));
        }

        [Test]
        public void GetUptime_IncreasedBy_PotV()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("PoMPercentageStacksExpired", 0.1));
            _gameStateService.SetTalentRank(_gameState, Spell.AnsweredPrayers, 2);

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.PrayersOfTheVirtuous, 0);
            var result = _answeredPrayersSpellService.GetUptime(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.PrayersOfTheVirtuous, 1);
            var resultRank1 = _answeredPrayersSpellService.GetUptime(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.PrayersOfTheVirtuous, 2);
            var resultRank2 = _answeredPrayersSpellService.GetUptime(_gameState, null);

            // Assert
            Assert.That(result, Is.EqualTo(0.055700792470588227d));
            Assert.That(resultRank1, Is.EqualTo(0.066840950964705867d));
            Assert.That(resultRank2, Is.EqualTo(0.0779811094588235d));
        }


        [Test]
        public void GetMaximumCastsPerMinute_Throws_No_PoMPercentageStacksExpired()
        {
            // Arrange
            _gameState.Profile.PlaystyleEntries.Remove(
                _gameState.Profile.PlaystyleEntries.Where(p => p.Name == "PoMPercentageStacksExpired").First());

            // Act
            var methodCall = new TestDelegate(
                () => _answeredPrayersSpellService.GetMaximumCastsPerMinute(_gameState));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("PoMPercentageStacksExpired needs to be set. (Parameter 'PoMPercentageStacksExpired')"));
            Assert.That(ex.ParamName, Is.EqualTo("PoMPercentageStacksExpired"));
        }

        [Test]
        public void GetUptime_Throws_No_PoMPercentageStacksExpired()
        {
            // Arrange
            _gameState.Profile.PlaystyleEntries.Remove(
                _gameState.Profile.PlaystyleEntries.Where(p => p.Name == "PoMPercentageStacksExpired").First());

            // Act
            var methodCall = new TestDelegate(
                () => _answeredPrayersSpellService.GetUptime(_gameState, null));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("PoMPercentageStacksExpired needs to be set. (Parameter 'PoMPercentageStacksExpired')"));
            Assert.That(ex.ParamName, Is.EqualTo("PoMPercentageStacksExpired"));
        }

        [Test]
        public void HW_CDR_Calculates_From_Uptime()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("PoMPercentageStacksExpired", 0.1));
            _gameState.RegisteredSpells.Add(new RegisteredSpell()
            {
                Spell = Spell.AnsweredPrayers,
                SpellData = _gameStateService.GetSpellData(_gameState, Spell.AnsweredPrayers),
                SpellService = _answeredPrayersSpellService,
            });
            _gameStateService.SetTalentRank(_gameState, Spell.AnsweredPrayers, 1);

            // Act
            var result_FH = _gameStateService.GetTotalHolyWordCooldownReduction(_gameState, Spell.FlashHeal);
            var result_PoH = _gameStateService.GetTotalHolyWordCooldownReduction(_gameState, Spell.PrayerOfHealing);
            var result_Renew = _gameStateService.GetTotalHolyWordCooldownReduction(_gameState, Spell.Renew);
            var result_Smite = _gameStateService.GetTotalHolyWordCooldownReduction(_gameState, Spell.Smite);

            // Assert
            Assert.That(result_FH, Is.EqualTo(6.5013071322352944d));
            Assert.That(result_PoH, Is.EqualTo(6.5013071322352944d));
            Assert.That(result_Renew, Is.EqualTo(2.1671023774117648d));
            Assert.That(result_Smite, Is.EqualTo(4.3342047548235296d));
        }

        [Test]
        public void HW_CDR_Calculates_From_Uptime_With_HA()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("PoMPercentageStacksExpired", 0.1));
            _gameState.RegisteredSpells.Add(new RegisteredSpell()
            {
                Spell = Spell.AnsweredPrayers,
                SpellData = _gameStateService.GetSpellData(_gameState, Spell.AnsweredPrayers),
                SpellService = _answeredPrayersSpellService,
            });
            _gameStateService.SetTalentRank(_gameState, Spell.AnsweredPrayers, 1);
            _gameStateService.SetTalentRank(_gameState, Spell.HarmoniousApparatus, 1);

            // Act
            var result_PoM = _gameStateService.GetTotalHolyWordCooldownReduction(_gameState, Spell.PrayerOfMending);
            var result_CoH = _gameStateService.GetTotalHolyWordCooldownReduction(_gameState, Spell.CircleOfHealing);
            var result_HF = _gameStateService.GetTotalHolyWordCooldownReduction(_gameState, Spell.HolyFire);

            // Assert
            Assert.That(result_PoM, Is.EqualTo(2.1671023774117648d));
            Assert.That(result_CoH, Is.EqualTo(2.1671023774117648d));
            Assert.That(result_HF, Is.EqualTo(2.1671023774117648d));
        }
    }
}
