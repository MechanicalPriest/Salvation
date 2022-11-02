using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile.Model;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.HolyPriest.Spells
{
    [TestFixture]
    public class SayYourPrayersTests : BaseTest
    {
        private GameState _gameState;
        private PrayerOfMending _spell;
        IGameStateService _gameStateService;

        [SetUp]
        public void Init()
        {
            _gameStateService = new GameStateService();
            _spell = new PrayerOfMending(_gameStateService, null, null);

            _gameState = GetGameState();
        }

        [Test]
        public void GetSayYourPrayersBounceMultiplier_UnTalented()
        {
            // Arrange

            // Act
            var result = _spell.GetSayYourPrayersBounceMultiplier(_gameState);

            // Assert
            Assert.That(1.0d, Is.EqualTo(result));
        }

        [Test]
        public void GetSayYourPrayersBounceMultiplier_Talented_Ranks()
        {
            // Arrange

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.SayYourPrayers, 0);
            var resultRank1 = _spell.GetSayYourPrayersBounceMultiplier(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.SayYourPrayers, 1);
            var resultRank2 = _spell.GetSayYourPrayersBounceMultiplier(_gameState);

            // Assert
            Assert.That(resultRank1, Is.EqualTo(1.00d));
            Assert.That(resultRank2, Is.EqualTo(1.15d));
        }
    }
}
