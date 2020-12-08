using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common.Items;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.Common.Items
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
                new FlashHeal(_gameStateService),
                new Heal(_gameStateService),
                new HolyWordSerenity(_gameStateService,
                    new FlashHeal(_gameStateService), new Heal(_gameStateService),
                    new BindingHeal(_gameStateService), new PrayerOfMending(_gameStateService)),
                new BindingHeal(_gameStateService));

            var diTranquilLight = new DivineImageTranquilLight(_gameStateService,
                new Renew(_gameStateService));

            var diDazzlingLights = new DivineImageDazzlingLights(_gameStateService,
                new PrayerOfHealing(_gameStateService),
                new HolyWordSanctify(_gameStateService,
                    new PrayerOfHealing(_gameStateService),
                    new Renew(_gameStateService),
                    new BindingHeal(_gameStateService),
                    new CircleOfHealing(_gameStateService)),
                new DivineStar(_gameStateService),
                new Halo(_gameStateService),
                new DivineHymn(_gameStateService),
                new CircleOfHealing(_gameStateService));

            var diBlessedLight = new DivineImageBlessedLight(_gameStateService,
                new PrayerOfMending(_gameStateService));

            _spell = new DivineImage(_gameStateService,
                new HolyWordSerenity(_gameStateService,
                    new FlashHeal(_gameStateService),
                    new Heal(_gameStateService),
                    new BindingHeal(_gameStateService),
                    new PrayerOfMending(_gameStateService)),
                new HolyWordSanctify(_gameStateService,
                    new PrayerOfHealing(_gameStateService),
                    new Renew(_gameStateService),
                    new BindingHeal(_gameStateService),
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
            Assert.AreEqual(1.03357750470943d, value);
        }

        [Test]
        public void GetUptime()
        {
            // Arrange

            // Act
            var value = _spell.GetUptime(_gameState, null);

            // Assert
            Assert.AreEqual(0.2583943761773575d, value);
        }
    }
}
