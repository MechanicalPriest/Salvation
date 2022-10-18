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
    public class PrayerCircleTests : BaseTest
    {
        private GameState _gameState;
        private IGameStateService _gameStateService;
        private PrayerOfHealing _spell;
        [SetUp]
        public void Init()
        {
            _gameState = GetGameState();
            _gameStateService = new GameStateService();
            _spell = new PrayerOfHealing(_gameStateService, new Renew(_gameStateService, null));
        }

        [Test]
        public void GetHastedCastTime_Calculates_Ranks()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("PrayerCircleUptime", 0.8));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.PrayerCircle, 0);
            var resultDefault = _spell.GetHastedCastTime(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.PrayerCircle, 1);
            var resultRank1 = _spell.GetHastedCastTime(_gameState, null);

            // Assert
            Assert.That(resultDefault, Is.EqualTo(1.95099558156883d));
            Assert.That(resultRank1, Is.EqualTo(1.5607964652550641d));
        }

        [Test]
        public void GetPrayerCircleCastTimeMultiplier_Calculates_Ranks()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("PrayerCircleUptime", 0.8));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.PrayerCircle, 0);
            var resultDefault = _spell.GetPrayerCircleCastTimeMultiplier(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.PrayerCircle, 1);
            var resultRank1 = _spell.GetPrayerCircleCastTimeMultiplier(_gameState);

            // Assert
            Assert.That(resultDefault, Is.EqualTo(1.0d));
            Assert.That(resultRank1, Is.EqualTo(0.80000000000000004d));
        }

        [Test]
        public void GetPrayerCircleCastTimeMultiplier_Throws_No_UnwaveringWillUptime()
        {
            // Arrange

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.PrayerCircle, 1);
            var methodCall = new TestDelegate(
                () => _spell.GetPrayerCircleCastTimeMultiplier(_gameState));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("PrayerCircleUptime needs to be set. (Parameter 'PrayerCircleUptime')"));
            Assert.That(ex.ParamName, Is.EqualTo("PrayerCircleUptime"));
        }

        [Test]
        public void GetActualManaCost_Calculates_Ranks()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("PrayerCircleUptime", 0.8));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.PrayerCircle, 0);
            var resultDefault = _spell.GetActualManaCost(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.PrayerCircle, 1);
            var resultRank1 = _spell.GetActualManaCost(_gameState, null);

            // Assert
            Assert.That(resultDefault, Is.EqualTo(10000.0d));
            Assert.That(resultRank1, Is.EqualTo(8000.0d));
        }

        [Test]
        public void GetPrayerCircleManaReductionMultiplier_Calculates_Ranks()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("PrayerCircleUptime", 0.8));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.PrayerCircle, 0);
            var resultDefault = _spell.GetPrayerCircleManaReductionMultiplier(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.PrayerCircle, 1);
            var resultRank1 = _spell.GetPrayerCircleManaReductionMultiplier(_gameState);

            // Assert
            Assert.That(resultDefault, Is.EqualTo(1.0d));
            Assert.That(resultRank1, Is.EqualTo(0.80000000000000004d));
        }

        [Test]
        public void GetPrayerCircleManaReductionMultiplier_Throws_No_UnwaveringWillUptime()
        {
            // Arrange

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.PrayerCircle, 1);
            var methodCall = new TestDelegate(
                () => _spell.GetPrayerCircleManaReductionMultiplier(_gameState));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("PrayerCircleUptime needs to be set. (Parameter 'PrayerCircleUptime')"));
            Assert.That(ex.ParamName, Is.EqualTo("PrayerCircleUptime"));
        }
    }
}
