using NUnit.Framework;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile.Model;
using Salvation.Core.State;
using System;

namespace Salvation.CoreTests.HolyPriest.Spells
{
    [TestFixture]
    public class TwistOfFateTests : BaseTest
    {
        private GameState _gameState;
        private ISpellService _spell;
        IGameStateService _gameStateService;

        [SetUp]
        public void Init()
        {
            _gameStateService = new GameStateService();
            _spell = new TwistOfFate(_gameStateService);

            _gameState = GetGameState();
        }

        [Test]
        public void GetAverageHealingMultiplier_UnTalented()
        {
            // Arrange

            // Act
            var result = _spell.GetAverageHealingMultiplier(_gameState, null);

            // Assert
            Assert.That(1.0d, Is.EqualTo(result));
        }

        [Test]
        public void GetAverageHealingMultiplier_Talented_Ranks()
        {
            // Arrange
            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("TwistOfFateUptime", 0.8));

            // Act
            _gameStateService.SetTalentRank(_gameState, Spell.TwistOfFate, 1);
            var resultRank1 = _spell.GetAverageHealingMultiplier(_gameState, null);

            _gameStateService.SetTalentRank(_gameState, Spell.TwistOfFate, 2);
            var resultRank2 = _spell.GetAverageHealingMultiplier(_gameState, null);

            // Assert
            Assert.That(1.04d, Is.EqualTo(resultRank1));
            Assert.That(1.08d, Is.EqualTo(resultRank2));
        }

        [Test]
        public void GetUptime()
        {
            // Arrange
            _gameStateService.SetTalentRank(_gameState, Spell.TwistOfFate, 1);

            _gameStateService.OverridePlaystyle(_gameState,
                new PlaystyleEntry("TwistOfFateUptime", 0.8));

            // Act
            var result = _spell.GetUptime(_gameState, null);

            // Assert
            Assert.That(0.80000000000000004d, Is.EqualTo(result));
        }

        [Test]
        public void GetAverageHealingMultiplier_Throws_No_TwistedFateUptime()
        {
            // Arrange
            _gameStateService.SetTalentRank(_gameState, Spell.TwistOfFate, 1);

            // Act
            var methodCall = new TestDelegate(
                () => _spell.GetAverageHealingMultiplier(_gameState, null));

            // Assert
            ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(methodCall);
            Assert.That(ex.Message, Is.EqualTo("TwistOfFateUptime needs to be set. (Parameter 'TwistOfFateUptime')"));
            Assert.That(ex.ParamName, Is.EqualTo("TwistOfFateUptime"));
        }
    }
}
