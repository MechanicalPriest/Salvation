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
    public class BindingHealsTests : BaseTest
    {
        private GameState _gameState;
        private BindingHeals _bindingHealsSpellService;
        IGameStateService _gameStateService;

        [SetUp]
        public void Init()
        {
            _gameStateService = new GameStateService();
            _bindingHealsSpellService = new BindingHeals(_gameStateService);

            _gameState = GetGameState();
        }

        [Test]
        public void TriggersMastery_Uses_HealSpellData()
        {
            // Arrange

            // Act
            var result = _bindingHealsSpellService.TriggersMastery(_gameState, null);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void GetAverageRawHealing_Calculates_Ranks()
        {
            // Arrange
            var externalHealAmount = 12345.67d;
            var spellData = _gameStateService.GetSpellData(_gameState, Spell.BindingHeals);
            spellData.Overrides[Override.ResultMultiplier] = externalHealAmount;
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("BindingHealsSelfCastPercentage", 0.1));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.BindingHeals, 0);
            var resultRank0 = _bindingHealsSpellService.GetAverageRawHealing(_gameState, spellData);

            _gameStateService.SetTalentRank(_gameState, Spell.BindingHeals, 1);
            var resultRank1 = _bindingHealsSpellService.GetAverageRawHealing(_gameState, spellData);

            // Assert
            Assert.That(resultRank0, Is.EqualTo(0.0d));
            Assert.That(resultRank1, Is.EqualTo(2222.2206000000001d));
        }

        [Test]
        public void GetNumberOfHealingTargets_Calculates()
        {
            // Arrange

            // Act
            var result = _bindingHealsSpellService.GetNumberOfHealingTargets(_gameState, null);

            // Assert
            Assert.That(result, Is.EqualTo(1.0d));
        }

        [Test]
        public void GetAverageRawHealing_Throws_No_ResultMultiplier()
        {
            // Arrange

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.BindingHeals, 1);
            var methodCall = new TestDelegate(
                () => _bindingHealsSpellService.GetAverageRawHealing(_gameState, null));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("SpellData Override.ResultMultiplier must be set. (Parameter 'Override.ResultMultiplier')"));
            Assert.That(ex.ParamName, Is.EqualTo("Override.ResultMultiplier"));
        }

        [Test]
        public void GetAverageRawHealing_Throws_No_BindingHealsSelfCastPercentage()
        {
            // Arrange
            var spellData = _gameStateService.GetSpellData(_gameState, Spell.BindingHeals);
            spellData.Overrides[Override.ResultMultiplier] = 12345.67d;

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.BindingHeals, 1);
            var methodCall = new TestDelegate(
                () => _bindingHealsSpellService.GetAverageRawHealing(_gameState, spellData));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("BindingHealsSelfCastPercentage needs to be set. (Parameter 'BindingHealsSelfCastPercentage')"));
            Assert.That(ex.ParamName, Is.EqualTo("BindingHealsSelfCastPercentage"));
        }

        [Test]
        public void GetActualCastsPerMinute_Throws_No_CastsPerMinute()
        {
            // Arrange

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.PrayerCircle, 1);
            var methodCall = new TestDelegate(
                () => _bindingHealsSpellService.GetActualCastsPerMinute(_gameState, null));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("SpellData Override.CastsPerMinute must be set. (Parameter 'Override.CastsPerMinute')"));
            Assert.That(ex.ParamName, Is.EqualTo("Override.CastsPerMinute"));
        }

        [Test]
        public void GetActualCastsPerMinute_Calculates()
        {
            // Arrange
            var cpm = 12345d;
            var spellData = _gameStateService.GetSpellData(_gameState, Spell.BindingHeals);
            spellData.Overrides[Override.CastsPerMinute] = cpm;

            // Act
            var result = _bindingHealsSpellService.GetActualCastsPerMinute(_gameState, spellData);

            // Assert
            Assert.That(result, Is.EqualTo(cpm));
        }

        [Test]
        public void FH_Calculates_Ranks()
        {
            // Arrange
            var fhSpellService = new FlashHeal(_gameStateService, null, _bindingHealsSpellService);
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("BindingHealsSelfCastPercentage", 0.2));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.BindingHeals, 0);
            var resultRank0 = fhSpellService.GetCastResults(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.BindingHeals, 1);
            var resultRank1 = fhSpellService.GetCastResults(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.BindingHeals, 2);
            var resultRank2 = fhSpellService.GetCastResults(_gameState);

            // Assert
            Assert.That(resultRank0.AdditionalCasts.Count, Is.EqualTo(1));
            Assert.That(resultRank0.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EchoOfLight).Any(), Is.True);

            Assert.That(resultRank1.AdditionalCasts.Count, Is.EqualTo(2));
            Assert.That(resultRank1.RawHealing, Is.EqualTo(7394.684554928077d));
            Assert.That(resultRank1.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EchoOfLight).Any(), Is.True);
            Assert.That(resultRank1.AdditionalCasts.Where(c => c.SpellId == (int)Spell.BindingHeals).Any(), Is.True);

            var bhsCast1 = resultRank1.AdditionalCasts.Where(c => c.SpellId == (int)Spell.BindingHeals).First();
            Assert.That(bhsCast1.AdditionalCasts.Count, Is.EqualTo(1));
            Assert.That(bhsCast1.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EchoOfLight).Any(), Is.True);
            Assert.That(bhsCast1.RawHealing, Is.EqualTo(1183.1495287884925d));

            Assert.That(resultRank2.AdditionalCasts.Count, Is.EqualTo(2));
            Assert.That(resultRank2.RawHealing, Is.EqualTo(7394.684554928077d));
            Assert.That(resultRank2.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EchoOfLight).Any(), Is.True);
            Assert.That(resultRank2.AdditionalCasts.Where(c => c.SpellId == (int)Spell.BindingHeals).Any(), Is.True);

            var bhsCast2 = resultRank2.AdditionalCasts.Where(c => c.SpellId == (int)Spell.BindingHeals).First();
            Assert.That(bhsCast2.AdditionalCasts.Count, Is.EqualTo(1));
            Assert.That(bhsCast2.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EchoOfLight).Any(), Is.True);
            Assert.That(bhsCast2.RawHealing, Is.EqualTo(2366.299057576985d));
        }

        [Test]
        public void Heal_Calculates_Ranks()
        {
            // Arrange
            var healSpellService = new Heal(_gameStateService, null, _bindingHealsSpellService);
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("BindingHealsSelfCastPercentage", 0.2));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.BindingHeals, 0);
            var resultRank0 = healSpellService.GetCastResults(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.BindingHeals, 1);
            var resultRank1 = healSpellService.GetCastResults(_gameState);

            _gameStateService.SetTalentRank(_gameState, Spell.BindingHeals, 2);
            var resultRank2 = healSpellService.GetCastResults(_gameState);

            // Assert
            Assert.That(resultRank0.AdditionalCasts.Count, Is.EqualTo(1));
            Assert.That(resultRank0.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EchoOfLight).Any(), Is.True);

            Assert.That(resultRank1.AdditionalCasts.Count, Is.EqualTo(2));
            Assert.That(resultRank1.RawHealing, Is.EqualTo(10745.970166028488d));
            Assert.That(resultRank1.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EchoOfLight).Any(), Is.True);
            Assert.That(resultRank1.AdditionalCasts.Where(c => c.SpellId == (int)Spell.BindingHeals).Any(), Is.True);

            var bhsCast1 = resultRank1.AdditionalCasts.Where(c => c.SpellId == (int)Spell.BindingHeals).First();
            Assert.That(bhsCast1.AdditionalCasts.Count, Is.EqualTo(1));
            Assert.That(bhsCast1.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EchoOfLight).Any(), Is.True);
            Assert.That(bhsCast1.RawHealing, Is.EqualTo(1719.3552265645585d));

            Assert.That(resultRank2.AdditionalCasts.Count, Is.EqualTo(2));
            Assert.That(resultRank2.RawHealing, Is.EqualTo(10745.970166028488d));
            Assert.That(resultRank2.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EchoOfLight).Any(), Is.True);
            Assert.That(resultRank2.AdditionalCasts.Where(c => c.SpellId == (int)Spell.BindingHeals).Any(), Is.True);

            var bhsCast2 = resultRank2.AdditionalCasts.Where(c => c.SpellId == (int)Spell.BindingHeals).First();
            Assert.That(bhsCast2.AdditionalCasts.Count, Is.EqualTo(1));
            Assert.That(bhsCast2.AdditionalCasts.Where(c => c.SpellId == (int)Spell.EchoOfLight).Any(), Is.True);
            Assert.That(bhsCast2.RawHealing, Is.EqualTo(3438.710453129117d));
        }
    }
}
