using NUnit.Framework;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Profile;
using Salvation.Core.State;

namespace Salvation.CoreTests.State
{
    [TestFixture]
    class StatOverrideTests : BaseTest
    {
        GameStateService _gameStateService;
        private GameState _state;

        [SetUp]
        public void Init()
        {
            _state = GetGameState();
            _gameStateService = new GameStateService();
        }

        [Test]
        public void CriticalStrikeRating_AppliesOverride()
        {
            // Arrange
            PlaystyleEntry playstyle = new PlaystyleEntry("OverrideStatCriticalStrike", 132513);

            // Act
            _gameStateService.OverridePlaystyle(_state, playstyle);
            var statValue = _gameStateService.GetCriticalStrikeRating(_state);
            // Assert
            Assert.AreEqual(132513, statValue);
        }

        [Test]
        public void VersatilityRating_AppliesOverride()
        {
            // Arrange
            PlaystyleEntry playstyle = new PlaystyleEntry("OverrideStatCriticalStrike", 132513);

            // Act
            _gameStateService.OverridePlaystyle(_state, playstyle);
            var statValue = _gameStateService.GetCriticalStrikeRating(_state);
            // Assert
            Assert.AreEqual(132513, statValue);
        }

        [Test]
        public void HasteRating_AppliesOverride()
        {
            // Arrange
            PlaystyleEntry playstyle = new PlaystyleEntry("OverrideStatCriticalStrike", 132513);

            // Act
            _gameStateService.OverridePlaystyle(_state, playstyle);
            var statValue = _gameStateService.GetCriticalStrikeRating(_state);
            // Assert
            Assert.AreEqual(132513, statValue);
        }

        [Test]
        public void MasteryRating_AppliesOverride()
        {
            // Arrange
            PlaystyleEntry playstyle = new PlaystyleEntry("OverrideStatCriticalStrike", 132513);

            // Act
            _gameStateService.OverridePlaystyle(_state, playstyle);
            var statValue = _gameStateService.GetCriticalStrikeRating(_state);
            // Assert
            Assert.AreEqual(132513, statValue);
        }

        [Test]
        public void Intellect_AppliesOverride()
        {
            // Arrange
            PlaystyleEntry playstyle = new PlaystyleEntry("OverrideStatCriticalStrike", 132513);

            // Act
            _gameStateService.OverridePlaystyle(_state, playstyle);
            var statValue = _gameStateService.GetCriticalStrikeRating(_state);
            // Assert
            Assert.AreEqual(132513, statValue);
        }
    }
}
