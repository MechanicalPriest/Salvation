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

            var harmoniousApparatusItem = new Item()
            {
                Equipped = true
            };
            harmoniousApparatusItem.Effects.Add(new ItemEffect()
            {
                Spell = new Core.Constants.BaseSpellData()
                {
                    Id = (int)Spell.HarmoniousApparatus
                }
            });
            _state.Profile.Items.Add(harmoniousApparatusItem);
        }

        [TestCaseSource(typeof(HarmoniousApparatusTestSpells), nameof(HarmoniousApparatusTestSpells.BaseValueTests))]
        public double HWCDR_Base_Values(Spell spell)
        {
            // Arrange


            // Act


            // Assert
            return _gameStateService.GetTotalHolyWordCooldownReduction(_state, spell);
        }

        [TestCaseSource(typeof(HarmoniousApparatusTestSpells), nameof(HarmoniousApparatusTestSpells.LotnValueTests))]
        public double HWCDR_LotN_Values(Spell spell)
        {
            // Arrange
            _gameStateService.SetActiveTalent(_state, Talent.LightOfTheNaaru);

            // Act


            // Assert
            return Math.Round(_gameStateService.GetTotalHolyWordCooldownReduction(_state, spell), 10);
        }

        [TestCaseSource(typeof(HarmoniousApparatusTestSpells), nameof(HarmoniousApparatusTestSpells.LotnHoValueTests))]
        public double HWCDR_LotN_HO_Values(Spell spell)
        {
            // Arrange
            _gameStateService.SetActiveTalent(_state, Talent.LightOfTheNaaru);
            _profileService.AddActiveConduit(_state.Profile, Conduit.HolyOration, 0);

            // Act


            // Assert
            return Math.Round(_gameStateService.GetTotalHolyWordCooldownReduction(_state, spell), 10);
        }

        [TestCaseSource(typeof(HarmoniousApparatusTestSpells), nameof(HarmoniousApparatusTestSpells.HoValueTests))]
        public double HWCDR_HO_Values(Spell spell)
        {
            // Arrange
            _profileService.AddActiveConduit(_state.Profile, Conduit.HolyOration, 0);

            // Act


            // Assert
            return Math.Round(_gameStateService.GetTotalHolyWordCooldownReduction(_state, spell), 10);
        }

        [TestCaseSource(typeof(HarmoniousApparatusTestSpells), nameof(HarmoniousApparatusTestSpells.ApothValueTests))]
        public double HWCDR_Apoth_Values(Spell spell)
        {
            // Arrange
            _gameStateService.SetActiveTalent(_state, Talent.Apotheosis);

            // Act


            // Assert
            return Math.Round(_gameStateService.GetTotalHolyWordCooldownReduction(_state, spell, true), 10);
        }

        [TestCaseSource(typeof(HarmoniousApparatusTestSpells), nameof(HarmoniousApparatusTestSpells.ApothHoValueTests))]
        public double HWCDR_Apoth_HO_Values(Spell spell)
        {
            // Arrange
            _gameStateService.SetActiveTalent(_state, Talent.Apotheosis);
            _profileService.AddActiveConduit(_state.Profile, Conduit.HolyOration, 0);

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
                yield return new TestCaseData(Spell.CircleOfHealing).Returns(4d);
                yield return new TestCaseData(Spell.PrayerOfMending).Returns(4d);
                yield return new TestCaseData(Spell.HolyFire).Returns(4d);
            }
        }
        public static IEnumerable LotnValueTests
        {
            get
            {
                yield return new TestCaseData(Spell.CircleOfHealing).Returns(5.3333333333d);
                yield return new TestCaseData(Spell.PrayerOfMending).Returns(5.3333333333d);
                yield return new TestCaseData(Spell.HolyFire).Returns(5.3333333333d);
            }
        }
        public static IEnumerable LotnHoValueTests
        {
            get
            {
                yield return new TestCaseData(Spell.CircleOfHealing).Returns(5.5733333333d);
                yield return new TestCaseData(Spell.PrayerOfMending).Returns(5.5733333333d);
                yield return new TestCaseData(Spell.HolyFire).Returns(5.5733333333d);
            }
        }
        public static IEnumerable HoValueTests
        {
            get
            {
                yield return new TestCaseData(Spell.CircleOfHealing).Returns(4.24d);
                yield return new TestCaseData(Spell.PrayerOfMending).Returns(4.24d);
                yield return new TestCaseData(Spell.HolyFire).Returns(4.24d);
            }
        }
        public static IEnumerable ApothValueTests
        {
            get
            {
                yield return new TestCaseData(Spell.CircleOfHealing).Returns(16d);
                yield return new TestCaseData(Spell.PrayerOfMending).Returns(16d);
                yield return new TestCaseData(Spell.HolyFire).Returns(16d);
            }
        }
        public static IEnumerable ApothHoValueTests
        {
            get
            {
                yield return new TestCaseData(Spell.CircleOfHealing).Returns(16.24d);
                yield return new TestCaseData(Spell.PrayerOfMending).Returns(16.24d);
                yield return new TestCaseData(Spell.HolyFire).Returns(16.24d);
            }
        }
    }
}
