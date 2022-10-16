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
    public class ApotheosisTests : BaseTest
    {
        private GameState _gameState;
        private Apotheosis _apotheosisSpellService;
        IGameStateService _gameStateService;

        [SetUp]
        public void Init()
        {
            _gameStateService = new GameStateService();
            _apotheosisSpellService = new Apotheosis(_gameStateService);

            _gameState = GetGameState();
        }

        [Test]
        public void GetDuration()
        {
            // Arrange

            // Act
            var result = _apotheosisSpellService.GetDuration(_gameState, null);

            // Assert
            Assert.That(result, Is.EqualTo(20.0d));
        }

        [Test]
        public void GetMaximumCastsPerMinute()
        {
            // Arrange

            // Act
            var result = _apotheosisSpellService.GetMaximumCastsPerMinute(_gameState, null);

            // Assert
            Assert.That(result, Is.EqualTo(0.65113350125944591d));
        }

        [Test]
        public void GetUptime()
        {
            // Arrange

            // Act
            var result = _apotheosisSpellService.GetUptime(_gameState, null);

            // Assert
            Assert.That(result, Is.EqualTo(0.21704450041981532d));
        }

        [Test]
        public void HW_CDR_Calculates_From_Uptime()
        {
            // Arrange
            _gameState.RegisteredSpells.Add(new RegisteredSpell()
            {
                Spell = Spell.Apotheosis,
                SpellData = _gameStateService.GetSpellData(_gameState, Spell.Apotheosis),
                SpellService = _apotheosisSpellService,
            });
            _gameStateService.SetTalentRank(_gameState, Spell.Apotheosis, 1);

            // Act
            var result_FH = _gameStateService.GetTotalHolyWordCooldownReduction(_gameState, Spell.FlashHeal);
            var result_PoH = _gameStateService.GetTotalHolyWordCooldownReduction(_gameState, Spell.PrayerOfHealing);
            var result_Renew = _gameStateService.GetTotalHolyWordCooldownReduction(_gameState, Spell.Renew);
            var result_Smite = _gameStateService.GetTotalHolyWordCooldownReduction(_gameState, Spell.Smite);

            // Assert
            Assert.That(result_FH, Is.EqualTo(9.906801007556675d));
            Assert.That(result_PoH, Is.EqualTo(9.906801007556675d));
            Assert.That(result_Renew, Is.EqualTo(3.3022670025188918d));
            Assert.That(result_Smite, Is.EqualTo(6.6045340050377837d));
        }

        [Test]
        public void HW_CDR_Calculates_From_Uptime_With_HA()
        {
            // Arrange
            _gameState.RegisteredSpells.Add(new RegisteredSpell()
            {
                Spell = Spell.Apotheosis,
                SpellData = _gameStateService.GetSpellData(_gameState, Spell.Apotheosis),
                SpellService = _apotheosisSpellService,
            });
            _gameStateService.SetTalentRank(_gameState, Spell.Apotheosis, 1);
            _gameStateService.SetTalentRank(_gameState, Spell.HarmoniousApparatus, 1);

            // Act
            var result_PoM = _gameStateService.GetTotalHolyWordCooldownReduction(_gameState, Spell.PrayerOfMending);
            var result_CoH = _gameStateService.GetTotalHolyWordCooldownReduction(_gameState, Spell.CircleOfHealing);
            var result_HF = _gameStateService.GetTotalHolyWordCooldownReduction(_gameState, Spell.HolyFire);

            // Assert
            Assert.That(result_PoM, Is.EqualTo(3.3022670025188918d));
            Assert.That(result_CoH, Is.EqualTo(3.3022670025188918d));
            Assert.That(result_HF, Is.EqualTo(3.3022670025188918d));
        }
    }
}
