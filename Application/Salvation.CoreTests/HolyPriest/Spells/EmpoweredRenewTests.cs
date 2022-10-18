using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile.Model;
using Salvation.Core.State;
using System;
using System.Linq;

namespace Salvation.CoreTests.HolyPriest.Spells
{
    [TestFixture]
    public class EmpoweredRenewTests : BaseTest
    {
        private GameState _gameState;
        private EmpoweredRenew _empoweredRenewSpellService;
        IGameStateService _gameStateService;

        [SetUp]
        public void Init()
        {
            _gameStateService = new GameStateService();
            _empoweredRenewSpellService = new EmpoweredRenew(_gameStateService);

            _gameState = GetGameState();
        }

        [Test]
        public void TriggersMastery_Uses_HealSpellData()
        {
            // Arrange

            // Act
            var result = _empoweredRenewSpellService.TriggersMastery(_gameState, null);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void GetAverageRawHealing_Calculates_Ranks()
        {
            // Arrange
            var externalHealAmount = 12345.67d;
            var spellData = _gameStateService.GetSpellData(_gameState, Spell.EmpoweredRenew);
            spellData.Overrides[Override.ResultMultiplier] = externalHealAmount;

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.EmpoweredRenew, 0);
            var resultRank0 = _empoweredRenewSpellService.GetAverageRawHealing(_gameState, spellData);

            _gameStateService.SetTalentRank(_gameState, Spell.EmpoweredRenew, 1);
            var resultRank1 = _empoweredRenewSpellService.GetAverageRawHealing(_gameState, spellData);

            // Assert
            Assert.That(resultRank0, Is.EqualTo(0.0d));
            Assert.That(resultRank1, Is.EqualTo(1283.4678977560975d));
        }

        [Test]
        public void GetNumberOfHealingTargets_Calculates()
        {
            // Arrange

            // Act
            var result = _empoweredRenewSpellService.GetNumberOfHealingTargets(_gameState, null);

            // Assert
            Assert.That(result, Is.EqualTo(1.0d));
        }

        [Test]
        public void GetAverageRawHealing_Throws_No_ResultMultiplier()
        {
            // Arrange

            // Act
            var methodCall = new TestDelegate(
                () => _empoweredRenewSpellService.GetAverageRawHealing(_gameState, null));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("SpellData Override.ResultMultiplier must be set. (Parameter 'Override.ResultMultiplier')"));
            Assert.That(ex.ParamName, Is.EqualTo("Override.ResultMultiplier"));
        }

        [Test]
        public void GetActualCastsPerMinute_Calculates()
        {
            // Arrange
            var cpm = 12345d;
            var spellData = _gameStateService.GetSpellData(_gameState, Spell.EmpoweredRenew);
            spellData.Overrides[Override.CastsPerMinute] = cpm;

            // Act
            var result = _empoweredRenewSpellService.GetActualCastsPerMinute(_gameState, spellData);

            // Assert
            Assert.That(result, Is.EqualTo(cpm));
        }

        [Test]
        public void Renew_Calculates_Ranks()
        {
            // Arrange
            var renewSpellService = new Renew(_gameStateService, _empoweredRenewSpellService);

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.EmpoweredRenew, 0);
            var resultRank0 = renewSpellService.GetCastResults(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.EmpoweredRenew, 1);
            var resultRank1 = renewSpellService.GetCastResults(_gameState);

            // Assert
            Assert.That(resultRank0.AdditionalCasts.Count, Is.EqualTo(0));

            Assert.That(resultRank1.AdditionalCasts.Count, Is.EqualTo(1));
            Assert.That(resultRank1.RawHealing, Is.EqualTo(7140.3811173798704d));
            Assert.That(resultRank1.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EmpoweredRenew).Any(), Is.True);

            var empRenewCast1 = resultRank1.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EmpoweredRenew).First();
            Assert.That(empRenewCast1.AdditionalCasts.Count, Is.EqualTo(1));
            Assert.That(empRenewCast1.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EchoOfLight).Any(), Is.True);
            Assert.That(empRenewCast1.RawHealing, Is.EqualTo(742.32098718829172d));
        }
    }
}
