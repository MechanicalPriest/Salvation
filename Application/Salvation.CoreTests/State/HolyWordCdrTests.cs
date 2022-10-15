using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Profile;
using Salvation.Core.Profile.Model;
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

        [TestCaseSource(typeof(HolyWordTestSpells), nameof(HolyWordTestSpells.BaseValueTests))]
        public double HWCDR_Base_Values(Spell spell)
        {
            // Arrange

            // Act


            // Assert
            return _gameStateService.GetTotalHolyWordCooldownReduction(_state, spell);
        }

        [TestCaseSource(typeof(HolyWordTestSpells), nameof(HolyWordTestSpells.LotnValueTests))]
        public double HWCDR_LotN_Values(Spell spell, int rank)
        {
            // Arrange
            _gameStateService.SetTalentRank(_state, Spell.LightOfTheNaaru, rank);

            // Act


            // Assert
            return Math.Round(_gameStateService.GetTotalHolyWordCooldownReduction(_state, spell), 10);
        }

        [TestCaseSource(typeof(HolyWordTestSpells), nameof(HolyWordTestSpells.ApothValueTests))]
        public double HWCDR_Apoth_Values(Spell spell)
        {
            // Arrange
            _gameStateService.SetTalentRank(_state, Spell.Apotheosis, 1);

            // Act


            // Assert
            return Math.Round(_gameStateService.GetTotalHolyWordCooldownReduction(_state, spell, true), 10);
        }

        [TestCaseSource(typeof(HolyWordTestSpells), nameof(HolyWordTestSpells.ApothLotnValueTests))]
        public double HWCDR_Apoth_LotN_Values(Spell spell, int rank)
        {
            // Arrange
            _gameStateService.SetTalentRank(_state, Spell.Apotheosis, 1);
            _gameStateService.SetTalentRank(_state, Spell.LightOfTheNaaru, rank);

            // Act


            // Assert
            return Math.Round(_gameStateService.GetTotalHolyWordCooldownReduction(_state, spell), 10);
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
                yield return new TestCaseData(Spell.Renew).Returns(2d);
                yield return new TestCaseData(Spell.CircleOfHealing).Returns(0);
                yield return new TestCaseData(Spell.PrayerOfMending).Returns(0);
                yield return new TestCaseData(Spell.HolyWordSerenity).Returns(30d);
                yield return new TestCaseData(Spell.HolyWordSanctify).Returns(30d);
                yield return new TestCaseData(Spell.Smite).Returns(4d);
                yield return new TestCaseData(Spell.HolyFire).Returns(0);
            }
        }
        public static IEnumerable LotnValueTests
        {
            get
            {
                yield return new TestCaseData(Spell.FlashHeal, 1).Returns(6.6d);
                yield return new TestCaseData(Spell.Heal, 1).Returns(6.6d);
                yield return new TestCaseData(Spell.PrayerOfHealing, 1).Returns(6.6d);
                yield return new TestCaseData(Spell.Renew, 1).Returns(2.2d);
                yield return new TestCaseData(Spell.CircleOfHealing, 1).Returns(0);
                yield return new TestCaseData(Spell.PrayerOfMending, 1).Returns(0);
                yield return new TestCaseData(Spell.HolyWordSerenity, 1).Returns(33.0d);
                yield return new TestCaseData(Spell.HolyWordSanctify, 1).Returns(33.0d);
                yield return new TestCaseData(Spell.Smite, 1).Returns(4.4d);
                yield return new TestCaseData(Spell.HolyFire, 1).Returns(0);

                yield return new TestCaseData(Spell.FlashHeal, 2).Returns(7.2d);
                yield return new TestCaseData(Spell.Heal, 2).Returns(7.2d);
                yield return new TestCaseData(Spell.PrayerOfHealing, 2).Returns(7.2d);
                yield return new TestCaseData(Spell.Renew, 2).Returns(2.4d);
                yield return new TestCaseData(Spell.CircleOfHealing, 2).Returns(0);
                yield return new TestCaseData(Spell.PrayerOfMending, 2).Returns(0);
                yield return new TestCaseData(Spell.HolyWordSerenity, 2).Returns(36.0d);
                yield return new TestCaseData(Spell.HolyWordSanctify, 2).Returns(36.0d);
                yield return new TestCaseData(Spell.Smite, 2).Returns(4.8d);
                yield return new TestCaseData(Spell.HolyFire, 2).Returns(0);
            }
        }
        public static IEnumerable ApothValueTests
        {
            get
            {
                yield return new TestCaseData(Spell.FlashHeal).Returns(24d);
                yield return new TestCaseData(Spell.Heal).Returns(24d);
                yield return new TestCaseData(Spell.PrayerOfHealing).Returns(24d);
                yield return new TestCaseData(Spell.Renew).Returns(8d);
                yield return new TestCaseData(Spell.CircleOfHealing).Returns(0);
                yield return new TestCaseData(Spell.PrayerOfMending).Returns(0);
                yield return new TestCaseData(Spell.HolyWordSerenity).Returns(30d);
                yield return new TestCaseData(Spell.HolyWordSanctify).Returns(30d);
                yield return new TestCaseData(Spell.Smite).Returns(16d);
                yield return new TestCaseData(Spell.HolyFire).Returns(0);
            }
        }
        public static IEnumerable ApothLotnValueTests
        {
            get
            {
                yield return new TestCaseData(Spell.FlashHeal, 1).Returns(26.4d);
                yield return new TestCaseData(Spell.Heal, 1).Returns(26.4d);
                yield return new TestCaseData(Spell.PrayerOfHealing, 1).Returns(26.4d);
                yield return new TestCaseData(Spell.Renew, 1).Returns(8.8d);
                yield return new TestCaseData(Spell.CircleOfHealing, 1).Returns(0);
                yield return new TestCaseData(Spell.PrayerOfMending, 1).Returns(0);
                yield return new TestCaseData(Spell.HolyWordSerenity, 1).Returns(33.0d);
                yield return new TestCaseData(Spell.HolyWordSanctify, 1).Returns(33.0d);
                yield return new TestCaseData(Spell.Smite, 1).Returns(17.6d);
                yield return new TestCaseData(Spell.HolyFire, 1).Returns(0);

                yield return new TestCaseData(Spell.FlashHeal, 2).Returns(28.8d);
                yield return new TestCaseData(Spell.Heal, 2).Returns(28.8d);
                yield return new TestCaseData(Spell.PrayerOfHealing, 2).Returns(28.8d);
                yield return new TestCaseData(Spell.Renew, 2).Returns(9.6d);
                yield return new TestCaseData(Spell.CircleOfHealing, 2).Returns(0);
                yield return new TestCaseData(Spell.PrayerOfMending, 2).Returns(0);
                yield return new TestCaseData(Spell.HolyWordSerenity, 2).Returns(36.0d);
                yield return new TestCaseData(Spell.HolyWordSanctify, 2).Returns(36.0d);
                yield return new TestCaseData(Spell.Smite, 2).Returns(19.2d);
                yield return new TestCaseData(Spell.HolyFire, 2).Returns(0);
            }
        }
    }
}
