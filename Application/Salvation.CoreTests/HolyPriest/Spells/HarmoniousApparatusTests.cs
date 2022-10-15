using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Profile;
using Salvation.Core.Profile.Model;
using Salvation.Core.State;
using System;
using System.Collections;

namespace Salvation.CoreTests.HolyPriest.Spells
{
    [TestFixture]
    public class HarmoniousApparatusTests : BaseTest
    {
        IGameStateService _gameStateService;
        ProfileService _profileService;
        GameState _state;

        [SetUp]
        public void Init()
        {
            _state = GetGameState();
            _gameStateService = new GameStateService();
            _profileService = new ProfileService();
        }

        [TestCaseSource(typeof(HarmoniousApparatusTestSpells), nameof(HarmoniousApparatusTestSpells.BaseValueTests))]
        public double HWCDR_Base_Values(Spell spell, int rank)
        {
            // Arrange
            _gameStateService.SetTalentRank(_state, Spell.HarmoniousApparatus, rank);

            // Act


            // Assert
            return _gameStateService.GetTotalHolyWordCooldownReduction(_state, spell);
        }

        [TestCaseSource(typeof(HarmoniousApparatusTestSpells), nameof(HarmoniousApparatusTestSpells.LotnValueTests))]
        public double HWCDR_LotN_Values(Spell spell, int rank)
        {
            // Arrange
            _gameStateService.SetTalentRank(_state, Spell.LightOfTheNaaru, 1);
            _gameStateService.SetTalentRank(_state, Spell.HarmoniousApparatus, rank);

            // Act


            // Assert
            return Math.Round(_gameStateService.GetTotalHolyWordCooldownReduction(_state, spell), 10);
        }

        [TestCaseSource(typeof(HarmoniousApparatusTestSpells), nameof(HarmoniousApparatusTestSpells.ApothValueTests))]
        public double HWCDR_Apoth_Values(Spell spell, int rank)
        {
            // Arrange
            _gameStateService.SetTalentRank(_state, Spell.Apotheosis, 1);
            _gameStateService.SetTalentRank(_state, Spell.HarmoniousApparatus, rank);

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

    public class HarmoniousApparatusTestSpells
    {
        public static IEnumerable BaseValueTests
        {
            get
            {
                yield return new TestCaseData(Spell.CircleOfHealing, 1).Returns(2d);
                yield return new TestCaseData(Spell.PrayerOfMending, 1).Returns(2d);
                yield return new TestCaseData(Spell.HolyFire, 1).Returns(2d);
                yield return new TestCaseData(Spell.CircleOfHealing, 2).Returns(4d);
                yield return new TestCaseData(Spell.PrayerOfMending, 2).Returns(4d);
                yield return new TestCaseData(Spell.HolyFire, 2).Returns(4d);
            }
        }
        public static IEnumerable LotnValueTests
        {
            get
            {
                yield return new TestCaseData(Spell.CircleOfHealing, 1).Returns(5.3333333333d);
                yield return new TestCaseData(Spell.PrayerOfMending, 1).Returns(5.3333333333d);
                yield return new TestCaseData(Spell.HolyFire, 1).Returns(5.3333333333d);
                yield return new TestCaseData(Spell.CircleOfHealing, 2).Returns(5.3333333333d);
                yield return new TestCaseData(Spell.PrayerOfMending, 2).Returns(5.3333333333d);
                yield return new TestCaseData(Spell.HolyFire, 2).Returns(5.3333333333d);
            }
        }
        public static IEnumerable ApothValueTests
        {
            get
            {
                yield return new TestCaseData(Spell.CircleOfHealing, 1).Returns(8.0d);
                yield return new TestCaseData(Spell.PrayerOfMending, 1).Returns(8.0d);
                yield return new TestCaseData(Spell.HolyFire, 1).Returns(8.0d);
                yield return new TestCaseData(Spell.CircleOfHealing, 2).Returns(16d);
                yield return new TestCaseData(Spell.PrayerOfMending, 2).Returns(16d);
                yield return new TestCaseData(Spell.HolyFire, 2).Returns(16d);
            }
        }
    }
}
