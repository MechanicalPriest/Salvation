using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Salvation.CoreTests.State
{
    [TestFixture]
    public class HolyWordCdrTests
    {
        IConstantsService _constantsService;
        IGameStateService _gameStateService;
        GameState _state;

        [OneTimeSetUp]
        public void InitOnce()
        {
            _constantsService = new ConstantsService();

        }

        [SetUp]
        public void Init()
        {
            var constants = _constantsService.LoadConstantsFromFile();

            PlayerProfile profile = new ProfileGenerationService()
                .GetDefaultProfile(Core.Constants.Data.Spec.HolyPriest);

            _state = new GameState(profile, constants);

            _gameStateService = new GameStateService();
        }

        [TestCaseSource(typeof(HolyWordTestSpells), "BaseValueTests")]
        public decimal HWCDR_Base_Values(Spell spell)
        {
            // Arrange


            // Act


            // Assert
            return _gameStateService.GetTotalHolyWordCooldownReduction(_state, spell);
        }

        [TestCaseSource(typeof(HolyWordTestSpells), "LotnValueTests")]
        public decimal HWCDR_LotN_Values(Spell spell)
        {
            // Arrange
            _state.Profile.Talents.Add(Talent.LightOfTheNaaru);

            // Act


            // Assert
            return Math.Round(_gameStateService.GetTotalHolyWordCooldownReduction(_state, spell), 10);
        }

        [TestCaseSource(typeof(HolyWordTestSpells), "LotnHoValueTests")]
        public decimal HWCDR_LotN_HO_Values(Spell spell)
        {
            // Arrange
            _state.Profile.Talents.Add(Talent.LightOfTheNaaru);
            _state.Profile.Conduits.Add(Conduit.HolyOration, 0);

            // Act


            // Assert
            return Math.Round(_gameStateService.GetTotalHolyWordCooldownReduction(_state, spell), 10);
        }

        [TestCaseSource(typeof(HolyWordTestSpells), "HoValueTests")]
        public decimal HWCDR_HO_Values(Spell spell)
        {
            // Arrange
            _state.Profile.Conduits.Add(Conduit.HolyOration, 0);

            // Act


            // Assert
            return Math.Round(_gameStateService.GetTotalHolyWordCooldownReduction(_state, spell), 10);
        }

        [TestCaseSource(typeof(HolyWordTestSpells), nameof(HolyWordTestSpells.ApothValueTests))]
        public decimal HWCDR_Apoth_Values(Spell spell)
        {
            // Arrange
            _state.Profile.Talents.Add(Talent.Apotheosis);

            // Act


            // Assert
            return Math.Round(_gameStateService.GetTotalHolyWordCooldownReduction(_state, spell, true), 10);
        }

        [TestCaseSource(typeof(HolyWordTestSpells), nameof(HolyWordTestSpells.ApothHoValueTests))]
        public decimal HWCDR_Apoth_HO_Values(Spell spell)
        {
            // Arrange
            _state.Profile.Talents.Add(Talent.Apotheosis);
            _state.Profile.Conduits.Add(Conduit.HolyOration, 0);

            // Act


            // Assert
            return Math.Round(_gameStateService.GetTotalHolyWordCooldownReduction(_state, spell, true), 10);
        }
    }

    public class HolyWordTestSpells
    {
        public static IEnumerable BaseValueTests
        {
            get
            {
                yield return new TestCaseData(Spell.FlashHeal).Returns(6m);
                yield return new TestCaseData(Spell.Heal).Returns(6m);
                yield return new TestCaseData(Spell.PrayerOfHealing).Returns(6m);
                yield return new TestCaseData(Spell.BindingHeal).Returns(3m);
                yield return new TestCaseData(Spell.Renew).Returns(2m);
                yield return new TestCaseData(Spell.CircleOfHealing).Returns(4m);
                yield return new TestCaseData(Spell.PrayerOfMending).Returns(4m);
                yield return new TestCaseData(Spell.HolyWordSerenity).Returns(30m);
                yield return new TestCaseData(Spell.HolyWordSanctify).Returns(30m);
            }
        }
        public static IEnumerable LotnValueTests
        {
            get
            {
                yield return new TestCaseData(Spell.FlashHeal).Returns(8m);
                yield return new TestCaseData(Spell.Heal).Returns(8m);
                yield return new TestCaseData(Spell.PrayerOfHealing).Returns(8m);
                yield return new TestCaseData(Spell.BindingHeal).Returns(4m);
                yield return new TestCaseData(Spell.Renew).Returns(2.6666666667m);
                yield return new TestCaseData(Spell.CircleOfHealing).Returns(5.3333333333m);
                yield return new TestCaseData(Spell.PrayerOfMending).Returns(5.3333333333m);
                yield return new TestCaseData(Spell.HolyWordSerenity).Returns(30m);
                yield return new TestCaseData(Spell.HolyWordSanctify).Returns(30m);
            }
        }
        public static IEnumerable LotnHoValueTests
        {
            get
            {
                yield return new TestCaseData(Spell.FlashHeal).Returns(8.3m);
                yield return new TestCaseData(Spell.Heal).Returns(8.3m);
                yield return new TestCaseData(Spell.PrayerOfHealing).Returns(8.3m);
                yield return new TestCaseData(Spell.BindingHeal).Returns(4.15m);
                yield return new TestCaseData(Spell.Renew).Returns(2.7666666667m);
                yield return new TestCaseData(Spell.CircleOfHealing).Returns(5.5333333333m);
                yield return new TestCaseData(Spell.PrayerOfMending).Returns(5.5333333333m);
                yield return new TestCaseData(Spell.HolyWordSerenity).Returns(31.5m);
                yield return new TestCaseData(Spell.HolyWordSanctify).Returns(31.5m);
            }
        }
        public static IEnumerable HoValueTests
        {
            get
            {
                yield return new TestCaseData(Spell.FlashHeal).Returns(6.3m);
                yield return new TestCaseData(Spell.Heal).Returns(6.3m);
                yield return new TestCaseData(Spell.PrayerOfHealing).Returns(6.3m);
                yield return new TestCaseData(Spell.BindingHeal).Returns(3.15m);
                yield return new TestCaseData(Spell.Renew).Returns(2.1m);
                yield return new TestCaseData(Spell.CircleOfHealing).Returns(4.2m);
                yield return new TestCaseData(Spell.PrayerOfMending).Returns(4.2m);
                yield return new TestCaseData(Spell.HolyWordSerenity).Returns(31.5m);
                yield return new TestCaseData(Spell.HolyWordSanctify).Returns(31.5m);
            }
        }
        public static IEnumerable ApothValueTests
        {
            get
            {
                yield return new TestCaseData(Spell.FlashHeal).Returns(24m);
                yield return new TestCaseData(Spell.Heal).Returns(24m);
                yield return new TestCaseData(Spell.PrayerOfHealing).Returns(24m);
                yield return new TestCaseData(Spell.BindingHeal).Returns(12m);
                yield return new TestCaseData(Spell.Renew).Returns(8m);
                yield return new TestCaseData(Spell.CircleOfHealing).Returns(16m);
                yield return new TestCaseData(Spell.PrayerOfMending).Returns(16m);
                yield return new TestCaseData(Spell.HolyWordSerenity).Returns(30m);
                yield return new TestCaseData(Spell.HolyWordSanctify).Returns(30m);
            }
        }
        public static IEnumerable ApothHoValueTests
        {
            get
            {
                yield return new TestCaseData(Spell.FlashHeal).Returns(24.3m);
                yield return new TestCaseData(Spell.Heal).Returns(24.3m);
                yield return new TestCaseData(Spell.PrayerOfHealing).Returns(24.3m);
                yield return new TestCaseData(Spell.BindingHeal).Returns(12.15m);
                yield return new TestCaseData(Spell.Renew).Returns(8.1m);
                yield return new TestCaseData(Spell.CircleOfHealing).Returns(16.2m);
                yield return new TestCaseData(Spell.PrayerOfMending).Returns(16.2m);
                yield return new TestCaseData(Spell.HolyWordSerenity).Returns(31.5m);
                yield return new TestCaseData(Spell.HolyWordSanctify).Returns(31.5m);
            }
        }
    }
}
