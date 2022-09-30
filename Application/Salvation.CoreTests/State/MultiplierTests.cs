using NUnit.Framework;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;

namespace Salvation.CoreTests.State
{
    [TestFixture]
    class MultiplierTests : BaseTest
    {
        IGameStateService _gameStateService;
        private GameState _state;

        [SetUp]
        public void Init()
        {
            _state = GetGameState();
            _gameStateService = new GameStateService();
        }

        [Test]
        public void CritMultiplierTest()
        {
            // Arrange


            // Act
            var crit = _gameStateService.GetCriticalStrikeMultiplier(_state);

            // Assert
            Assert.AreEqual(1.0632222222222223d, crit);
        }

        [Test]
        public void CritMultiplier_Increased_By_Racial()
        {
            // Arrange
            _state.Profile.Race = Race.Dwarf;
            var baseState = GetGameState();
            baseState.Profile.Race = Race.Orc;

             // Act
             var crit = _gameStateService.GetCriticalStrikeMultiplier(_state);
             var critbase = _gameStateService.GetCriticalStrikeMultiplier(baseState);

            // Assert
            Assert.AreEqual(1.0644866666666666d, crit);
            Assert.AreEqual(1.0632222222222223d, critbase);
            Assert.Less(critbase, crit);
        }

        [Test]
        public void HasteMultiplierTest()
        {
            // Arrange


            // Act
            var haste = _gameStateService.GetHasteMultiplier(_state);

            // Assert
            Assert.AreEqual(1.0251176470588235d, haste);
        }

        [Test]
        public void MasteryMultiplierTest()
        {
            // Arrange


            // Act
            var mastery = _gameStateService.GetMasteryMultiplier(_state);

            // Assert
            Assert.AreEqual(1.1069444444444445d, mastery);
        }

        [Test]
        public void VersMultiplierTest()
        {
            // Arrange


            // Act
            var vers = _gameStateService.GetVersatilityMultiplier(_state);

            // Assert
            Assert.AreEqual(1.039609756097561d, vers);
        }

        [Test]
        public void LeechMultiplierTest()
        {
            // Arrange


            // Act
            var leech = _gameStateService.GetLeechMultiplier(_state);

            // Assert
            Assert.AreEqual(1.0d, leech);
        }
    }
}
