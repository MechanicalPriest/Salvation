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
    public class RenewedFaithTests : BaseTest
    {
        private GameState _gameState;
        private RenewedFaith _renewedFaithSpellService;
        IGameStateService _gameStateService;

        [SetUp]
        public void Init()
        {
            _gameStateService = new GameStateService();
            _renewedFaithSpellService = new RenewedFaith(_gameStateService);

            _gameState = GetGameState();

            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("GroupSize", 20));

            _gameState.RegisteredSpells.Add(
                new RegisteredSpell()
                {
                    Spell = Spell.Renew,
                    SpellData = _gameStateService.GetSpellData(_gameState, Spell.Renew),
                    SpellService = new Renew(_gameStateService)
                });
            _gameState.RegisteredSpells.Add(
                new RegisteredSpell()
                {
                    Spell = Spell.HolyWordSalvation,
                    SpellData = _gameStateService.GetSpellData(_gameState, Spell.HolyWordSalvation),
                    SpellService = new HolyWordSalvation(_gameStateService,
                        new HolyWordSerenity(_gameStateService,
                            new FlashHeal(_gameStateService, null, null),
                            new Heal(_gameStateService, null, null),
                            new PrayerOfMending(_gameStateService)),
                        new HolyWordSanctify(_gameStateService,
                            new PrayerOfHealing(_gameStateService),
                            new Renew(_gameStateService),
                            new CircleOfHealing(_gameStateService)),
                        new Renew(_gameStateService),
                        new PrayerOfMending(_gameStateService))
                });
        }

        [Test]
        public void GetUptime()
        {
            // Arrange

            // Act
            var result = _renewedFaithSpellService.GetUptime(_gameState, null);

            // Assert
            Assert.That(result, Is.EqualTo(0.094340418589652836d));
        }

        [Test]
        public void GetUptime_Throws_No_GroupSize()
        {
            // Arrange
            _gameState.Profile.PlaystyleEntries.Remove(
                _gameState.Profile.PlaystyleEntries.Where(p => p.Name == "GroupSize").First());

            // Act
            var methodCall = new TestDelegate(
                () => _renewedFaithSpellService.GetUptime(_gameState, null));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("GroupSize needs to be set. (Parameter 'GroupSize')"));
            Assert.That(ex.ParamName, Is.EqualTo("GroupSize"));
        }

        [Test]
        public void GetAverageHealingMultiplier()
        {
            // Arrange

            // Act
            var result = _renewedFaithSpellService.GetAverageHealingMultiplier(_gameState, null);

            // Assert
            Assert.That(result, Is.EqualTo(1.0075472334871722d));
        }

        [Test]
        public void GetAverageHealingMultiplier_Throws_No_GroupSize()
        {
            // Arrange
            _gameState.Profile.PlaystyleEntries.Remove(
                _gameState.Profile.PlaystyleEntries.Where(p => p.Name == "GroupSize").First());

            // Act
            var methodCall = new TestDelegate(
                () => _renewedFaithSpellService.GetAverageHealingMultiplier(_gameState, null));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("GroupSize needs to be set. (Parameter 'GroupSize')"));
            Assert.That(ex.ParamName, Is.EqualTo("GroupSize"));
        }
    }
}
