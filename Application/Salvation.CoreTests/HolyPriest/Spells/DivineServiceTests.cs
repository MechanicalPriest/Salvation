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
    public class DivineServiceTests : BaseTest
    {
        private GameState _gameState;
        private PrayerOfMending _spell;
        IGameStateService _gameStateService;

        [SetUp]
        public void Init()
        {
            _gameStateService = new GameStateService();
            _spell = new PrayerOfMending(_gameStateService);

            _gameState = GetGameState();
        }

        [Test]
        public void GetDivineServiceStackMultiplier_UnTalented()
        {
            // Arrange

            // Act
            var result = _spell.GetDivineServiceStackMultiplier(_gameState);

            // Assert
            Assert.That(0.0d, Is.EqualTo(result));
        }

        [Test]
        public void GetSayYourPrayersBounceMultiplier_Talented_Ranks()
        {
            // Arrange

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.DivineService, 0);
            var resultRank1 = _spell.GetDivineServiceStackMultiplier(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.DivineService, 1);
            var resultRank2 = _spell.GetDivineServiceStackMultiplier(_gameState);

            // Assert
            Assert.That(resultRank1, Is.EqualTo(0.00d));
            Assert.That(resultRank2, Is.EqualTo(0.04d));
        }

        [Test]
        public void GetAverageRawHealing_Talented_Ranks()
        {
            // Arrange

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.DivineService, 0);
            var resultRank0 = _spell.GetAverageRawHealing(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.DivineService, 1);
            var resultRank1 = _spell.GetAverageRawHealing(_gameState);

            // Assert
            Assert.That(resultRank0, Is.EqualTo(11110.240341148095d));
            Assert.That(resultRank1, Is.EqualTo(12265.705336627499d));
        }
    }
}
