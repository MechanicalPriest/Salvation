using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.HolyPriest.Spells
{
    [TestFixture]
    class DivineImageTests : BaseTest
    {
        private ISpellService _spell;
        private GameState _gameState;

        [SetUp]
        public void Init()
        {
            IGameStateService _gameStateService = new GameStateService();

            var diHealingLight = new DivineImageHealingLight(_gameStateService,
                new FlashHeal(_gameStateService, null, null),
                new Heal(_gameStateService, null, null),
                new HolyWordSerenity(_gameStateService,
                    new FlashHeal(_gameStateService, null, null),
                    new Heal(_gameStateService, null, null),
                    new PrayerOfMending(_gameStateService, null)));

            var diTranquilLight = new DivineImageTranquilLight(_gameStateService,
                new Renew(_gameStateService, null));

            var diDazzlingLights = new DivineImageDazzlingLights(_gameStateService,
                new PrayerOfHealing(_gameStateService, new Renew(_gameStateService, null)),
                new HolyWordSanctify(_gameStateService,
                    new PrayerOfHealing(_gameStateService, new Renew(_gameStateService, null)),
                    new Renew(_gameStateService, null),
                    new CircleOfHealing(_gameStateService)),
                new DivineStar(_gameStateService),
                new Halo(_gameStateService),
                new DivineHymn(_gameStateService),
                new CircleOfHealing(_gameStateService));

            var diBlessedLight = new DivineImageBlessedLight(_gameStateService,
                new PrayerOfMending(_gameStateService, null));

            _spell = new DivineImage(_gameStateService,
                new HolyWordSerenity(_gameStateService,
                    new FlashHeal(_gameStateService, null, null),
                    new Heal(_gameStateService, null, null),
                    new PrayerOfMending(_gameStateService, null)),
                new HolyWordSanctify(_gameStateService,
                    new PrayerOfHealing(_gameStateService, new Renew(_gameStateService, null)),
                    new Renew(_gameStateService, null),
                    new CircleOfHealing(_gameStateService)),
                new HolyWordChastise(_gameStateService,
                    new Smite(_gameStateService),
                    new HolyFire(_gameStateService)),
                diHealingLight,
                diTranquilLight,
                diDazzlingLights,
                diBlessedLight);
            _gameState = GetGameState();
        }

        [Test]
        public void GetDuration()
        {
            // Arrange

            // Act
            var value = _spell.GetDuration(_gameState, null);

            // Assert
            Assert.AreEqual(15, value);
        }

        [Test]
        public void GetActualCastsPerMinute()
        {
            // Arrange

            // Act
            var value = _spell.GetActualCastsPerMinute(_gameState, null);

            // Assert
            Assert.AreEqual(1.1664471613238547d, value);
        }

        [Test]
        public void GetUptime()
        {
            // Arrange

            // Act
            var value = _spell.GetUptime(_gameState, null);

            // Assert
            Assert.AreEqual(0.29161179033096368d, value);
        }
    }
}
