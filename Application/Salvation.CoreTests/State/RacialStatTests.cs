using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Profile;
using Salvation.Core.State;
using SimcProfileParser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salvation.CoreTests.State
{
    internal class RacialStatTests : BaseTest
    {
        IGameStateService _gameStateService;
        private GameState _state;

        [OneTimeSetUp]
        public async Task InitOnce()
        {
            _state = GetGameState();
            _gameStateService = new GameStateService();

            // Load the simc profile
            var profileStringBeitaky = await File.ReadAllTextAsync(
                Path.Combine("TestData", "Beitaky.simc"));

            var simcProfileService = new SimcProfileService(
                new SimcGenerationService(),
                new ProfileService()
                );

            // Update the profile with the simc data
            await simcProfileService.ApplySimcProfileAsync(
                profileStringBeitaky, _state.Profile);
        }

        [TestCaseSource(typeof(StatRatingTestCases), nameof(StatRatingTestCases.RacialIntellectValues))]
        public double CalculatesRacialIntellect(Race race)
        {
            // Arrange
            _state.Profile.Race = race;
            _state.Profile.Items = new System.Collections.Generic.List<Core.Profile.Model.Item>();
            var specData = _state.Constants.Specs.Where(s => s.SpecId == (int)_state.Profile.Spec).FirstOrDefault();
            specData.IntBase = 1000;

            // Act
            var intellect = _gameStateService.GetIntellect(_state);

            // Assert
            return intellect;
        }

        [TestCaseSource(typeof(StatRatingTestCases), nameof(StatRatingTestCases.RacialStaminaValues))]
        public double CalculatesRacialStamina(Race race)
        {
            // Arrange
            _state.Profile.Race = race;
            _state.Profile.Items = new System.Collections.Generic.List<Core.Profile.Model.Item>();
            var specData = _state.Constants.Specs.Where(s => s.SpecId == (int)_state.Profile.Spec).FirstOrDefault();
            specData.StamBase = 1000;

            // Act
            var stamina = _gameStateService.GetStamina(_state);

            // Assert
            return stamina;
        }
    }
    public class StatRatingTestCases
    {
        public static IEnumerable RacialIntellectValues
        {
            get
            {
                yield return new TestCaseData(Race.BloodElf).Returns(1002);
                yield return new TestCaseData(Race.DarkIronDwarf).Returns(999);
                yield return new TestCaseData(Race.DracthyrAlliance).Returns(1002);
                yield return new TestCaseData(Race.DracthyrHorde).Returns(1002);
                yield return new TestCaseData(Race.Draenei).Returns(1000);
                yield return new TestCaseData(Race.Dwarf).Returns(999);
                yield return new TestCaseData(Race.Gnome).Returns(1003);
                yield return new TestCaseData(Race.Goblin).Returns(1003);
                yield return new TestCaseData(Race.HighmountainTauren).Returns(999);
                yield return new TestCaseData(Race.Human).Returns(1000);
                yield return new TestCaseData(Race.KulTiran).Returns(999);
                yield return new TestCaseData(Race.LightforgedDraenei).Returns(1000);
                yield return new TestCaseData(Race.MagharOrc).Returns(999);
                yield return new TestCaseData(Race.Mechagnome).Returns(1002);
                yield return new TestCaseData(Race.Nightborne).Returns(1002);
                yield return new TestCaseData(Race.NightElf).Returns(1000);
                yield return new TestCaseData(Race.NoRace).Returns(1000);
                yield return new TestCaseData(Race.Orc).Returns(999);
                yield return new TestCaseData(Race.Pandaren).Returns(1000);
                yield return new TestCaseData(Race.PandarenAlliance).Returns(1000);
                yield return new TestCaseData(Race.PandarenHorde).Returns(1000);
                yield return new TestCaseData(Race.Tauren).Returns(998);
                yield return new TestCaseData(Race.Troll).Returns(997);
                yield return new TestCaseData(Race.Undead).Returns(998);
                yield return new TestCaseData(Race.VoidElf).Returns(1002);
                yield return new TestCaseData(Race.Vulpera).Returns(1001);
                yield return new TestCaseData(Race.Worgen).Returns(997);
                yield return new TestCaseData(Race.ZandalariTroll).Returns(997);
            }
        }

        public static IEnumerable RacialStaminaValues
        {
            get
            {
                yield return new TestCaseData(Race.BloodElf).Returns(1000);
                yield return new TestCaseData(Race.DarkIronDwarf).Returns(1001);
                yield return new TestCaseData(Race.DracthyrAlliance).Returns(998);
                yield return new TestCaseData(Race.DracthyrHorde).Returns(998);
                yield return new TestCaseData(Race.Draenei).Returns(1002);
                yield return new TestCaseData(Race.Dwarf).Returns(1001);
                yield return new TestCaseData(Race.Gnome).Returns(999);
                yield return new TestCaseData(Race.Goblin).Returns(999);
                yield return new TestCaseData(Race.HighmountainTauren).Returns(1002);
                yield return new TestCaseData(Race.Human).Returns(1000);
                yield return new TestCaseData(Race.KulTiran).Returns(1002);
                yield return new TestCaseData(Race.LightforgedDraenei).Returns(1001);
                yield return new TestCaseData(Race.MagharOrc).Returns(1001);
                yield return new TestCaseData(Race.Mechagnome).Returns(999);
                yield return new TestCaseData(Race.Nightborne).Returns(1000);
                yield return new TestCaseData(Race.NightElf).Returns(1000);
                yield return new TestCaseData(Race.NoRace).Returns(1000);
                yield return new TestCaseData(Race.Orc).Returns(1001);
                yield return new TestCaseData(Race.Pandaren).Returns(1002);
                yield return new TestCaseData(Race.PandarenAlliance).Returns(1002);
                yield return new TestCaseData(Race.PandarenHorde).Returns(1002);
                yield return new TestCaseData(Race.Tauren).Returns(1002);
                yield return new TestCaseData(Race.Troll).Returns(1000);
                yield return new TestCaseData(Race.Undead).Returns(1001);
                yield return new TestCaseData(Race.VoidElf).Returns(1000);
                yield return new TestCaseData(Race.Vulpera).Returns(999);
                yield return new TestCaseData(Race.Worgen).Returns(1000);
                yield return new TestCaseData(Race.ZandalariTroll).Returns(1000);
            }
        }
    }
}
