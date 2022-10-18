using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile.Model;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.HolyPriest.Spells
{
    [TestFixture]
    public class SanctifiedPrayersTests : BaseTest
    {
        private GameState _gameState;
        private IGameStateService _gameStateService;
        private PrayerOfHealing _spell;
        [SetUp]
        public void Init()
        {
            _gameState = GetGameState();
            _gameStateService = new GameStateService();
            _spell = new PrayerOfHealing(_gameStateService, new Renew(_gameStateService));
        }

        [Test]
        public void GetAverageRawHealing_Calculates_Ranks()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("SanctifiedPrayersUptime", 0.8));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.SanctifiedPrayers, 0);
            var resultDefault = _spell.GetAverageRawHealing(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.SanctifiedPrayers, 1);
            var resultRank1 = _spell.GetAverageRawHealing(_gameState, null);

            // Assert
            Assert.AreEqual(15936.820161482929d, resultDefault);
            Assert.AreEqual(17849.238580860881d, resultRank1);
        }

        [Test]
        public void GetSanctifiedPrayersMultiplier_Calculates_Ranks()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("SanctifiedPrayersUptime", 0.8));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.SanctifiedPrayers, 0);
            var resultDefault = _spell.GetSanctifiedPrayersMultiplier(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.SanctifiedPrayers, 1);
            var resultRank1 = _spell.GetSanctifiedPrayersMultiplier(_gameState);

            // Assert
            Assert.AreEqual(1.0d, resultDefault);
            Assert.AreEqual(1.1200000000000001d, resultRank1);
        }

        [Test]
        public void FH_GetUnwaveringWillMultiplier_Throws_No_UnwaveringWillUptime()
        {
            // Arrange

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.SanctifiedPrayers, 1);
            var methodCall = new TestDelegate(
                () => _spell.GetSanctifiedPrayersMultiplier(_gameState));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("SanctifiedPrayersUptime needs to be set. (Parameter 'SanctifiedPrayersUptime')"));
            Assert.That(ex.ParamName, Is.EqualTo("SanctifiedPrayersUptime"));
        }
    }
}
