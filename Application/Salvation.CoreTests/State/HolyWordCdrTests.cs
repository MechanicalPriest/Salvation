using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;
using System.Collections;

namespace Salvation.CoreTests.State
{
    [TestFixture]
    public class HolyWordCdrTests : BaseTest
    {
        IGameStateService _gameStateService;
        GameState _state;

        [SetUp]
        public void Init()
        {
            _state = GetGameState();
            _gameStateService = new GameStateService();
        }

        [TestCaseSource(typeof(HolyWordTestSpells), "BaseValueTests")]
        public double HWCDR_Base_Values(Spell spell)
        {
            // Arrange


            // Act


            // Assert
            return _gameStateService.GetTotalHolyWordCooldownReduction(_state, spell);
        }

        [TestCaseSource(typeof(HolyWordTestSpells), "LotnValueTests")]
        public double HWCDR_LotN_Values(Spell spell)
        {
            // Arrange
            _state.Profile.Talents.Add(Talent.LightOfTheNaaru);

            // Act


            // Assert
            return Math.Round(_gameStateService.GetTotalHolyWordCooldownReduction(_state, spell), 10);
        }

        [TestCaseSource(typeof(HolyWordTestSpells), "LotnHoValueTests")]
        public double HWCDR_LotN_HO_Values(Spell spell)
        {
            // Arrange
            _state.Profile.Talents.Add(Talent.LightOfTheNaaru);
            _state.Profile.Conduits.Add(Conduit.HolyOration, 0);

            // Act


            // Assert
            return Math.Round(_gameStateService.GetTotalHolyWordCooldownReduction(_state, spell), 10);
        }

        [TestCaseSource(typeof(HolyWordTestSpells), "HoValueTests")]
        public double HWCDR_HO_Values(Spell spell)
        {
            // Arrange
            _state.Profile.Conduits.Add(Conduit.HolyOration, 0);

            // Act


            // Assert
            return Math.Round(_gameStateService.GetTotalHolyWordCooldownReduction(_state, spell), 10);
        }

        [TestCaseSource(typeof(HolyWordTestSpells), nameof(HolyWordTestSpells.ApothValueTests))]
        public double HWCDR_Apoth_Values(Spell spell)
        {
            // Arrange
            _state.Profile.Talents.Add(Talent.Apotheosis);

            // Act


            // Assert
            return Math.Round(_gameStateService.GetTotalHolyWordCooldownReduction(_state, spell, true), 10);
        }

        [TestCaseSource(typeof(HolyWordTestSpells), nameof(HolyWordTestSpells.ApothHoValueTests))]
        public double HWCDR_Apoth_HO_Values(Spell spell)
        {
            // Arrange
            _state.Profile.Talents.Add(Talent.Apotheosis);
            _state.Profile.Conduits.Add(Conduit.HolyOration, 0);

            // Act


            // Assert
            return Math.Round(_gameStateService.GetTotalHolyWordCooldownReduction(_state, spell, true), 10);
        }

        [Test]
        public void HWCDR_Invalid_Spell()
        {
            // Arrange

            // Act
            var result = _gameStateService.GetTotalHolyWordCooldownReduction(_state, Spell.DivineStar, true);

            // Assert
            Assert.AreEqual(0, result);
        }
    }

    public class HolyWordTestSpells
    {
        public static IEnumerable BaseValueTests
        {
            get
            {
                yield return new TestCaseData(Spell.FlashHeal).Returns(6d);
                yield return new TestCaseData(Spell.Heal).Returns(6d);
                yield return new TestCaseData(Spell.PrayerOfHealing).Returns(6d);
                yield return new TestCaseData(Spell.BindingHeal).Returns(3d);
                yield return new TestCaseData(Spell.Renew).Returns(2d);
                yield return new TestCaseData(Spell.CircleOfHealing).Returns(4d);
                yield return new TestCaseData(Spell.PrayerOfMending).Returns(4d);
                yield return new TestCaseData(Spell.HolyWordSerenity).Returns(30d);
                yield return new TestCaseData(Spell.HolyWordSanctify).Returns(30d);
            }
        }
        public static IEnumerable LotnValueTests
        {
            get
            {
                yield return new TestCaseData(Spell.FlashHeal).Returns(8d);
                yield return new TestCaseData(Spell.Heal).Returns(8d);
                yield return new TestCaseData(Spell.PrayerOfHealing).Returns(8d);
                yield return new TestCaseData(Spell.BindingHeal).Returns(4d);
                yield return new TestCaseData(Spell.Renew).Returns(2.6666666667d);
                yield return new TestCaseData(Spell.CircleOfHealing).Returns(5.3333333333d);
                yield return new TestCaseData(Spell.PrayerOfMending).Returns(5.3333333333d);
                yield return new TestCaseData(Spell.HolyWordSerenity).Returns(30d);
                yield return new TestCaseData(Spell.HolyWordSanctify).Returns(30d);
            }
        }
        public static IEnumerable LotnHoValueTests
        {
            get
            {
                yield return new TestCaseData(Spell.FlashHeal).Returns(8.36d);
                yield return new TestCaseData(Spell.Heal).Returns(8.36d);
                yield return new TestCaseData(Spell.PrayerOfHealing).Returns(8.36d);
                yield return new TestCaseData(Spell.BindingHeal).Returns(4.18d);
                yield return new TestCaseData(Spell.Renew).Returns(2.7866666667d);
                yield return new TestCaseData(Spell.CircleOfHealing).Returns(5.5733333333d);
                yield return new TestCaseData(Spell.PrayerOfMending).Returns(5.5733333333d);
                yield return new TestCaseData(Spell.HolyWordSerenity).Returns(31.8d);
                yield return new TestCaseData(Spell.HolyWordSanctify).Returns(31.8d);
            }
        }
        public static IEnumerable HoValueTests
        {
            get
            {
                yield return new TestCaseData(Spell.FlashHeal).Returns(6.36d);
                yield return new TestCaseData(Spell.Heal).Returns(6.36d);
                yield return new TestCaseData(Spell.PrayerOfHealing).Returns(6.36d);
                yield return new TestCaseData(Spell.BindingHeal).Returns(3.18d);
                yield return new TestCaseData(Spell.Renew).Returns(2.12d);
                yield return new TestCaseData(Spell.CircleOfHealing).Returns(4.24d);
                yield return new TestCaseData(Spell.PrayerOfMending).Returns(4.24d);
                yield return new TestCaseData(Spell.HolyWordSerenity).Returns(31.8d);
                yield return new TestCaseData(Spell.HolyWordSanctify).Returns(31.8d);
            }
        }
        public static IEnumerable ApothValueTests
        {
            get
            {
                yield return new TestCaseData(Spell.FlashHeal).Returns(24d);
                yield return new TestCaseData(Spell.Heal).Returns(24d);
                yield return new TestCaseData(Spell.PrayerOfHealing).Returns(24d);
                yield return new TestCaseData(Spell.BindingHeal).Returns(12d);
                yield return new TestCaseData(Spell.Renew).Returns(8d);
                yield return new TestCaseData(Spell.CircleOfHealing).Returns(16d);
                yield return new TestCaseData(Spell.PrayerOfMending).Returns(16d);
                yield return new TestCaseData(Spell.HolyWordSerenity).Returns(30d);
                yield return new TestCaseData(Spell.HolyWordSanctify).Returns(30d);
            }
        }
        public static IEnumerable ApothHoValueTests
        {
            get
            {
                yield return new TestCaseData(Spell.FlashHeal).Returns(24.36d);
                yield return new TestCaseData(Spell.Heal).Returns(24.36d);
                yield return new TestCaseData(Spell.PrayerOfHealing).Returns(24.36d);
                yield return new TestCaseData(Spell.BindingHeal).Returns(12.18d);
                yield return new TestCaseData(Spell.Renew).Returns(8.12d);
                yield return new TestCaseData(Spell.CircleOfHealing).Returns(16.24d);
                yield return new TestCaseData(Spell.PrayerOfMending).Returns(16.24d);
                yield return new TestCaseData(Spell.HolyWordSerenity).Returns(31.8d);
                yield return new TestCaseData(Spell.HolyWordSanctify).Returns(31.8d);
            }
        }
    }
}
