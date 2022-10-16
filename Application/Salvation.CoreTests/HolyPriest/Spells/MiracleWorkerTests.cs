using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.HolyPriest.Spells
{
    [TestFixture]
    public class MiracleWorkerTests : BaseTest
    {
        GameState _gameState;
        GameStateService _gameStateService;
        HolyWordSerenity _serenity;
        HolyWordSanctify _sanctify;

        [SetUp]
        public void InitOnce()
        {
            _gameState = GetGameState();
            _gameStateService = new GameStateService();

            _serenity = new HolyWordSerenity(_gameStateService,
                new FlashHeal(_gameStateService, null, null),
                new Heal(_gameStateService, null, null),
                new PrayerOfMending(_gameStateService));

            _sanctify = new HolyWordSanctify(_gameStateService,
                new PrayerOfHealing(_gameStateService),
                new Renew(_gameStateService),
                new CircleOfHealing(_gameStateService));
        }

        [Test]
        public void Serenity_GetMaximumCastsPerMinute_Calculates_Ranks()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();

            // Act
            gameStateService.SetTalentRank(_gameState, Spell.MiracleWorker, 0);
            var resultDefault = _serenity.GetMaximumCastsPerMinute(_gameState, null);

            gameStateService.SetTalentRank(_gameState, Spell.MiracleWorker, 1);
            var resultRank1 = _serenity.GetMaximumCastsPerMinute(_gameState, null);

            // Assert
            Assert.That(resultDefault, Is.EqualTo(1.5617546259653283d));
            Assert.That(resultRank1, Is.EqualTo(1.7128881272247742d));
        }

        [Test]
        public void Serenity_GetMiracleWorkerCharges_Calculates_Ranks()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();

            // Act
            gameStateService.SetTalentRank(_gameState, Spell.MiracleWorker, 0);
            var resultDefault = _serenity.GetMiracleWorkerCharges(_gameState, null);

            gameStateService.SetTalentRank(_gameState, Spell.MiracleWorker, 1);
            var resultRank1 = _serenity.GetMiracleWorkerCharges(_gameState, null);

            // Assert
            Assert.That(resultDefault, Is.EqualTo(0.0d));
            Assert.That(resultRank1, Is.EqualTo(1.0d));
        }

        [Test]
        public void Sanctify_GetMaximumCastsPerMinute_Calculates_Ranks()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();

            // Act
            gameStateService.SetTalentRank(_gameState, Spell.MiracleWorker, 0);
            var resultDefault = _sanctify.GetMaximumCastsPerMinute(_gameState, null);

            gameStateService.SetTalentRank(_gameState, Spell.MiracleWorker, 1);
            var resultRank1 = _sanctify.GetMaximumCastsPerMinute(_gameState, null);

            // Assert
            Assert.That(resultDefault, Is.EqualTo(2.1022718247888577d));
            Assert.That(resultRank1, Is.EqualTo(2.2534053260483038d));
        }

        [Test]
        public void Sanctify_GetMiracleWorkerCharges_Calculates_Ranks()
        {
            // Arrange
            IGameStateService gameStateService = new GameStateService();

            // Act
            gameStateService.SetTalentRank(_gameState, Spell.MiracleWorker, 0);
            var resultDefault = _sanctify.GetMiracleWorkerCharges(_gameState, null);

            gameStateService.SetTalentRank(_gameState, Spell.MiracleWorker, 1);
            var resultRank1 = _sanctify.GetMiracleWorkerCharges(_gameState, null);

            // Assert
            Assert.That(resultDefault, Is.EqualTo(0.0d));
            Assert.That(resultRank1, Is.EqualTo(1.0d));
        }
    }
}
