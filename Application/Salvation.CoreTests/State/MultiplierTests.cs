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
        public double CritMultiplierTest()
        {            
            // Arrange


            // Act


            // Assert
            return _gameStateService.GetCriticalStrikeMultiplier(_state);
        }
        [TestCaseSource(typeof(MultiplierTestData), nameof(MultiplierTestData.HasteTestData))]
        public double HasteMultiplierTest()
        {
            // Arrange


            // Act


            // Assert
            return _gameStateService.GetHasteMultiplier(_state);
        }
        [TestCaseSource(typeof(MultiplierTestData), nameof(MultiplierTestData.MasteryTestData))]
        public double MasteryMultiplierTest()
        {
            // Arrange


            // Act


            // Assert
            return _gameStateService.GetMasteryMultiplier(_state);
        }
        [TestCaseSource(typeof(MultiplierTestData), nameof(MultiplierTestData.VersTestData))]
        public double VersMultiplierTest()
        {
            // Arrange


            // Act


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
                yield return new TestCaseData().Returns(1.1265714285714286d);
            }
        }
        public static IEnumerable HasteTestData
        {
            get
            {
                yield return new TestCaseData().Returns(1.0733333333333333d);
            }
        }
        public static IEnumerable MasteryTestData
        {
            get
            {
                yield return new TestCaseData().Returns(1.1864285714285716d);
            }
        }
        public static IEnumerable VersTestData
        {
            get
            {
                yield return new TestCaseData().Returns(1.0347500000000001d);
            }
        }
    }
}
