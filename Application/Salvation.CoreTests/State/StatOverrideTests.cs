using NUnit.Framework;
using Salvation.Core.Profile.Model;
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
            PlaystyleEntry playstyle = new PlaystyleEntry("OverrideStatVersatility", 132513);

            // Act
            _gameStateService.OverridePlaystyle(_state, playstyle);
            var statValue = _gameStateService.GetVersatilityRating(_state);

            // Assert
            Assert.AreEqual(132513, statValue);
        }

        [Test]
        public void HasteRating_AppliesOverride()
        {
            // Arrange
            PlaystyleEntry playstyle = new PlaystyleEntry("OverrideStatHaste", 132513);

            // Act
            _gameStateService.OverridePlaystyle(_state, playstyle);
            var statValue = _gameStateService.GetHasteRating(_state);

            // Assert
            Assert.AreEqual(132513, statValue);
        }

        [Test]
        public void MasteryRating_AppliesOverride()
        {
            // Arrange
            PlaystyleEntry playstyle = new PlaystyleEntry("OverrideStatMastery", 132513);

            // Act
            _gameStateService.OverridePlaystyle(_state, playstyle);
            var statValue = _gameStateService.GetMasteryRating(_state);

            // Assert
            Assert.AreEqual(132513, statValue);
        }

        [Test]
        public void Intellect_AppliesOverride()
        {
            // Arrange
            PlaystyleEntry playstyle = new PlaystyleEntry("OverrideStatIntellect", 132513);

            // Act
            _gameStateService.OverridePlaystyle(_state, playstyle);
            var statValue = _gameStateService.GetIntellect(_state);

            // Assert
            Assert.AreEqual(139138.64999999999d, statValue); // TODO: Fix this test when the Armor Skills bug is fixed
        }

        [Test]
        public void Leech_AppliesOverride()
        {
            // Arrange
            PlaystyleEntry playstyle = new PlaystyleEntry("OverrideStatLeech", 1234);

            // Act
            _gameStateService.OverridePlaystyle(_state, playstyle);
            var statValue = _gameStateService.GetLeechRating(_state);

            // Assert
            Assert.AreEqual(1234.0d, statValue);
        }
    }
}
