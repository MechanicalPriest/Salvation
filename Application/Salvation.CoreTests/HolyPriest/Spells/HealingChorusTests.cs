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
    public class HealingChorusTests : BaseTest
    {
        private GameState _gameState;
        private CircleOfHealing _circleOfHealingSpellService;
        IGameStateService _gameStateService;

        [SetUp]
        public void Init()
        {
            _gameStateService = new GameStateService();
            _circleOfHealingSpellService = new CircleOfHealing(_gameStateService);

            _gameState = GetGameState();

            _gameState.RegisteredSpells.Add(
                new RegisteredSpell()
                {
                    Spell = Spell.Renew,
                    SpellData = _gameStateService.GetSpellData(_gameState, Spell.Renew),
                    SpellService = new Renew(_gameStateService)
                });

            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("HealingChorusStacksWastedPerMinute", 3.0));
        }

        [Test]
        public void GetHealingChorusModifier_Calculates_Ranks()
        {
            // Arrange

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.HealingChorus, 0);
            var result = _circleOfHealingSpellService.GetHealingChorusModifier(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.HealingChorus, 1);
            var resultRank1 = _circleOfHealingSpellService.GetHealingChorusModifier(_gameState);

            // Assert
            Assert.That(result, Is.EqualTo(1.0d));
            Assert.That(resultRank1, Is.EqualTo(1.0202451077834325d));
        }

        [Test]
        public void GetAverageRawHealing_Calculates_Ranks()
        {
            // Arrange

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.HealingChorus, 0);
            var result = _circleOfHealingSpellService.GetAverageRawHealing(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.HealingChorus, 1);
            var resultRank1 = _circleOfHealingSpellService.GetAverageRawHealing(_gameState);

            // Assert
            Assert.That(result, Is.EqualTo(19124.184193779514d));
            Assert.That(resultRank1, Is.EqualTo(19511.355364052793d));
        }

        [Test]
        public void GetRenewUptime_Throws_No_GroupSize()
        {
            // Arrange
            _gameState.Profile.PlaystyleEntries.Remove(
                _gameState.Profile.PlaystyleEntries.Where(p => p.Name == "HealingChorusStacksWastedPerMinute").First());
            _gameStateService.SetTalentRank(_gameState, Spell.HealingChorus, 1);

            // Act
            var methodCall = new TestDelegate(
                () => _circleOfHealingSpellService.GetHealingChorusModifier(_gameState, null));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("HealingChorusStacksWastedPerMinute needs to be set. (Parameter 'HealingChorusStacksWastedPerMinute')"));
            Assert.That(ex.ParamName, Is.EqualTo("HealingChorusStacksWastedPerMinute"));
        }
    }
}
