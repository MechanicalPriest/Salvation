using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections;

namespace Salvation.CoreTests.State
{
    [TestFixture]
    class MultiplierTests
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
                .GetDefaultProfile(Spec.HolyPriest);

            _state = new GameState(profile, constants);

            _gameStateService = new GameStateService();
        }
        [TestCaseSource(typeof(MultiplierTestData), nameof(MultiplierTestData.CritTestData))]
        public double CritMultiplierTest(int crit_val)
        {            
            // Arrange


            // Act

            _state.Profile.CritRating = crit_val;
            // Assert
            return _gameStateService.GetCriticalStrikeMultiplier(_state);
        }
        [TestCaseSource(typeof(MultiplierTestData), nameof(MultiplierTestData.HasteTestData))]
        public double HasteMultiplierTest(int haste_val)
        {
            // Arrange


            // Act

            _state.Profile.HasteRating = haste_val;

            // Assert
            return _gameStateService.GetHasteMultiplier(_state);
        }
        [TestCaseSource(typeof(MultiplierTestData), nameof(MultiplierTestData.MasteryTestData))]
        public double MasteryMultiplierTest(int mastery_val)
        {
            // Arrange


            // Act

            _state.Profile.MasteryRating = mastery_val;

            // Assert
            return _gameStateService.GetMasteryMultiplier(_state);
        }
        [TestCaseSource(typeof(MultiplierTestData), nameof(MultiplierTestData.VersTestData))]
        public double VersMultiplierTest(int vers_val)
        {
            // Arrange


            // Act

            _state.Profile.VersatilityRating = vers_val;

            // Assert
            return _gameStateService.GetVersatilityMultiplier(_state);
        }
    }
    public class MultiplierTestData
    {
        // TODO: Get Proper Test Data that isn't generated from the methods that we are testing lol 
        // (it was correct to previous when not getting DR'ed
        public static IEnumerable CritTestData
        {
            get
            {
                yield return new TestCaseData(100).Returns(1.0785714285714285d);
                yield return new TestCaseData(1500).Returns(1.4628857142857221d); 
                yield return new TestCaseData(300).Returns(1.1357142857142857d);
            }
        }
        public static IEnumerable HasteTestData
        {
            get
            {
                yield return new TestCaseData(100).Returns(1.0303030303030303d);
                yield return new TestCaseData(1500).Returns(1.433666666666672d);
                yield return new TestCaseData(300).Returns(1.0909090909090908d);
            }
        }
        public static IEnumerable MasteryTestData
        {
            get
            {
                yield return new TestCaseData(100).Returns(1.1357142857142859d);
                yield return new TestCaseData(1500).Returns(1.5950357142857121d);
                yield return new TestCaseData(300).Returns(1.2071428571428573d);
            }
        }
        public static IEnumerable VersTestData
        {
            get
            {
                yield return new TestCaseData(100).Returns(1.0249999999999999d);
                yield return new TestCaseData(1500).Returns(1.3675250000000068d);
                yield return new TestCaseData(300).Returns(1.075d);
            }
        }
    }
}
