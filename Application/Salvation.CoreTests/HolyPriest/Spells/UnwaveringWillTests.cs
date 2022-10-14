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
    public class UnwaveringWillTests : BaseTest
    {
        private GameState _gameState;
        private GameStateService _gameStateService;
        private FlashHeal _flashHeal;
        private Heal _heal;
        private PrayerOfHealing _prayerOfHealing;
        private Smite _smite;

        [SetUp]
        public void Init()
        {
            _gameState = GetGameState();
            _gameStateService = new GameStateService();

            _flashHeal = new FlashHeal(_gameStateService, null);
            _heal = new Heal(_gameStateService, null);
            _prayerOfHealing = new PrayerOfHealing(_gameStateService);
            _smite = new Smite(_gameStateService);
        }

        [Test]
        public void FH_GetHastedCastTime_Calculates_Ranks()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("UnwaveringWillUptime", 0.9));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 0);
            var resultDefault = _flashHeal.GetHastedCastTime(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 1);
            var resultRank1 = _flashHeal.GetHastedCastTime(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 2);
            var resultRank2 = _flashHeal.GetHastedCastTime(_gameState, null);

            // Assert
            Assert.AreEqual(1.4632466861766227d, resultDefault);
            Assert.AreEqual(1.3974005852986746d, resultRank1);
            Assert.AreEqual(1.3315544844207268d, resultRank2);
        }

        [Test]
        public void FH_GetUnwaveringWillMultiplier_Calculates_Ranks()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("UnwaveringWillUptime", 0.9));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 0);
            var resultDefault = _flashHeal.GetUnwaveringWillMultiplier(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 1);
            var resultRank1 = _flashHeal.GetUnwaveringWillMultiplier(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 2);
            var resultRank2 = _flashHeal.GetUnwaveringWillMultiplier(_gameState);

            // Assert
            Assert.AreEqual(1.0d, resultDefault);
            Assert.AreEqual(0.95499999999999996d, resultRank1);
            Assert.AreEqual(0.91000000000000003d, resultRank2);
        }

        [Test]
        public void FH_GetUnwaveringWillMultiplier_Throws_No_UnwaveringWillUptime()
        {
            // Arrange

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 1);
            var methodCall = new TestDelegate(
                () => _flashHeal.GetUnwaveringWillMultiplier(_gameState));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("UnwaveringWillUptime needs to be set. (Parameter 'UnwaveringWillUptime')"));
            Assert.That(ex.ParamName, Is.EqualTo("UnwaveringWillUptime"));
        }

        [Test]
        public void Heal_GetHastedCastTime_Calculates_Ranks()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("UnwaveringWillUptime", 0.9));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 0);
            var resultDefault = _heal.GetHastedCastTime(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 1);
            var resultRank1 = _heal.GetHastedCastTime(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 2);
            var resultRank2 = _heal.GetHastedCastTime(_gameState, null);

            // Assert
            Assert.AreEqual(2.4387444769610376d, resultDefault);
            Assert.AreEqual(2.3290009754977907d, resultRank1);
            Assert.AreEqual(2.2192574740345443d, resultRank2);
        }

        [Test]
        public void Heal_GetUnwaveringWillMultiplier_Calculates_Ranks()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("UnwaveringWillUptime", 0.9));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 0);
            var resultDefault = _heal.GetUnwaveringWillMultiplier(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 1);
            var resultRank1 = _heal.GetUnwaveringWillMultiplier(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 2);
            var resultRank2 = _heal.GetUnwaveringWillMultiplier(_gameState);

            // Assert
            Assert.AreEqual(1.0d, resultDefault);
            Assert.AreEqual(0.95499999999999996d, resultRank1);
            Assert.AreEqual(0.91000000000000003d, resultRank2);
        }

        [Test]
        public void Heal_GetUnwaveringWillMultiplier_Throws_No_UnwaveringWillUptime()
        {
            // Arrange

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 1);
            var methodCall = new TestDelegate(
                () => _heal.GetUnwaveringWillMultiplier(_gameState));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("UnwaveringWillUptime needs to be set. (Parameter 'UnwaveringWillUptime')"));
            Assert.That(ex.ParamName, Is.EqualTo("UnwaveringWillUptime"));
        }

        [Test]
        public void PoH_GetHastedCastTime_Calculates_Ranks()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("UnwaveringWillUptime", 0.9));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 0);
            var resultDefault = _prayerOfHealing.GetHastedCastTime(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 1);
            var resultRank1 = _prayerOfHealing.GetHastedCastTime(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 2);
            var resultRank2 = _prayerOfHealing.GetHastedCastTime(_gameState, null);

            // Assert
            Assert.AreEqual(1.95099558156883d, resultDefault);
            Assert.AreEqual(1.8632007803982327d, resultRank1);
            Assert.AreEqual(1.7754059792276353d, resultRank2);
        }

        [Test]
        public void PoH_GetUnwaveringWillMultiplier_Calculates_Ranks()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("UnwaveringWillUptime", 0.9));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 0);
            var resultDefault = _prayerOfHealing.GetUnwaveringWillMultiplier(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 1);
            var resultRank1 = _prayerOfHealing.GetUnwaveringWillMultiplier(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 2);
            var resultRank2 = _prayerOfHealing.GetUnwaveringWillMultiplier(_gameState);

            // Assert
            Assert.AreEqual(1.0d, resultDefault);
            Assert.AreEqual(0.95499999999999996d, resultRank1);
            Assert.AreEqual(0.91000000000000003d, resultRank2);
        }

        [Test]
        public void PoH_GetUnwaveringWillMultiplier_Throws_No_UnwaveringWillUptime()
        {
            // Arrange

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 1);
            var methodCall = new TestDelegate(
                () => _prayerOfHealing.GetUnwaveringWillMultiplier(_gameState));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("UnwaveringWillUptime needs to be set. (Parameter 'UnwaveringWillUptime')"));
            Assert.That(ex.ParamName, Is.EqualTo("UnwaveringWillUptime"));
        }

        [Test]
        public void Smite_GetHastedCastTime_Calculates_Ranks()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("UnwaveringWillUptime", 0.9));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 0);
            var resultDefault = _smite.GetHastedCastTime(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 1);
            var resultRank1 = _smite.GetHastedCastTime(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 2);
            var resultRank2 = _smite.GetHastedCastTime(_gameState, null);

            // Assert
            Assert.AreEqual(1.4632466861766227d, resultDefault);
            Assert.AreEqual(1.3974005852986746d, resultRank1);
            Assert.AreEqual(1.3315544844207268d, resultRank2);
        }

        [Test]
        public void Smite_GetUnwaveringWillMultiplier_Calculates_Ranks()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("UnwaveringWillUptime", 0.9));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 0);
            var resultDefault = _smite.GetUnwaveringWillMultiplier(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 1);
            var resultRank1 = _smite.GetUnwaveringWillMultiplier(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 2);
            var resultRank2 = _smite.GetUnwaveringWillMultiplier(_gameState);

            // Assert
            Assert.AreEqual(1.0d, resultDefault);
            Assert.AreEqual(0.95499999999999996d, resultRank1);
            Assert.AreEqual(0.91000000000000003d, resultRank2);
        }

        [Test]
        public void Smite_GetUnwaveringWillMultiplier_Throws_No_UnwaveringWillUptime()
        {
            // Arrange

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.UnwaveringWill, 1);
            var methodCall = new TestDelegate(
                () => _smite.GetUnwaveringWillMultiplier(_gameState));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("UnwaveringWillUptime needs to be set. (Parameter 'UnwaveringWillUptime')"));
            Assert.That(ex.ParamName, Is.EqualTo("UnwaveringWillUptime"));
        }
    }
}
