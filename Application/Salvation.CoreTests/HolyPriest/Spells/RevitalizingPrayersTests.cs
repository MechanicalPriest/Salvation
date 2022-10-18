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
    public class RevitalizingPrayersTests : BaseTest
    {
        private GameState _gameState;
        private PrayerOfHealing _prayerOfHealingSpellService;
        IGameStateService _gameStateService;

        [SetUp]
        public void Init()
        {
            _gameStateService = new GameStateService();
            _prayerOfHealingSpellService = new PrayerOfHealing(_gameStateService, 
                new Renew(_gameStateService, null));

            _gameState = GetGameState(); 

            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("GroupSize", 20));
        }

        [Test]
        public void GetRevitalizingPrayersCpm_Calculates_Ranks()
        {
            // Arrange

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.RevitalizingPrayers, 0);
            var result = _prayerOfHealingSpellService.GetRevitalizingPrayersCpm(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.RevitalizingPrayers, 1);
            var resultRank1 = _prayerOfHealingSpellService.GetRevitalizingPrayersCpm(_gameState, null);

            // Assert
            Assert.That(result, Is.EqualTo(0.0d));
            Assert.That(resultRank1, Is.EqualTo(13.520789205882355d));
        }

        [Test]
        public void GetRenewTicksPerMinute_Calculates_Ranks()
        {
            // Arrange

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.RevitalizingPrayers, 0);
            var result = _prayerOfHealingSpellService.GetRenewTicksPerMinute(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.RevitalizingPrayers, 1);
            var resultRank1 = _prayerOfHealingSpellService.GetRenewTicksPerMinute(_gameState, null);

            // Assert
            Assert.That(result, Is.EqualTo(0.0d));
            Assert.That(resultRank1, Is.EqualTo(54.083156823529421d));
        }

        [Test]
        public void GetRenewUptime_Calculates_Ranks()
        {
            // Arrange

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.RevitalizingPrayers, 0);
            var result = _prayerOfHealingSpellService.GetRenewUptime(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.RevitalizingPrayers, 1);
            var resultRank1 = _prayerOfHealingSpellService.GetRenewUptime(_gameState, null);

            // Assert
            Assert.That(result, Is.EqualTo(0.0d));
            Assert.That(resultRank1, Is.EqualTo(0.067603946029411777d));
        }

        [Test]
        public void GetRenewUptime_Throws_No_GroupSize()
        {
            // Arrange
            _gameState.Profile.PlaystyleEntries.Remove(
                _gameState.Profile.PlaystyleEntries.Where(p => p.Name == "GroupSize").First());
            _gameStateService.SetTalentRank(_gameState, Spell.RevitalizingPrayers, 1);

            // Act
            var methodCall = new TestDelegate(
                () => _prayerOfHealingSpellService.GetRenewUptime(_gameState, null));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("GroupSize needs to be set. (Parameter 'GroupSize')"));
            Assert.That(ex.ParamName, Is.EqualTo("GroupSize"));
        }

        [Test]
        public void GetCastResults_Calculates_Renew()
        {
            // Arrange

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.RevitalizingPrayers, 0);
            var resultRank0 = _prayerOfHealingSpellService.GetCastResults(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.RevitalizingPrayers, 1);
            var resultRank1 = _prayerOfHealingSpellService.GetCastResults(_gameState, null);

            // Assert
            Assert.That(resultRank0.AdditionalCasts.Count, Is.EqualTo(1));
            Assert.That(resultRank0.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EchoOfLight).Any(), Is.True);

            Assert.That(resultRank1.AdditionalCasts.Count, Is.EqualTo(2));
            Assert.That(resultRank1.RawHealing, Is.EqualTo(15936.820161482929d));
            Assert.That(resultRank1.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EchoOfLight).Any(), Is.True);
            Assert.That(resultRank1.AdditionalCasts.Where(c => c.SpellId == (int)Spell.Renew).Any(), Is.True);

            var renewCast1 = resultRank1.AdditionalCasts.Where(c => c.SpellId == (int)Spell.Renew).First();
            Assert.That(renewCast1.AdditionalCasts.Count, Is.EqualTo(0));
            Assert.That(renewCast1.RawHealing, Is.EqualTo(3555.5511831815993d));
        }
    }
}
