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
        GameState _state;

        [SetUp]
        public void Init()
        {
            _state = GetGameState();
            _gameStateService = new GameStateService();
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
        public double HWCDR_LotN_Values(Spell spell, int haRank, int lotnRank)
        {
            // Arrange
            _gameStateService.SetTalentRank(_state, Spell.LightOfTheNaaru, lotnRank);
            _gameStateService.SetTalentRank(_state, Spell.HarmoniousApparatus, haRank);

            // Act


            // Assert
            return _gameStateService.GetTotalHolyWordCooldownReduction(_state, spell);
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

        [TestCaseSource(typeof(HarmoniousApparatusTestSpells), nameof(HarmoniousApparatusTestSpells.ApothLotnValueTests))]
        public double HWCDR_Apoth_LotN_Values(Spell spell, int haRank, int lotnRank)
        {
            // Arrange
            _gameStateService.SetTalentRank(_state, Spell.Apotheosis, 1);
            _gameStateService.SetTalentRank(_state, Spell.HarmoniousApparatus, haRank);
            _gameStateService.SetTalentRank(_state, Spell.LightOfTheNaaru, lotnRank);

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
                yield return new TestCaseData(Spell.CircleOfHealing, 1, 1).Returns(2.2d);
                yield return new TestCaseData(Spell.PrayerOfMending, 1, 1).Returns(2.2d);
                yield return new TestCaseData(Spell.HolyFire, 1, 1).Returns(2.2d);

                yield return new TestCaseData(Spell.CircleOfHealing, 2, 1).Returns(4.4d);
                yield return new TestCaseData(Spell.PrayerOfMending, 2, 1).Returns(4.4d);
                yield return new TestCaseData(Spell.HolyFire, 2, 1).Returns(4.4d);

                yield return new TestCaseData(Spell.CircleOfHealing, 1, 2).Returns(2.4d);
                yield return new TestCaseData(Spell.PrayerOfMending, 1, 2).Returns(2.4d);
                yield return new TestCaseData(Spell.HolyFire, 1, 2).Returns(2.4d);

                yield return new TestCaseData(Spell.CircleOfHealing, 2, 2).Returns(4.8d);
                yield return new TestCaseData(Spell.PrayerOfMending, 2, 2).Returns(4.8d);
                yield return new TestCaseData(Spell.HolyFire, 2, 2).Returns(4.8d);
            }
        }
        public static IEnumerable ApothValueTests
        {
            get
            {
                yield return new TestCaseData(Spell.CircleOfHealing, 1).Returns(8.0d);
                yield return new TestCaseData(Spell.PrayerOfMending, 1).Returns(8.0d);
                yield return new TestCaseData(Spell.HolyFire, 1).Returns(8.0d);
                yield return new TestCaseData(Spell.CircleOfHealing, 2).Returns(16.0d);
                yield return new TestCaseData(Spell.PrayerOfMending, 2).Returns(16.0d);
                yield return new TestCaseData(Spell.HolyFire, 2).Returns(16.0d);
            }
        }
        public static IEnumerable ApothLotnValueTests
        {
            get
            {
                yield return new TestCaseData(Spell.CircleOfHealing, 1, 1).Returns(8.8d);
                yield return new TestCaseData(Spell.PrayerOfMending, 1, 1).Returns(8.8d);
                yield return new TestCaseData(Spell.HolyFire, 1, 1).Returns(8.8d);

                yield return new TestCaseData(Spell.CircleOfHealing, 2, 1).Returns(17.6d);
                yield return new TestCaseData(Spell.PrayerOfMending, 2, 1).Returns(17.6d);
                yield return new TestCaseData(Spell.HolyFire, 2, 1).Returns(17.6d);

                yield return new TestCaseData(Spell.CircleOfHealing, 1, 2).Returns(9.6d);
                yield return new TestCaseData(Spell.PrayerOfMending, 1, 2).Returns(9.6d);
                yield return new TestCaseData(Spell.HolyFire, 1, 2).Returns(9.6d);

                yield return new TestCaseData(Spell.CircleOfHealing, 2, 2).Returns(19.2d);
                yield return new TestCaseData(Spell.PrayerOfMending, 2, 2).Returns(19.2d);
                yield return new TestCaseData(Spell.HolyFire, 2, 2).Returns(19.2d);
            }
        }
    }
}
