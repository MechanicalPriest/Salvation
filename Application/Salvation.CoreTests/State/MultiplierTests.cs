using NUnit.Framework;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;

namespace Salvation.CoreTests.State
{
    [TestFixture]
    class MultiplierTests : BaseTest
    {
        IGameStateService _gameStateService;
        private GameState _state;

        [OneTimeSetUp]
        public void InitOnce()
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
            Assert.AreEqual(1.1180000000000001d, crit);
        }

        [Test]
        public void HasteMultiplierTest()
        {
            // Arrange


            // Act
            var haste = _gameStateService.GetHasteMultiplier(_state);

            // Assert
            Assert.AreEqual(1.1293939393939394d, haste);
        }

        [Test]
        public void MasteryMultiplierTest()
        {
            // Arrange


            // Act
            var mastery = _gameStateService.GetMasteryMultiplier(_state);

            // Assert
            Assert.AreEqual(1.1357142857142859d, mastery);
        }

        [Test]
        public void VersMultiplierTest()
        {
            // Arrange


            // Act
            var vers = _gameStateService.GetVersatilityMultiplier(_state);

            // Assert
            Assert.AreEqual(1.2030000000000001d, vers);
        }
    }
}
